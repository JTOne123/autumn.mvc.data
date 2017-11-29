using Newtonsoft.Json.Serialization;
using Xunit;

namespace Autumn.Mvc.Data.Tests
{
    public class AutumnExtensionsTest
    {
        [Fact]
        public void PascalCase()
        {
            var actual = "that_is_test";
            var expected = "ThatIsTest";
            Assert.True(expected == actual.ToPascalCase());
        }

        [Fact]
        public void CamelCase()
        {
            var actual = "That_Is_test";
            var expected = "thatIsTest";
            Assert.True(expected == actual.ToCamelCase());
            Assert.True(expected == actual.ToCase(new CamelCaseNamingStrategy()));

        }


        [Fact]
        public void SnakeCase()
        {
            var actual = "thatIsTest";
            var expected = "that_is_test";
            Assert.True(expected == actual.ToSnakeCase());
            Assert.True(expected == actual.ToCase(new SnakeCaseNamingStrategy()));
        }
    }
}