using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    [AutumnEntity(Name = "artist",Version = "v1")]
    public class Artist : AbstractEntity
    {
        [MinLength(4)]
        public string Name { get; set; }
    }
}