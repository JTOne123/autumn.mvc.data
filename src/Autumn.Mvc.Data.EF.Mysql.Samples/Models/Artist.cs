using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    /// <summary>
    /// Entity class for Artist
    /// </summary>
    [Entity( Version = "v1")]
    public class Artist : AbstractEntity
    {
        /// <summary>
        /// Name of artist
        /// </summary>
        [MinLength(4)]
        public string Name { get; set; }
    }
}