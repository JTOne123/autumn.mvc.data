using System;
using Antlr4.Runtime.Tree;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class QueryErrorNodeException : QueryException<IErrorNode>
    {
        public QueryErrorNodeException(IErrorNode origin,
            Exception innerException = null) : base(origin, string.Format("Error parsing : {0}",origin.ToStringTree()), innerException)
        {
        }
    }
}