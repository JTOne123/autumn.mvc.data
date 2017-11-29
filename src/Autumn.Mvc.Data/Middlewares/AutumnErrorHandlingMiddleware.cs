using System;
using System.Net;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Middlewares
{
    public class AutumnErrorHandlingMiddleware
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings;
        private readonly RequestDelegate _next;

        public AutumnErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        static AutumnErrorHandlingMiddleware()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver =
                    new DefaultContractResolver() { NamingStrategy = AutumnApplication.Current.NamingStrategy }
            };
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new AutumnErrorModel() {Message = exception.Message, StackTrace = exception.StackTrace};
            if (exception is AutumnQueryComparisonException comparisonException)
            {
                result.Origin = comparisonException.Origin.GetText();
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result, JsonSerializerSettings));
        }
    }
}