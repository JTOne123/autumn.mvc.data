using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models
{
    public class ErrorModelBadRequest
    {
        public List<string> Messages { get; set; }

        public ErrorModelBadRequest()
        {

        }

        public ErrorModelBadRequest(ModelStateDictionary modelState)
        {
            Messages = new List<string>();
            if (modelState == null) throw new ArgumentNullException(nameof(modelState));
            foreach (var item in modelState.Values)
            foreach (var error in item.Errors)
                Messages.Add(error.ErrorMessage);
        }
    }
}