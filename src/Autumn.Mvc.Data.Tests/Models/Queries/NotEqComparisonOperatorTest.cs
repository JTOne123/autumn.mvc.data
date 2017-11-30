using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Models.Queries
{
    public class NotEqComparisonOperatorTest : ComparisonTest
    {

        /// <summary>
        /// test : StringExemple!=
        /// </summary>
        [Fact]
        public void NotEnougthArgumentExceptionTest()
        {
            var expected = new List<string>();
            foreach (var p in typeof(Exemple).GetProperties())
            {
                expected.Add(p.Name + "!=");
                expected.Add(p.Name + "=neq=");
            }

            foreach (var item in expected)
            {
                Assert.Throws<AutumnQueryComparisonNotEnoughtArgumentException>(() => { Parse(item); });
            }
        }

        /// <summary>
        /// test : StringExemple!=('a','b')
        /// test : StringExemple=neq=('a','b')
        /// </summary>
        [Fact]
        public void NotEnougthArgumentTest()
        {
            Assert.Throws<AutumnQueryComparisonTooManyArgumentException>(() => { Parse("StringExemple!=('a','b')"); });
            Assert.Throws<AutumnQueryComparisonTooManyArgumentException>(() => { Parse("StringExemple=neq=('a','b')"); });
        }

        /// <summary>
        /// test : StringExemple!=...
        /// test : StringExemple=neq=...'
        /// </summary>
        [Fact]
        public void StringTest()
        {
            var parameter = Expression.Parameter(typeof(Exemple));
            var argument = GetRandom<string>();
            var memberExpression = AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, "StringExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(argument)), parameter);

            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Not(actual.Body), parameter);

            var expected = Parse("StringExemple!="+argument);
            Assert.Equal(actual.ToString(), expected.ToString());


            expected = Parse("StringExemple=neq="+argument);
            Assert.Equal(actual.ToString(), expected.ToString());

        }

        /// <summary>
        /// test : DateTimeNullExemple!='1973-08-19'
        /// test : DateTimeNullExemple=neq='1973-08-19'
        /// test : DateTimeNullExemple!='1973-08-19T23:59:59'
        /// test : DateTimeNullExemple=neq='1973-08-19T23:59:59'
        /// </summary>
        [Fact]
        public void NullableDatetimeTest()
        {
            #region Datetime? & yyyy-MM-dd
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, "NullableDateTimeExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19), typeof(DateTime?))), parameter);

            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Not(actual.Body), parameter);
            
            var expected = Parse("NullableDateTimeExemple!=1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("NullableDateTimeExemple=neq=1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
            
            #region DateTime? & yyyy-MM-ddThh:mm:ss
            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19, 23, 59, 59), typeof(DateTime?))), parameter);
            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Not(actual.Body), parameter);
          

            expected = Parse("NullableDateTimeExemple!=1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("NullableDateTimeExemple=neq=1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
        }
        
        /// <summary>
        /// test : DateTimeExemple!='1973-08-19'
        /// test : DateTimeExemple=neq='1973-08-19'
        /// test : DateTimeExemple!='1973-08-19T23:59:59'
        /// test : DateTimeExemple=neq='1973-08-19T23:59:59'
        /// </summary>
        [Fact]
        public void DatetimeTest()
        {
            #region Datetime & yyyy-MM-dd
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, "DateTimeExemple", null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19), typeof(DateTime))), parameter);

            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Not(actual.Body), parameter);
            
            var expected = Parse("DateTimeExemple!=1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("DateTimeExemple=neq=1973-08-19");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
            
            #region DateTime & yyyy-MM-ddThh:mm:ss
            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(new DateTime(1973, 8, 19, 23, 59, 59), typeof(DateTime))), parameter);
            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Not(actual.Body), parameter);
          

            expected = Parse("DateTimeExemple!=1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("DateTimeExemple=neq=1973-08-19T23:59:59");
            Assert.Equal(actual.ToString(), expected.ToString());
            #endregion
        }
        
        /// <summary>
        /// test : NullableInt16Exemple!=1
        /// test : NullableInt16Exemple=neq=1
        /// </summary>
        [Fact]
        public void NullableInt16Test()
        {
            Assert.True(Eq<short?>(1));
        }

        /// <summary>
        /// test : Int16Exemple!=1
        /// test : Int16Exemple=neq=1
        /// </summary>
        [Fact]
        public void Int16Test()
        {
            Assert.True(Eq<short>(1));
        }
        
        /// <summary>
        /// test : NullableInt32Exemple!=1
        /// test : NullableInt32Exemple=neq=1
        /// </summary>
        [Fact]
        public void NullableInt32Test()
        {
            Assert.True(Eq<int?>(1));
        }

        /// <summary>
        /// test : Int16Exemple!=1
        /// test : Int16Exemple=neq=1
        /// </summary>
        [Fact]
        public void Int32Test()
        {
            Assert.True(Eq<int>(1));
        }

        /// <summary>
        /// test : NullableInt64Exemple!=1
        /// test : NullableInt64Exemple=neq=1
        /// </summary>
        [Fact]
        public void NullableInt64Test()
        {
            Assert.True(Eq<long?>(1));
        }

        /// <summary>
        /// test : Int16Exemple!=1
        /// test : Int16Exemple=neq=1
        /// </summary>
        [Fact]
        public void Int64Test()
        {
            Assert.True(Eq<long>(1));
        }
        
        
        /// <summary>
        /// test : NullableSingleExemple!=...
        /// test : NullableSingleExemple=neq=...
        /// </summary>
        [Fact]
        public void NullableSingleTest()
        {
            Assert.True(Eq<float?>((float)1.5));
        }

        /// <summary>
        /// test : SingleExemple!=...
        /// test : SingleExemple=neq=...
        /// </summary>
        [Fact]
        public void SingleTest()
        {
            Assert.True(Eq<float>((float)1.5));
        }
        
        /// <summary>
        /// test : NullableDoubleExemple!=...
        /// test : NullableDoubleExemple=neq=...
        /// </summary>
        [Fact]
        public void NullableDoubleTest()
        {
            Assert.True(Eq<double?>((double)1.5));
        }

        /// <summary>
        /// test : DoubleExemple!=...
        /// test : DoubleExemple=neq=...
        /// </summary>
        [Fact]
        public void DoubleTest()
        {
            Assert.True(Eq<double>((double)1.5));
        }
        
        
        /// <summary>
        /// test : NullableDecimalExemple!=...
        /// test : NullableDecimalExemple=neq=...
        /// </summary>
        [Fact]
        public void NullableDecimalTest()
        {
            Assert.True(Eq<decimal?>((decimal)1.5));
        }

        /// <summary>
        /// test : DecimalExemple!=...
        /// test : DecimalExemple=neq=...
        /// </summary>
        [Fact]
        public void DecimalTest()
        {
            Assert.True(Eq<decimal>((decimal)1.5));
        }
        
        private static bool Eq<T>(T value, string valueToString=null)
        {
            var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments()[0] : typeof(T);
            var propertyName = (typeof(T).IsGenericType ? "Nullable" : "") + type.Name + "Exemple";
            var v = valueToString ?? Convert.ToString(value,CultureInfo.InvariantCulture);
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, propertyName, null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(value, typeof(T))), parameter);

            actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Not(actual.Body), parameter);

            var expected = Parse(propertyName + "!=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse(propertyName + "=neq=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());
            return true;
        }

     
    }
}