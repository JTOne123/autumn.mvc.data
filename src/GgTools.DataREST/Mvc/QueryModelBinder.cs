using System;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using GgTools.DataREST.Helpers;
using GgTools.DataREST.Rsql;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;

namespace GgTools.DataREST.Mvc
{
    public class QueryModelBinder<T> : IModelBinder
    {
        private readonly string _queryField;
        private readonly NamingStrategy _namingStrategy;

        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="queryField"></param>
        /// <param name="namingStrategy"></param>
        public QueryModelBinder(string queryField, NamingStrategy namingStrategy)
        {
            _queryField = queryField;
            _namingStrategy = namingStrategy;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            var eval = RsqlHelper.True<T>();
            if (queryCollection.TryGetValue(_queryField, out var query))
            {
                var hash = Hash(query[0]);
                if (!RsqlHelper.QueriesCache.TryGetValue(hash, out var specification))
                {
                    eval = Build(query[0]);
                    RsqlHelper.QueriesCache.Set(hash, specification);
                }
            }
            bindingContext.Result = ModelBindingResult.Success(eval);
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
        private Expression<Func<T, bool>> Build(string query)
        {
            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new RsqlLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new RsqlParser(commonTokenStream);
            var eval = parser.eval();
            var visitor = new DefaultRsqlVisitor<T>(_namingStrategy);
            return visitor.VisitEval(eval);
        }
    }
}        