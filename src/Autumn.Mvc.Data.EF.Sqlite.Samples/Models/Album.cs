using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Sqlite.Samples.Models
{
    [Entity]
    public class Album : AbstractEntity
    {
        [Required]
        [MaxLength(160)]
        public string Title { get; set; }

        [Required]
        public int ArtistId { get; set; }
    }
}