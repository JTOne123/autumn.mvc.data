using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Npgsql.Samples.Models
{
    [Resource]
    public class Album : AbstractEntity
    {
        [Required]
        [MaxLength(160)]
        public string Title { get; set; }

        [Required]
        public int ArtistId { get; set; }
    }
}