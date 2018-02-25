using System;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models
{
    public class OnlyCountModelBinder  : IModelBinder 
    {
        private readonly AutumnDataSettings _autumnDataSettings;
        
        public OnlyCountModelBinder(AutumnDataSettings autumnDataSettings)
        {
            _autumnDataSettings = autumnDataSettings ?? throw new ArgumentNullException(nameof(autumnDataSettings));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
                if (queryCollection.TryGetValue(_autumnDataSettings.OnlyCountField, out var onlyCountString))
                {
                    if (bool.TryParse(onlyCountString, out var onlyCount))
                    {
                        bindingContext.Result = ModelBindingResult.Success(onlyCount);
                    }
                }
            }
            catch (Exception e)
            {
                bindingContext.ModelState.AddModelError(GetType().FullName, e.Message);
            }

            return Task.CompletedTask;
        }
    }
}