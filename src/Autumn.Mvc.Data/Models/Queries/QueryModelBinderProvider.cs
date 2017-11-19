using System.Linq.Expressions;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Queries
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        private readonly AutumnSettings _autumnSettings;
         
        public QueryModelBinderProvider(AutumnSettings autumnSettings)
        {
            _autumnSettings = autumnSettings;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Expression<>)) return null;
            return CommonHelper.GetExpressionModelBinder(context.Metadata.ModelType, _autumnSettings);
        }
    }
}