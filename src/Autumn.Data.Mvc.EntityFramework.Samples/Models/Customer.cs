using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Data.Mvc.EntityFramework.Samples.Models
{
    [AutumnEntity]
    public class Customer : AbstractEntity
    {
        [Required]
        [MaxLength(40)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(40)]
        public string LastName { get; set; }

        [MaxLength(80)]
        public string Company { get; set; }

        [MaxLength(70)]
        public string Address { get; set; }

        [MaxLength(40)]
        public string City { get; set; }

        [MaxLength(40)]
        public string State { get; set; }

        [MaxLength(40)]
        public string Country { get; set; }

        [MaxLength(10)]
        public string PostalCode { get; set; }

        [MaxLength(24)]
        public string Phone { get; set; }

        [MaxLength(24)]
        public string Fax { get; set; }

        [MaxLength(60)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int SupportRepId { get; set; }
    }
}