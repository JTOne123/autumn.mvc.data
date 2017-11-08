using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Autumn.Mvc.Data
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
            if (actionDescriptor.ActionName != "Find") return;
            
            operation.Produces.Add("application/json");
            
            operation.Parameters.Clear();
            var parameter = new NonBodyParameter
            {
                Type = "string",
                In = AutumnSettings.Instance.QueryFieldName,
                Name = AutumnSettings.Instance.QueryFieldName
            };
            operation.Parameters.Add(parameter);
            parameter = new NonBodyParameter
            {
                In = AutumnSettings.Instance.PageSizeFieldName,
                Type = "integer",
                Name = AutumnSettings.Instance.PageSizeFieldName
            };
            operation.Parameters.Add(parameter);
            parameter = new NonBodyParameter
            {
                In = AutumnSettings.Instance.PageNumberFieldName,
                Type = "integer",
                Name = AutumnSettings.Instance.PageNumberFieldName
            };
            operation.Parameters.Add(parameter);
        }
    }
}