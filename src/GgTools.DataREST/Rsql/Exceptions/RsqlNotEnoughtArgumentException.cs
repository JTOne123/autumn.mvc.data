using System;

namespace GgTools.DataREST.Rsql.Exceptions
{
    public class RsqlNotEnoughtArgumentException : RsqlArgumentException
    {
        public RsqlNotEnoughtArgumentException(RsqlParser.ArgumentsContext origin) : base(origin, "Not enought argument")
        {
        }
    }
}