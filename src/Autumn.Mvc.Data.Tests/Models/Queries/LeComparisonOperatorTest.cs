using System;
using System.Globalization;
using System.Linq.Expressions;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Models.Queries
{
    public class LeComparisonOperatorTest : ComparisonTest
    {

        /// <summary>
        /// test : Int32Exemple < ...
        /// test : Int32Exemple =lt= ...
        /// </summary>
        [Fact]
        public void NotEnougthArgumentExceptionTest()
        {
            Assert.Throws<AutumnQueryComparisonNotEnoughtArgumentException>(() => { Parse("Int32Exemple<="); });
            Assert.Throws<AutumnQueryComparisonNotEnoughtArgumentException>(() => { Parse("Int32Exemple=le="); });
        }

        /// <summary>
        /// test : Int32Exemple<(... , ...)
        /// test : Int32Exemple=lt=(... , ...)
        /// </summary>
        [Fact]
        public void TooManyArgumentExceptionTest()
        {
            Assert.Throws<AutumnQueryComparisonTooManyArgumentException>(() => { Parse("Int32Exemple<=(1,2)"); });
            Assert.Throws<AutumnQueryComparisonTooManyArgumentException>(() => { Parse("Int32Exemple=le=(1,2)"); });
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
        public void InvalidComparatorSelectionExceptionTest()
        {
            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("StringExemple<='" + GetRandom<string>() + "'");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("StringExemple=le=" + GetRandom<string>() + "'");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("BooleanExemple<=true");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("BooleanExemple=le=true");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("NullableBooleanExemple<=true");
            });

            Assert.Throws<AutumnQueryComparisonInvalidComparatorSelectionException>(() =>
            {
                Parse("NullableBooleanExemple=le=true");
            });
        }

        /// <summary>
        /// test : NullableInt16Exemple==1
        /// test : NullableInt16Exemple=eq=1
        /// </summary>
        [Fact]
        public void NullableInt16Test()
        {
            Assert.True(Le<short?>(1));
        }

        /// <summary>
        /// test : Int16Exemple==1
        /// test : Int16Exemple=eq=1
        /// </summary>
        [Fact]
        public void Int16Test()
        {
            Assert.True(Le<short>(1));
        }

        /// <summary>
        /// test : NullableInt32Exemple==1
        /// test : NullableInt32Exemple=eq=1
        /// </summary>
        [Fact]
        public void NullableInt32Test()
        {
            Assert.True(Le<int?>(1));
        }

        /// <summary>
        /// test : Int16Exemple==1
        /// test : Int16Exemple=eq=1
        /// </summary>
        [Fact]
        public void Int32Test()
        {
            Assert.True(Le<int>(1));
        }

        /// <summary>
        /// test : NullableInt64Exemple==1
        /// test : NullableInt64Exemple=eq=1
        /// </summary>
        [Fact]
        public void NullableInt64Test()
        {
            Assert.True(Le<long?>(1));
        }

        /// <summary>
        /// test : Int16Exemple==1
        /// test : Int16Exemple=eq=1
        /// </summary>
        [Fact]
        public void Int64Test()
        {
            Assert.True(Le<long>(1));
        }


        /// <summary>
        /// test : NullableSingleExemple==...
        /// test : NullableSingleExemple=eq=...
        /// </summary>
        [Fact]
        public void NullableSingleTest()
        {
            Assert.True(Le<float?>((float) 1.5));
        }

        /// <summary>
        /// test : SingleExemple==...
        /// test : SingleExemple=eq=...
        /// </summary>
        [Fact]
        public void SingleTest()
        {
            Assert.True(Le<float>((float) 1.5));
        }

        /// <summary>
        /// test : NullableDoubleExemple==...
        /// test : NullableDoubleExemple=eq=...
        /// </summary>
        [Fact]
        public void NullableDoubleTest()
        {
            Assert.True(Le<double?>((double) 1.5));
        }

        /// <summary>
        /// test : DoubleExemple==...
        /// test : DoubleExemple=eq=...
        /// </summary>
        [Fact]
        public void DoubleTest()
        {
            Assert.True(Le<double>((double) 1.5));
        }


        /// <summary>
        /// test : NullableDecimalExemple==...
        /// test : NullableDecimalExemple=eq=...
        /// </summary>
        [Fact]
        public void NullableDecimalTest()
        {
            Assert.True(Le<decimal?>((decimal) 1.5));
        }

        /// <summary>
        /// test : DecimalExemple==...
        /// test : DecimalExemple=eq=...
        /// </summary>
        [Fact]
        public void DecimalTest()
        {
            Assert.True(Le<decimal>((decimal) 1.5));
        }


        private static bool Le<T>(T value, string valueToString = null)
        {
            var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments()[0] : typeof(T);
            var propertyName = (typeof(T).IsGenericType ? "Nullable" : "") + type.Name + "Exemple";
            var v = valueToString ?? Convert.ToString(value, CultureInfo.InvariantCulture);
            var parameter = Expression.Parameter(typeof(Exemple));
            var memberExpression =
                AutumnQueryHelper.GetMemberExpressionValue<Exemple>(parameter, propertyName, null);

            var actual = Expression.Lambda<Func<Exemple, bool>>(Expression.LessThanOrEqual(
                memberExpression.Expression,
                Expression.Constant(value, typeof(T))), parameter);

            var expected = Parse(propertyName + "<=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());

            expected = Parse(propertyName + "=le=" + v);
            Assert.Equal(actual.ToString(), expected.ToString());
            return true;
        }
    }
}