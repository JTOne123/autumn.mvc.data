using System;
using System.Collections.Generic;
using System.Reflection;

namespace Autumn.Mvc.Data.Models.Queries
{
    public class AutumnMethodContainsInfo
    {
        private readonly ConstructorInfo _constructor;
        private readonly MethodInfo _addMethod;
        public MethodInfo ContainsMethod { get;  }

        public AutumnMethodContainsInfo(Type type)
        {
            var t = typeof(List<>).MakeGenericType(type);
            _constructor = t.GetConstructor(Type.EmptyTypes);
            _addMethod = t.GetMethod("Add", new[] {type});
            ContainsMethod = t.GetMethod("Contains", new[] {type});
        }

        public object Convert(List<object> values)
        {
            if (values == null) return null;
            var result = _constructor.Invoke(new object[] { });
            values.ForEach(a =>
            {
                _addMethod.Invoke(result, new object[] {a});
            });
            return result;
        }
    }
}