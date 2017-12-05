using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.SqlServer.Samples.Models
{
    public abstract class AbstractEntity
    {
        [Annotations.Key()]
        [IgnoreOperationProperty()]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
    }
}