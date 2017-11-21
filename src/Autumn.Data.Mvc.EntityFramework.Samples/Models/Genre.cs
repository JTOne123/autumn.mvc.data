using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    [AutumnEntity]
    public class Genre :AbstractEntity
    {
        public string Name { get; set; }
    }
}