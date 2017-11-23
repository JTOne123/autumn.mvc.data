using System.Collections.Generic;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Http;

namespace Autumn.Mvc.Data.Middlewares
{
    public class IgnoreOperationMiddleware
    {
        private readonly RequestDelegate _next;
        private static Dictionary<string, List<string>> _exclusions;
      
        public IgnoreOperationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                if (_exclusions[method].Contains(Path(method, path)))
                {
                    context.Response.StatusCode = 404;
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
        
        static IgnoreOperationMiddleware()
        {
            _exclusions = new Dictionary<string, List<string>>
            {
                {"POST", new List<string>()},
                {"PUT", new List<string>()},
                {"DELETE", new List<string>()}
            };
            foreach (var item in AutumnSettings.Current.IgnoresPaths)
            {
                var ignore = ((int) item.Value).ToString().PadLeft(3, '0');
                if (ignore[0] == '1')
                    _exclusions["POST"].Add(item.Key);
                if (ignore[1] == '1')
                    _exclusions["PUT"].Add(item.Key);
                if (ignore[2] == '1')
                    _exclusions["DELETE"].Add(item.Key);
            }
        }
    }
}