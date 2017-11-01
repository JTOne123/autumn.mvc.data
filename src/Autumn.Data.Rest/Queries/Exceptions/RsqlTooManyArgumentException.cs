namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlTooManyArgumentException : RsqlArgumentException
    {
        public RsqlTooManyArgumentException(RsqlParser.ArgumentsContext origin) : base(origin, "Too many arguments")
        {
        }
    }
}