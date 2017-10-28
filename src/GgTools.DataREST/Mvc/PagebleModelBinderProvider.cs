using GgTools.DataREST.Commons;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GgTools.DataREST.Mvc
{
    public class PageableModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(IPageable) ? new PageableModelBinder() : null;
        }
    }
}