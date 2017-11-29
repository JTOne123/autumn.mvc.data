using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Middlewares.Models
{
    [AutumnEntity(Insertable = false)]
    public class AutumnIgnoreOperationMiddlewareTestPost
    {
        [AutumnKey]
        public int Test { get; set; }
    }
}