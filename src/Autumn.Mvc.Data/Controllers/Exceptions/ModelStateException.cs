using Autumn.Mvc.Data.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    public class ModelStateException : RepositoryControllerAsyncException
    {
        public ModelStateException(ModelStateDictionary stateDictionary) : base(stateDictionary.ToMessage())
        {
        }
    }
}