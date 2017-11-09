using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;    
namespace Autumn.Mvc.Data.Swagger
{
    public class AutumnOperationFilter : IOperationFilter

    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;
            if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)) return;
            if (!actionDescriptor.ControllerTypeInfo.IsGenericType &&
                actionDescriptor.ControllerTypeInfo.GetGenericTypeDefinition() !=
                typeof(RepositoryControllerAsync<,>)) return;

            operation.Consumes.Clear();
            if (new[] {"POST", "PUT", "PATCH"}.Contains(context.ApiDescription.HttpMethod))
            {
                operation.Consumes.Add("application/json");
            }
            
            if (actionDescriptor.ActionName != "Get") return;
            operation.Parameters.Clear();
            var parameter = new NonBodyParameter
            {
                Type = "string",
                In = "query",
                Name = AutumnSettings.Instance.QueryFieldName
            };
            operation.Parameters.Add(parameter);
            
            parameter = new NonBodyParameter
            {
                In = "query",
                Type = "integer",
                Minimum = 0,
                Format = "int32",
                Default = AutumnSettings.Instance.DefaultPageSize,    
                Name = AutumnSettings.Instance.PageSizeFieldName
            };
            operation.Parameters.Add(parameter);
            
            parameter = new NonBodyParameter
            {
                In = "query",
                Type = "integer",
                Minimum = 0,
                Format = "int32",
                Default = 0,
                Name = AutumnSettings.Instance.PageNumberFieldName
            };
            operation.Parameters.Add(parameter);
        }
    }
}