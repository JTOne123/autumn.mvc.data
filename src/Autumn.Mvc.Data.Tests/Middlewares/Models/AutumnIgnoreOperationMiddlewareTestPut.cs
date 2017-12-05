using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Middlewares.Models
{
    [Entity(Updatable = false)]
    public class AutumnIgnoreOperationMiddlewareTestPut
    {
        [Key]
        public int Test { get; set; }
    }
}