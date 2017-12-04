using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Exceptions
{
    public class AutumnModelStateException : AutumnException
    {
        public AutumnModelStateException(ModelStateDictionary stateDictionary) : base(ToMessage(stateDictionary))
        {
        }
        
        private static string ToMessage(ModelStateDictionary stateDictionary){
        
            var stringbuilder = new StringBuilder();
            foreach (var item in stateDictionary.Values)
            foreach (var error in item.Errors)
                stringbuilder.Append(error.ErrorMessage);
            return stringbuilder.ToString();
        }
    }
}