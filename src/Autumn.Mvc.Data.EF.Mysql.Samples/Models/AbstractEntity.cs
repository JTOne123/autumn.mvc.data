using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    public abstract class AbstractEntity
    {
        [AutumnKey()]
        [AutumnIgnoreOperationProperty(AutumnIgnoreOperationPropertyType.Update |AutumnIgnoreOperationPropertyType.Insert)]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
    }

    public abstract class AbstractEntityWithAddress : AbstractEntity
    {
        
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

    }

}