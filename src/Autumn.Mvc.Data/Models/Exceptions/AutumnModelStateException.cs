using Autumn.Mvc.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Exceptions
{
    public class AutumnModelStateException : AutumnException
    {
        public AutumnModelStateException(ModelStateDictionary stateDictionary) : base(stateDictionary.ToMessage())
        {
        }
    }
}