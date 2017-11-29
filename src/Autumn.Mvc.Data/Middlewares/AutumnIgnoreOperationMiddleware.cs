using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Autumn.Mvc.Data.Middlewares
{
    public class AutumnIgnoreOperationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, List<string>> Exclusions;
      
        public AutumnIgnoreOperationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                if (Exclusions[method].Contains(Path(method, path)))
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

        static string Path(string method, string path)
        {
            if ( method == "PUT" || method == "DELETE")
            {
                return path.Substring(0, path.LastIndexOf('/'));
            }
            return path;
        }
        
        static AutumnIgnoreOperationMiddleware()
        {
            Exclusions = new Dictionary<string, List<string>>
            {
                {"POST", new List<string>()},
                {"PUT", new List<string>()},
                {"DELETE", new List<string>()}
            };
            foreach (var item in AutumnApplication.Current.IgnoreOperations)
            {
                if (item.Value.Contains(HttpMethod.Post))
                    Exclusions["POST"].Add(item.Key);
                if (item.Value.Contains(HttpMethod.Put))
                    Exclusions["PUT"].Add(item.Key);
                if (item.Value.Contains(HttpMethod.Delete))
                    Exclusions["DELETE"].Add(item.Key);
            }
        }
    }
}