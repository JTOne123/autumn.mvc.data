using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    [AutumnEntity(Name = "genres",Version = "v1")]
    public class Genre :AbstractEntity
    {
        public string Name { get; set; }
    }
}