using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Middlewares.Models
{
    [Entity(Insertable = false)]
    public class AutumnIgnoreOperationMiddlewareTestPost
    {
        [Key]
        public int Test { get; set; }
    }
}