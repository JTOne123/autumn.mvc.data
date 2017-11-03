using System;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Autumn.Data.Rest.Helpers;
using Autumn.Data.Rest.Queries;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Autumn.Data.Rest.Tests.Queries
{
    public class EqComparisonOperatorTest
    {

        private static readonly NamingStrategy _camelCase = new CamelCaseNamingStrategy();
        private static readonly NamingStrategy _snakeCase = new SnakeCaseNamingStrategy();
        
    
        private class Exemple
        {
            public string StringExemple { get; set; }
            public DateTime? NullableDateTimeExemple { get; set; }
            public DateTime DateTimeExemple { get; set; }
            public int IntExemple { get; set; }
            public int? NullableIntExemple { get; set; }
        }


        private static Expression<Func<Exemple, bool>> Parse(string query, NamingStrategy namingStrategy=null)
        {
            var antlrInputStream = new AntlrInputStream(query);
            var lexer = new RsqlLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new RsqlParser(commonTokenStream);
            var eval = parser.eval();
            var visitor = new DefaultRsqlVisitor<Exemple>(namingStrategy);
            return visitor.VisitEval(eval);
        }

        /// <summary>
        /// test : StringExemple=='test'
        /// test : StringExemple=eq='test'
        /// </summary>
        [Fact]
        public void EqStringTest()
        {
            var parameter = Expression.Parameter(typeof(Exemple));

            var memberExpression = CommonHelper.GetMemberExpressionValue<Exemple>(parameter, "StringExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant("test")), parameter);

            var expected = Parse("StringExemple=='test'");
            Assert.Equal(actual.ToString(), expected.ToString());


            expected = Parse("StringExemple=eq='test'");
            Assert.Equal(actual.ToString(), expected.ToString());

        }

        /// <summary>
        /// test : DateTimeNullExemple=='1973-08-19'
        /// test : DateTimeNullExemple=eq='1973-08-19'
        /// test : DateTimeNullExemple=='1973-08-19T23:59:59'
        /// test : DateTimeNullExemple=eq='1973-08-19T23:59:59'
        /// </summary>
        [Fact]
        public void EqNullableDatetimeTest()
        {
            #region Datetime? & yyyy-MM-dd
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                CommonHelper.GetMemberExpressionValue<Exemple>(parameter, "NullableDateTimeExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19), typeof(DateTime?))), parameter);

            var expected = Parse("NullableDateTimeExemple==1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("NullableDateTimeExemple=eq=1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
            
            #region DateTime? & yyyy-MM-ddThh:mm:ss
            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19, 23, 59, 59), typeof(DateTime?))), parameter);


            expected = Parse("NullableDateTimeExemple==1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("NullableDateTimeExemple=eq=1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
        }
        
        /// <summary>
        /// test : DateTimeExemple=='1973-08-19'
        /// test : DateTimeExemple=eq='1973-08-19'
        /// test : DateTimeExemple=='1973-08-19T23:59:59'
        /// test : DateTimeExemple=eq='1973-08-19T23:59:59'
        /// </summary>
        [Fact]
        public void EqDatetimeTest()
        {
            #region Datetime & yyyy-MM-dd
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                CommonHelper.GetMemberExpressionValue<Exemple>(parameter, "DateTimeExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19), typeof(DateTime))), parameter);

            var expected = Parse("DateTimeExemple==1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("DateTimeExemple=eq=1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
            
            #region DateTime & yyyy-MM-ddThh:mm:ss
            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19, 23, 59, 59), typeof(DateTime))), parameter);


            expected = Parse("DateTimeExemple==1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("DateTimeExemple=eq=1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
        }

        /// <summary>
        /// test : NullableIntExemple==1
        /// test : NullableIntExemple=eq=1
        /// </summary>
        [Fact]
        public void EqNullableIntTest()
        {
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                CommonHelper.GetMemberExpressionValue<Exemple>(parameter, "NullableIntExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(1, typeof(int?))), parameter);

            var expected = Parse("NullableIntExemple==1");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("NullableIntExemple=eq=1");
            Assert.Equal(actual.ToString(), expected.ToString());
        }

        /// <summary>
        /// test : IntExemple==1
        /// test : IntExemple=eq=1
        /// </summary>
        [Fact]
        public void EqIntTest()
        {

            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                CommonHelper.GetMemberExpressionValue<Exemple>(parameter, "IntExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(1, typeof(int))), parameter);

            var expected = Parse("IntExemple==1");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("IntExemple=eq=1");
            Assert.Equal(actual.ToString(), expected.ToString());
        }

    }
}