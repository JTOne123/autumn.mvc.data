using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class AutumnPageableModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(AutumnPageable<>)) return null;
            return AutumnModelHelper.GetPageableModelBinder(context.Metadata.ModelType);
        }
    }
}