using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autumn.Data.Rest.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Rest.MongoDB.Helpers
{
    public static class MongoDbHelper
    {
        private static readonly Dictionary<Type, PropertyInfo> Ids=new Dictionary<Type, PropertyInfo>();
        
        public static PropertyInfo GetId<T>()
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