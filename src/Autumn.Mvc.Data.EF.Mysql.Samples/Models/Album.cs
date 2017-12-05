using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    /// <summary>
    /// Entity class for Album
    /// </summary>
    [Entity]
    public class Album : AbstractEntity
    {
        /// <summary>
        /// Title of album
        /// </summary>
        [Required]
        [MaxLength(160)]
        public string Title { get; set; }

        /// <summary>
        /// Artist Id
        /// </summary>
        [Required]
        public int ArtistId { get; set; }
    }
}