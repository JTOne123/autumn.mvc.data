using Autumn.Mvc.Data.Configurations;

namespace Autumn.Mvc.Data.MongoDB.Configurations
{
    public class CollectionInfo
    {
        public EntityInfo EntityInfo { get; }
        public string Name { get; }

        public CollectionInfo(EntityInfo entityInfo, string name)
        {
            this.EntityInfo = entityInfo;
            this.Name = name;
        }
    }
}