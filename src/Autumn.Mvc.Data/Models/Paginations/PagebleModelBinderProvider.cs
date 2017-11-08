using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Models.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class PageableModelBinderProvider : IModelBinderProvider
    {
        private readonly AutumnSettings _autumnSettings;

        public PageableModelBinderProvider(AutumnSettings autumnSettings)
        {
            _autumnSettings = autumnSettings;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Pageable<>)) return null;
            return CommonHelper.GetPageableModelBinder(context.Metadata.ModelType, _autumnSettings);
        }
    }
}