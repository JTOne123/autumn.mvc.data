using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Queries
{
    public class AutumnQueryModelBinderProvider : IModelBinderProvider
    {
       
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Expression<>)) return null;
            return AutumnModelHelper.GetExpressionModelBinder(context.Metadata.ModelType);
        }
    }
}