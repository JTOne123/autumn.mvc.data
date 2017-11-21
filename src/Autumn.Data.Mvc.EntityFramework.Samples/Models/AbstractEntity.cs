using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    public class AbstractEntity
    {
        [AutumnKey()]
        [AutumnIgnore(AutumnIgnoreType.Put)]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
    }

}