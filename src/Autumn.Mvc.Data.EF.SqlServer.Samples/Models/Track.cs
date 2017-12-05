using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.SqlServer.Samples.Models
{
    [Entity]
    public class Track : AbstractEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [Required]
        public int AlbumId { get; set; }
       
        [Required]
        public int MediaTypeId { get; set; }
       
        [MaxLength(220)]
        public string Composer { get; set; }
        
        [Required]
        public int GenreId { get; set; }
        
        [Required]
        public int Milliseconds { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
    }
}