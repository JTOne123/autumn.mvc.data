using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Models.Queries
{
    public class IsNullComparisonOperatorTest : ComparisonTest
    {
        
        /// <summary>
        /// test : StringExemple==
        /// </summary>
        [Fact]
        public void NotEnougthArgumentExceptionTest()
        {
            var expected = new List<string>();
            foreach (var p in typeof(Exemple).GetProperties().Where(p=>
                (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition()==typeof(Nullable<>))
                || p.PropertyType==typeof(string)))
            {
                expected.Add(p.Name + "=is-null=");
                expected.Add(p.Name + "=nil=");
            }

            foreach (var item in expected)
            {
                Assert.Throws<AutumnQueryComparisonNotEnoughtArgumentException>(() => { Parse(item); });
            }
        }

        
        /// <summary>
        /// test : StringExemple==...
        /// test : StringExemple=eq=...'
        /// </summary>
        [Fact]
        public void StringTest()
        {
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, "StringExemple", null);


            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(null, typeof(object))), parameter);

            var expected = Parse("StringExemple=is-null=" + true.ToString());
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse("StringExemple=nil=" + true.ToString());
            Assert.Equal(actual.ToString(), expected.ToString());

        }

        private static bool IsNull<T>(T value, string valueToString=null)
        {
            var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments()[0] : typeof(T);
            var propertyName = (typeof(T).IsGenericType ? "Nullable" : "") + type.Name + "Exemple";
            var v = valueToString ?? Convert.ToString(value,CultureInfo.InvariantCulture);
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, propertyName, null);

            var actual = Expression.Lambda<Func<Exemple, bool>>( Expression.Equal(
                memberExpression.Expression,
                Expression.Constant(value, typeof(T))), parameter);

            var expected = Parse(propertyName + "=is-null=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse(propertyName + "=nil=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());
            return true;
        }
    }
}