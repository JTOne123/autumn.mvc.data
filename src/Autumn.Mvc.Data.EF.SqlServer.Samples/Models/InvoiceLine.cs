using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    [Entity(Version = "v2")]
    public class InvoiceLine : AbstractEntity
    {
        /// <summary>
        /// invoice Id 
        /// </summary>
        public int InvoiceId { get; set; }
        public int TrackId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}