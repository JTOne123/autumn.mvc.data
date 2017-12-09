using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Http;

namespace Autumn.Mvc.Data.Middlewares
{
    public class IgnoreOperationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AutumnSettings _settings;

        public IgnoreOperationMiddleware(RequestDelegate next, AutumnSettings settings)
        {
            _next = next;
            _settings = settings;
        }

        private static HttpMethod Method(string method)
        {
            if (method.ToUpperInvariant() == "GET")
                return HttpMethod.Get;
            else if (method.ToUpperInvariant() == "POST")
            {
                return HttpMethod.Post;
            }
            else if (method.ToUpperInvariant() == "PUT")
            {
                return HttpMethod.Put;
            }
            else if (method.ToUpperInvariant() == "OPTIONS")
            {
                return HttpMethod.Options;
            }
            else if (method.ToUpperInvariant() == "HEAD")
            {
                return HttpMethod.Head;
            }
            else if (method.ToUpperInvariant() == "DELETE")
            {
                return HttpMethod.Delete;
            }
            else if (method.ToUpperInvariant() == "TRACE")
            {
                return HttpMethod.Trace;
            }
            return null;
        }

        private static string Path(HttpMethod method, PathString path)
        {
            var result = path.ToString();
            if (method == HttpMethod.Put || method == HttpMethod.Delete)
            {
                return result.Substring(0, result.LastIndexOf('/'));
            }
            return result;
        }


        public async Task Invoke(HttpContext context)
        {
            var ignore = _settings.DataSettings().IgnoreOperations;
            var method = Method(context.Request.Method);
            var path = Path(method, context.Request.Path);
            if (ignore.ContainsKey(path))
            {
                if (ignore[path].Contains(method))
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}