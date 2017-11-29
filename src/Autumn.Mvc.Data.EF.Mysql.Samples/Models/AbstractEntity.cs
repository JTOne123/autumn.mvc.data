using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    /// <summary>
    /// abstract class Entity 
    /// </summary>
    public abstract class AbstractEntity
    {
        /// <summary>
        /// Id of Entity
        /// </summary>
        [AutumnKey()]
        [AutumnIgnoreOperationProperty()]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
    }
}