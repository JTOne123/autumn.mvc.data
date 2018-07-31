using System;
using System.Reflection;
using Autumn.Mvc.Data.Annotations;


namespace Autumn.Mvc.Data.Configurations
{
    public class EntityInfo
    {
        public Type EntityType { get; }
        public string Name { get; }
        public AutumnDataSettings Settings { get; set; }
        public PropertyInfo KeyInfo { get; }
        public PropertyInfo CreatedDateInfo { get; }
        public PropertyInfo LastModifiedDateInfo { get; }
        public PropertyInfo CreatedByInfo { get; }
        public PropertyInfo LastModifiedByInfo { get; }

        public EntityInfo(
            AutumnDataSettings dataSettings,
            Type entityType,
            EntityAttribute entityAttribute,
            PropertyInfo keyInfo,
            PropertyInfo createdDateInfo,
            PropertyInfo lastModifiedDateInfo,
            PropertyInfo createdByInfo,
            PropertyInfo lastModifiedByInfo)
        {
            Settings = dataSettings ?? throw new ArgumentNullException(nameof(dataSettings));
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Name = entityAttribute?.Name ?? entityType.Name;
            KeyInfo = keyInfo;
            CreatedDateInfo = createdDateInfo;
            LastModifiedDateInfo = lastModifiedDateInfo;
            CreatedByInfo = createdByInfo;
            LastModifiedByInfo = lastModifiedByInfo;
        }
    }
}