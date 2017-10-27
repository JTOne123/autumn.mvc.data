using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication1.Commons
{
    public class SpecificationModelBinderProvider : IModelBinderProvider
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> BinderTypes = new ConcurrentDictionary<Type, ConstructorInfo>();
        
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.IsGenericType && context.Metadata.ModelType.GetGenericTypeDefinition()==typeof(ISpecification<>))
            {
                if (!BinderTypes.ContainsKey(context.Metadata.ModelType))
                {
                    var entityType = context.Metadata.ModelType.GetGenericArguments()[0];
                    var binderTypeName = string.Format("{0}[[{2}, {3}]], {1}",
                        typeof(SpecificationModelBinder<>).FullName,
                        typeof(SpecificationModelBinder<>).Assembly.FullName,
                        entityType.FullName,
                        entityType.Assembly.FullName);
                    var binderType =
                        Type.GetType(binderTypeName);
                    BinderTypes[context.Metadata.ModelType] = binderType.GetConstructor(new Type[0]);
                }
                return (IModelBinder) BinderTypes[context.Metadata.ModelType].Invoke(null);
            }
            return null;
        }
    }
}