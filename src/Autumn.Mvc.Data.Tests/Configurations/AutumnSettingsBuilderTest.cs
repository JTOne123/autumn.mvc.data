using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Configurations
{
    public class AutumnConfigurationBuilderTest
    {
        private static AutumnSettingsBuilder Builder()
        {
            return new AutumnSettingsBuilder();
        }

        private static Assembly Assembly()
        {
            return (typeof(AutumnConfigurationBuilderTest)).Assembly;
        }

        [Fact]
        public void Pluralized()
        {
            var builder = Builder();
            Assert.True(AutumnSettings.Current.PluralizeController);
            builder.Pluralized(false);
            Assert.False(AutumnSettings.Current.PluralizeController);
            builder.Pluralized();
            Assert.True(AutumnSettings.Current.PluralizeController);
        }

        [Fact]
        public void PageSizeFieldName()
        {
            var builder = Builder();
            builder.PageSizeFieldName(null);
            Assert.True(AutumnSettings.Current.PageSizeFieldName==null);
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.PageSizeFieldName=="PageSize");
            Assert.True(AutumnSettings.Current.DefaultPageSize == 100);
            
            builder.PageSizeFieldName("Ps", 200);
            builder.Build((typeof(AutumnConfigurationBuilderTest).Assembly));
            
            Assert.True(AutumnSettings.Current.PageSizeFieldName == "Ps");
            Assert.True(AutumnSettings.Current.DefaultPageSize == 200);
            
        }

        [Fact]
        public void PageNumberFieldName()
        {
            var builder = Builder();
            builder.PageNumberFieldName(null);
            Assert.True(AutumnSettings.Current.PageNumberFieldName==null);
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.PageNumberFieldName=="PageNumber");
            
            builder.PageNumberFieldName("PN");
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.PageNumberFieldName == "PN");
        }

        [Fact]
        public void QueryFieldName()
        {
            var builder = Builder();
            builder.QueryFieldName(null);
            Assert.True(AutumnSettings.Current.QueryFieldName == null);
            
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.QueryFieldName == "Query");

            builder.NamingStrategy(new CamelCaseNamingStrategy());
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.QueryFieldName == "query");

            builder.QueryFieldName("QuerY");
            Assert.True(AutumnSettings.Current.QueryFieldName == "QuerY");
            builder.NamingStrategy(new SnakeCaseNamingStrategy());
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.QueryFieldName == "quer_y");
        }
        
        [Fact]
        public void SortFieldName()
        {
            var builder = Builder();
            builder.SortFieldName(null);
            Assert.True(AutumnSettings.Current.SortFieldName == null);
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.SortFieldName == "Sort");
            
            builder.NamingStrategy(new CamelCaseNamingStrategy());
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.SortFieldName == "sort");

            builder.SortFieldName("S");
            builder.Build(Assembly());
            Assert.True(AutumnSettings.Current.SortFieldName == "s");
        }
    }
}