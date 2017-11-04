using System;
using System.Net;
using System.Threading.Tasks;
using Autumn.Data.Rest.Queries.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Autumn.Data.Rest.Middlewares
{
    public class ErrorHandlingMiddleware
    {
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            object o = null;
            if (exception is RsqlComparisonException comparisonException)
            {
                o = new {error = exception.Message, comparison= comparisonException.Origin.GetText()};
            }
            else
            {
                o = new {error = exception.Message};
            }
            var result = JsonConvert.SerializeObject(o);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}