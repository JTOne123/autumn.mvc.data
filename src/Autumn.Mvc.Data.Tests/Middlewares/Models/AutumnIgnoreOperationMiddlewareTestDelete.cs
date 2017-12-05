using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Middlewares.Models
{
    [Entity(Deletable = false)]
    public class AutumnIgnoreOperationMiddlewareTestDelete
    {
        [Key]
        public int Test { get; set; }
    }
}