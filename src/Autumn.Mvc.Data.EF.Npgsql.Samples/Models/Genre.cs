using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Npgsql.Samples.Models
{
    [Entity(Insertable = false, Updatable = false, Deletable = false)]
    public class Genre : AbstractEntity
    {
        public string Name { get; set; }
    }
}