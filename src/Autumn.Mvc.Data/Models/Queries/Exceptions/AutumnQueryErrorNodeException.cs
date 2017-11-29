using System;
using Antlr4.Runtime.Tree;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class AutumnQueryErrorNodeException : AutumnQueryException<IErrorNode>
    {
        public AutumnQueryErrorNodeException(IErrorNode origin,
            Exception innerException = null) : base(origin, string.Format("Error parsing : {0}",origin.ToStringTree()), innerException)
        {
        }
    }
}