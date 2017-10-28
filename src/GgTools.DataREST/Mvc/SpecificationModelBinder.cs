using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using GgTools.DataREST.Commons;
using GgTools.DataREST.Rsql;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;

namespace GgTools.DataREST.Mvc
{
    public class SpecificationModelBinder<T> : IModelBinder
    {
        private static readonly MemoryCache Queries = new MemoryCache(new MemoryCacheOptions(){ExpirationScanFrequency = TimeSpan.FromMinutes(5)});
        
        /// <summary>
        /// bind 
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            if (queryCollection.TryGetValue("query", out var query))
            {
                var hash = Hash(query[0]);
                if (!Queries.TryGetValue(hash, out var specification))
                {
                    specification = Build(query[0]);
                    Queries.Set(hash, specification);
                }
                bindingContext.Result = ModelBindingResult.Success(specification);
            }
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// create hash 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static string Hash(string query)
        {
            using (var md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}?{1}", typeof(T).FullName, query)));
                var hash = md5.Hash;
                var builder=new StringBuilder();
                foreach (var t in hash)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// build specification from rsql query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static ISpecification<T> Build(string query)
        {
            // parse query by antlr
            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new RsqlLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new RsqlParser(commonTokenStream);
            var context = parser.eval();
            return new Specification<T>();
        }
    }
}        