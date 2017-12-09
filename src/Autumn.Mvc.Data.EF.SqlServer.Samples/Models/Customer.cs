using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    [Entity]
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