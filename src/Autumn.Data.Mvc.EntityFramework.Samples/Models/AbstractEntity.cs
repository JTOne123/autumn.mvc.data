using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    public class AbstractEntity
    {
        [AutumnKey(Insertable = true, Updatable = false)]
        public int Id { get; set; }
    }

}