using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.EF.Mysql.Samples.Models;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    [Resource]
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