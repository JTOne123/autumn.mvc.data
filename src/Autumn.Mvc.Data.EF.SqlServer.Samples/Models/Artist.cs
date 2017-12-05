using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.SqlServer.Samples.Models
{
    [Entity(Version = "v1")]
    public class Artist : AbstractEntity
    {
        [MinLength(4)]
        public string Name { get; set; }
    }
}