namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlNotEnoughtArgumentException : RsqlArgumentException
    {
        public RsqlNotEnoughtArgumentException(RsqlParser.ArgumentsContext origin) : base(origin, "Not enought argument")
        {
        }
    }
}