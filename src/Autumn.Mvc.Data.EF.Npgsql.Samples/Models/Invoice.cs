using System;
using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Npgsql.Samples.Models
{
    [Entity(Version = "v2")]
    public class Invoice : AbstractEntity
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [MaxLength(70)]
        public string BillingAddress { get; set; }

        [MaxLength(40)]
        public string BillingCity { get; set; }

        [MaxLength(40)]
        public string BillingState { get; set; }

        [MaxLength(40)]
        public string BillingCountry { get; set; }

        [MaxLength(10)]
        public string BillingPostalCode { get; set; }

        [Required]
        [Range(0, 99999999999.99)]
        public decimal Total { get; set; }
    }
}