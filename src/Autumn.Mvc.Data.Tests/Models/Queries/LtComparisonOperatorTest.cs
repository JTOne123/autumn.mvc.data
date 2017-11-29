using System;
using System.Globalization;
using System.Linq.Expressions;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Models.Queries
{
    public class LtComparisonOperatorTest : ComparisonTest
    {


        /// <summary>
        /// test : Int32Exemple < ...
        /// test : Int32Exemple =lt= ...
        /// </summary>
        [Fact]
        public void LtNotEnougthArgumentExceptionTest()
        {
            Assert.Throws<AutumnQueryComparisonNotEnoughtArgumentException>(() => { Parse("Int32Exemple<"); });
            Assert.Throws<AutumnQueryComparisonNotEnoughtArgumentException>(() => { Parse("Int32Exemple=lt="); });
        }

        /// <summary>
        /// test : Int32Exemple<(... , ...)
        /// test : Int32Exemple=lt=(... , ...)
        /// </summary>
        [Fact]
        public void LtTooManyArgumentExceptionTest()
        {
            Assert.Throws<AutumnQueryComparisonTooManyArgumentException>(() => { Parse("Int32Exemple<(1,2)"); });
            Assert.Throws<AutumnQueryComparisonTooManyArgumentException>(() => { Parse("Int32Exemple=lt=(1,2)"); });
        }

        /// <summary>
        /// test : StringExemple<'...'
        /// test : StringExemple=lt='...'
        /// test : BooleanExemple<...
        /// test : BooleanExemple=lt=...
        /// test : NullableBooleanExemple<...
        /// test : NullableBooleanExemple=lt=...
        /// </summary>
        [Fact]
        public void LtInvalidComparatorSelectionExceptionTest()
        {
            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                 Parse("StringExemple<'"+GetRandom<string>()+"'");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("StringExemple=lt="+GetRandom<string>()+"'");
            });
            
            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("BooleanExemple<true");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("BooleanExemple=lt=true");
            });
            
            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("NullableBooleanExemple<true");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("NullableBooleanExemple=lt=true");
            });
        }

     
        private static bool Lt<T>(T value, string valueToString=null)
        {
            var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments()[0] : typeof(T);
            var propertyName = (typeof(T).IsGenericType ? "Nullable" : "") + type.Name + "Exemple";
            var v = valueToString ?? Convert.ToString(value,CultureInfo.InvariantCulture);
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, propertyName, null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.LessThan(
                memberExpression.Expression,
                Expression.Constant(value, typeof(T))), parameter);

            var expected = Parse(propertyName + "<" + v);
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse(propertyName + "=lt=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());
            return true;
        }

     
    }
}