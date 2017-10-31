namespace Autumn.Data.Rest.Rsql.Exceptions
{
    public class RsqlTooManyArgumentException : RsqlArgumentException
    {
        public RsqlTooManyArgumentException(RsqlParser.ArgumentsContext origin) : base(origin, "Too many arguments")
        {
        }
    }
}