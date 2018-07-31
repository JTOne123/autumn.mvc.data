using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Sqlite.Samples.Models
{
    [Resource(Insertable =false,Updatable =false,Deletable =false )]
    public class Customer : AbstractEntityWithAddress
    {
        [Required]
        [MaxLength(40)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(40)]
        public string LastName { get; set; }

        [MaxLength(80)]
        public string Company { get; set; }

        [Required]
        public int SupportRepId { get; set; }
    }
}