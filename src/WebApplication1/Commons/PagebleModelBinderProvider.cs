using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication1.Commons
{
    public class PageableModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(IPageable))
            {
                return new PageableModelBinder();
            }
            return null;
        }
    }
}