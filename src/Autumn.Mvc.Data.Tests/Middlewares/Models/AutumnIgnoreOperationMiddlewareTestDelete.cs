using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Middlewares.Models
{
    [AutumnEntity(Deletable = false)]
    public class AutumnIgnoreOperationMiddlewareTestDelete
    {
        [AutumnKey]
        public int Test { get; set; }
    }
}