using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using Antlr4.Runtime;
using Autumn.Data.Rest.Helpers;
using Autumn.Data.Rest.Queries;
using Autumn.Data.Rest.Queries.Exceptions;
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
            public int Int32Exemple { get; set; }
            public int? NullableInt32Exemple { get; set; }
            public short Int16Exemple { get; set; }
            public short? NullableInt16Exemple { get; set; }
            public long Int64Exemple { get; set; }
            public long? NullableInt64Exemple { get; set; }
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
        /// test : StringExemple==
        /// </summary>
        [Fact]
        public void EqNotEnougthArgumentExceptionTest()
        {
            Assert.Throws<RsqlNotEnoughtArgumentException>(() => { Parse("StringExemple=="); });
        }

        /// <summary>
        /// test : StringExemple==('a','b')
        /// </summary>
        [Fact]
        public void EqNotEnougthArgumentTest()
        {
            Assert.Throws<RsqlTooManyArgumentException>(() => { Parse("StringExemple==('a','b')"); });
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
        /// test : NullableInt16Exemple==1
        /// test : NullableInt16Exemple=eq=1
        /// </summary>
        [Fact]
        public void EqNullableInt16Test()
        {
            Assert.True(EqNullableTest<short?>(1));
        }

        /// <summary>
        /// test : Int16Exemple==1
        /// test : Int16Exemple=eq=1
        /// </summary>
        [Fact]
        public void EqInt16Test()
        {
            Assert.True(EqNullableTest<short>(1));
        }
        
        /// <summary>
        /// test : NullableInt32Exemple==1
        /// test : NullableInt32Exemple=eq=1
        /// </summary>
        [Fact]
        public void EqNullableInt32Test()
        {
            Assert.True(EqNullableTest<int?>(1));
        }

        /// <summary>
        /// test : Int16Exemple==1
        /// test : Int16Exemple=eq=1
        /// </summary>
        [Fact]
        public void EqInt32Test()
        {
            Assert.True(EqNullableTest(1));
        }


        /// <summary>
        /// test : NullableInt64Exemple==1
        /// test : NullableInt64Exemple=eq=1
        /// </summary>
        [Fact]
        public void EqNullableInt64Test()
        {
            Assert.True(EqNullableTest<long?>(1));
        }

        /// <summary>
        /// test : Int16Exemple==1
        /// test : Int16Exemple=eq=1
        /// </summary>
        [Fact]
        public void EqInt64Test()
        {
            Assert.True(EqNullableTest<long>(1));
        }

        private bool EqNullableTest<T>(T value, string valueToString=null)
        {
            var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments()[0] : typeof(T);
            var propertyName = (typeof(T).IsGenericType ? "Nullable" : "") + type.Name + "Exemple";
            var v = valueToString ?? value.ToString();
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                CommonHelper.GetMemberExpressionValue<Exemple>(parameter, propertyName, null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(value, typeof(T))), parameter);

            var expected = Parse(propertyName + "==" + v);
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse(propertyName + "=eq=1" + valueToString);
            Assert.Equal(actual.ToString(), expected.ToString());
            return true;
        }

     
    }
}