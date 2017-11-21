using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data
{
    public static class AutumnTypeBuilder
    {
        public static Dictionary<AutumnIgnoreType, Type> CompileResultType(Type originType)
        {
            var typeBuilderPost = GetTypeBuilder(originType, AutumnIgnoreType.Post);
            typeBuilderPost.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                                     MethodAttributes.RTSpecialName);

            var typeBuilderPut = GetTypeBuilder(originType, AutumnIgnoreType.Put);
            typeBuilderPut.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                                    MethodAttributes.RTSpecialName);
            foreach (var property in originType.GetProperties())
            {
                CreateProperty(typeBuilderPost, property, AutumnIgnoreType.Post);
                CreateProperty(typeBuilderPut, property, AutumnIgnoreType.Put);
            }

            var result = new Dictionary<AutumnIgnoreType, Type>
            {
                [AutumnIgnoreType.Post] = typeBuilderPost.CreateTypeInfo().AsType(),
                [AutumnIgnoreType.Put] = typeBuilderPut.CreateTypeInfo().AsType()
            };

            return result;
        }

        private static TypeBuilder GetTypeBuilder(Type originType, AutumnIgnoreType type)
        {
            var typeSignature = originType.Name + "Proxy_" + type.ToString();
            var an = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var tb = moduleBuilder.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo, AutumnIgnoreType type)
        {
            var ignoreAttribute = propertyInfo.GetCustomAttribute<AutumnIgnoreAttribute>();
           
            if (ignoreAttribute?.Type == (AutumnIgnoreType.Post | AutumnIgnoreType.Put)) return;
            if (ignoreAttribute?.Type == AutumnIgnoreType.Put  && type == AutumnIgnoreType.Put) return;
            if (ignoreAttribute?.Type == AutumnIgnoreType.Post && type == AutumnIgnoreType.Post) return;

            var propertyName = propertyInfo.Name;
            var propertyType = propertyInfo.PropertyType;
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder =
                typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            foreach (var attribute in propertyInfo.GetCustomAttributes())
            {
                var customAttributeBuilder = BuildCustomAttribute(attribute);
                if (customAttributeBuilder != null)
                {
                    propertyBuilder.SetCustomAttribute(customAttributeBuilder);
                }
            }

            if (propertyInfo.CanRead)
            {
                var getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                    Type.EmptyTypes);
                var getIl = getPropMthdBldr.GetILGenerator();

                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(getPropMthdBldr);
            }

            if (!propertyInfo.CanWrite) return;
            var setPropMthdBldr =
                typeBuilder.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] {propertyType});

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();


            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        /// <summary>
        /// find the constructofInfo by attribute type
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static Tuple<ConstructorInfo, object[]> BuildConstuctorInfos(Attribute attribute)
        {
            ConstructorInfo constructorInfo;
            object[] args;

            switch (attribute)
            {
                case RangeAttribute rangeAttribute:
                    constructorInfo = typeof(RangeAttribute).GetConstructor(new[] {rangeAttribute.Minimum.GetType(), rangeAttribute.Maximum.GetType()});
                    args = new[] {rangeAttribute.Minimum, rangeAttribute.Maximum};
                    break;
                case RegularExpressionAttribute regularExpressionAttribute:
                    constructorInfo = typeof(RegularExpressionAttribute).GetConstructor(new[] {typeof(string)});
                    args = new object[] {regularExpressionAttribute.Pattern};
                    break;
                case MinLengthAttribute minLengthAttribute:
                    constructorInfo = typeof(MinLengthAttribute).GetConstructor(new[] {typeof(int)});
                    args = new object[] {minLengthAttribute.Length};
                    break;
                case MaxLengthAttribute maxLengthAttribute:
                    constructorInfo = typeof(MaxLengthAttribute).GetConstructor(new[] {typeof(int)});
                    args = new object[] {maxLengthAttribute.Length};
                    break;
                case CompareAttribute compareAttribute:
                    constructorInfo = typeof(CompareAttribute).GetConstructor(new[] {typeof(string)});
                    args = new object[] {compareAttribute.OtherProperty};
                    break;
                case StringLengthAttribute stringLengthAttribute:
                    constructorInfo = typeof(StringLengthAttribute).GetConstructor(new[] {typeof(int)});
                    args = new object[] {stringLengthAttribute.MaximumLength};
                    break;
                case EnumDataTypeAttribute enumDataTypeAttribute:
                    constructorInfo = typeof(EnumDataTypeAttribute).GetConstructor(new[] {typeof(Type)});
                    args = new object[] {enumDataTypeAttribute.EnumType};
                    break;
                default:
                    constructorInfo = attribute.GetType().GetConstructor(Type.EmptyTypes);
                    args = new object[] { };
                    break;
            }

            return new Tuple<ConstructorInfo, object[]>(constructorInfo, args);
        }

        private static CustomAttributeBuilder BuildCustomAttribute(Attribute attribute)
        {
            var type = attribute.GetType();
            if (type == typeof(AutumnIgnoreAttribute)) return null;
            if (!type.IsSubclassOf(typeof(ValidationAttribute)) && type.Namespace != "Newtonsoft.Json") return null;

            var constructorInfo = BuildConstuctorInfos(attribute);
            if (constructorInfo.Item1 == null) return null;

            PropertyInfo[] namedProperties;
            object[] propertyValues = null;

            namedProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite).ToArray();
            if (namedProperties.Length > 0)
            {
                propertyValues = (from p in namedProperties
                    select p.GetValue(attribute, null)).ToArray();

                #region remove null value properties

                var propertyValuesTmp = new List<object>();
                var namedPropertiesTmp = new List<PropertyInfo>();
                for (var i = 0; i < propertyValues.Length; i++)
                {
                    var val = propertyValues[i];
                    if (val == null) continue;
                    namedPropertiesTmp.Add(namedProperties[i]);
                    propertyValuesTmp.Add(val);
                }
                namedProperties = namedPropertiesTmp.ToArray();
                propertyValues = propertyValuesTmp.ToArray();

                #endregion
            }

            FieldInfo[] namedFields;
            object[] fieldValues = null;

            namedFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (namedFields.Length > 0)
            {
                (from f in namedFields
                    select f.GetValue(attribute)).ToArray();

                #region remove null value properties

                var fieldValuesTmp = new List<object>();
                var namedFieldsTmp = new List<FieldInfo>();
                for (var i = 0; i < namedFields.Length; i++)
                {
                    var val = namedFields[i];
                    if (val == null) continue;
                    namedFieldsTmp.Add(namedFields[i]);
                    fieldValuesTmp.Add(val);
                }
                namedFields = namedFieldsTmp.ToArray();
                fieldValues = fieldValuesTmp.ToArray();

                #endregion
            }

            if (namedFields.Length > 0 && namedProperties.Length > 0)
            {
                return new CustomAttributeBuilder(constructorInfo.Item1,
                    constructorInfo.Item2,
                    namedProperties,
                    propertyValues,
                    namedFields,
                    fieldValues);
            }
            if (namedFields.Length > 0)
            {
                return new CustomAttributeBuilder(constructorInfo.Item1,
                    constructorInfo.Item2,
                    namedFields,
                    fieldValues);
            }
            if (namedProperties.Length > 0)
            {
                return new CustomAttributeBuilder(constructorInfo.Item1,
                    constructorInfo.Item2,
                    namedProperties,
                    propertyValues);
            }
            return new CustomAttributeBuilder(constructorInfo.Item1,
                constructorInfo.Item2);
        }
    }
}