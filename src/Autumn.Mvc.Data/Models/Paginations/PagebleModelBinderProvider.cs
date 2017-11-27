using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class PageableModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Pageable<>)) return null;
            return CommonHelper.GetPageableModelBinder(context.Metadata.ModelType);
        }
    }
}