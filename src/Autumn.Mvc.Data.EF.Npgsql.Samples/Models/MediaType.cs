using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Npgsql.Samples.Models
{
    /// <summary>
    /// Entity class for media type
    /// </summary>
    [Resource]
    public class MediaType : AbstractEntity
    {
        [MaxLength(120)]
        public string Name { get; set; }
    }
}