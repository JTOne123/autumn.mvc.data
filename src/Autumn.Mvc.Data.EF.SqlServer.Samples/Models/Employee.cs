using System;
using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    [Resource]
    public class Employee : AbstractEntityWithAddress
    {
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        public int? ReportsTo { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime HireDate { get; set; }
    }
}