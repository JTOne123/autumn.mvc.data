using System;
using System.Net;
using System.Threading.Tasks;
using Autumn.Data.Rest.Configurations;
using Autumn.Data.Rest.Queries.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context )
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

        static ErrorHandlingMiddleware()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver =
                    new DefaultContractResolver() {NamingStrategy = AutumnSettings.Instance.NamingStrategy}
            };
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            object o = null;
            if (exception is RsqlComparisonException comparisonException)
            {
                o = new
                {
                    Error = exception.Message,
                    Timestamp = DateTime.UtcNow,
                    Comparison = comparisonException.Origin.GetText()
                };
            }
            else
            {
                o = new
                {
                    Error = exception.Message,
                    Timestamp = DateTime.UtcNow
                };
            }
            var result = JsonConvert.SerializeObject(o, _jsonSerializerSettings);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}