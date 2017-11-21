using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    [AutumnEntity(Name = "Album")]
    public class Album : AbstractEntity
    {
        [Required]
        [MaxLength(160)]
        public string Title { get; set; }

        [Required]
        public int ArtistId { get; set; }
    }
}