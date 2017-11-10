using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Autumn.Mvc.Data.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdAttribute : Attribute
    {
        public bool Insertable { get; set; }
        public bool Updatable { get; set; }
        
        private static readonly Dictionary<Type, PropertyInfo> Ids=new Dictionary<Type, PropertyInfo>();
        
        public static PropertyInfo GetOrRegisterId<T>()
        {
            lock (Ids)
            {
                if (Ids.ContainsKey(typeof(T))) return Ids[typeof(T)];
                var propertyInfo = typeof(T).GetProperties()
                    .Single(p => p.GetCustomAttribute<IdAttribute>() != null);
                Ids.Add(typeof(T), propertyInfo);
                return Ids[typeof(T)];
            }
        }
    }
}