using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    [AutumnEntity]
    [AutumnIgnoreOperation(AutumnIgnoreOperationType.Insert | AutumnIgnoreOperationType.Update | AutumnIgnoreOperationType.Delete)]
    public class Genre :AbstractEntity
    {
        public string Name { get; set; }
    }
}