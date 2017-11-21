using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    public class AbstractEntity
    {
        [AutumnKey()]
        [AutumnProperty(Updatable = false)]
        public int Id { get; set; }
    }

}