using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Middlewares.Models
{
    [AutumnEntity(Updatable = false)]
    public class AutumnIgnoreOperationMiddlewareTestPut
    {
        [AutumnKey]
        public int Test { get; set; }
    }
}