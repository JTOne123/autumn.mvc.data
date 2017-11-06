using System;
using System.Globalization;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Mvc.Models.Queries
{
    public class ComparisonTest
    {

        protected string GetRandom<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return System.IO.Path.GetRandomFileName().Replace("'", "");
            }
            if (!typeof(T).IsClass)
            {
                return Convert.ToString(default(T), CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        protected class Exemple
        {
            public string StringExemple { get; set; }
            public DateTime? NullableDateTimeExemple { get; set; }
            public DateTime DateTimeExemple { get; set; }
            public int Int32Exemple { get; set; }
            public int? NullableInt32Exemple { get; set; }
            public short Int16Exemple { get; set; }
            public short? NullableInt16Exemple { get; set; }
            public long Int64Exemple { get; set; }
            public long? NullableInt64Exemple { get; set; }
            public float SingleExemple { get; set; }
            public float? NullableSingleExemple { get; set; }
            public double DoubleExemple { get; set; }
            public double? NullableDoubleExemple { get; set; }
            public decimal DecimalExemple { get; set; }
            public decimal? NullableDecimalExemple { get; set; }
        }
        
        
        protected static Expression<Func<Exemple, bool>> Parse(string query, NamingStrategy namingStrategy=null)
        {
            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new RsqlLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new RsqlParser(commonTokenStream);
            var eval = parser.eval();
            var visitor = new DefaultRsqlVisitor<Exemple>(namingStrategy);
            return visitor.VisitEval(eval);
        }
        
        
      
    }
}