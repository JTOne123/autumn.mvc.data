using System;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Configurations.Exceptions;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Configurations
{
    public class AutumnOptionsBuilderTest
    {
        [Fact]
        public void DefaultApiVersionTest()
        {
            var expected = new AutumnOptionsBuilder();
            expected.DefaultApiVersion("v3");
            Assert.True(expected.Build().DefaultApiVersion == "v3");
            // null argument
            Assert.Throws(typeof(ArgumentNullException), () => expected.DefaultApiVersion(null));
            // invalid version
            Assert.Throws(typeof(ArgumentException), () => expected.DefaultApiVersion("448"));
        }

        [Fact]
        public void PageNumberFieldNameTest()
        {
            var expected = new AutumnOptionsBuilder();
            expected.PageNumberFieldName("test");
            Assert.True(expected.Build().PageNumberFieldName == "test");
            // null argument
            Assert.Throws(typeof(ArgumentNullException), () => expected.PageNumberFieldName(null));
        }
        
        [Fact]
        public void PageSizeFieldNameTest()
        {
            var expected = new AutumnOptionsBuilder();
            expected.PageSizeFieldName("test");
            Assert.True(expected.Build().PageSizeFieldName == "test");
            // null argument
            Assert.Throws(typeof(ArgumentNullException), () => expected.PageSizeFieldName(null));
        }
        
        [Fact]
        public void QueryFieldNameTest()
        {
            var expected = new AutumnOptionsBuilder();
            expected.QueryFieldName("test");
            Assert.True(expected.Build().QueryFieldName == "test");
            // null argument
            Assert.Throws(typeof(ArgumentNullException), () => expected.QueryFieldName(null));
        }

        
        [Fact]
        public void SortFieldName()
        {
            var expected = new AutumnOptionsBuilder();
            expected.SortFieldName("test");
            Assert.True(expected.Build().SortFieldName == "test");
            // null argument
            Assert.Throws(typeof(ArgumentNullException), () => expected.SortFieldName(null));
        }
        
        
        [Fact]
        public void BadFormatFieldNameException()
        {
            var expected = new AutumnOptionsBuilder();
            Assert.Throws(typeof(AutumnInvalidFormatFieldNameException), () => expected.PageSizeFieldName("test-"));
        }
        
        [Fact]
        public void FieldNameAlreadyUsedException()
        {
            var expected = new AutumnOptionsBuilder();
            expected.PageNumberFieldName("test");
            Assert.Throws(typeof(AutumnAlreadyFieldNameUsedException), () => expected.PageSizeFieldName("test"));
        }
    }
}