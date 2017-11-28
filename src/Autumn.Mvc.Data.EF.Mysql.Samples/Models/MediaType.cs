using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    /// <summary>
    /// Entity class for media type
    /// </summary>
    [AutumnEntity]
    public class MediaType : AbstractEntity
    {
        [MaxLength(120)]
        public string Name { get; set; }
    }
}