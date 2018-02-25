using System;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models
{
    public class OnlyCountModelBinderProvider : IModelBinderProvider
    {
        private readonly AutumnDataSettings _autumnDataSettings;

        public OnlyCountModelBinderProvider(AutumnDataSettings autumnDataSettings)
        {
            _autumnDataSettings = autumnDataSettings ?? throw new ArgumentNullException(nameof(autumnDataSettings));
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return new OnlyCountModelBinder(_autumnDataSettings);
        }
    }
}