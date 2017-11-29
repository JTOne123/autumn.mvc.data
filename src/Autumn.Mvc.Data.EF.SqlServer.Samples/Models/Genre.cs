using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.SqlServer.Samples.Models
{
    [AutumnEntity(Insertable = false, Updatable = false, Deletable = false)]
    public class Genre : AbstractEntity
    {
        public string Name { get; set; }
    }
}