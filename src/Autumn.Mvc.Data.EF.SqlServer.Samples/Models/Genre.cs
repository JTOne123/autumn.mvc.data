using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.SqlServer.Samples.Models
{
    [AutumnEntity]
    public class Genre :AbstractEntity
    {
        public string Name { get; set; }
    }
}