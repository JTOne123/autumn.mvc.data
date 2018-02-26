using System;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models
{
    public class CountOnlyModelBinderProvider : IModelBinderProvider
    {
        private readonly AutumnDataSettings _autumnDataSettings;

        public CountOnlyModelBinderProvider(AutumnDataSettings autumnDataSettings)
        {
            _autumnDataSettings = autumnDataSettings ?? throw new ArgumentNullException(nameof(autumnDataSettings));
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return new CountOnlyModelBinder(_autumnDataSettings);
        }
    }
}