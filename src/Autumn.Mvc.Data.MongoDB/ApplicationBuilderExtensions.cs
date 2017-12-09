using Autumn.Mvc.Data.MongoDB.Configurations;
using Microsoft.AspNetCore.Builder;

namespace Autumn.Mvc.Data.MongoDB
{
    public static class ApplicationBuilderExtensions
    {
        public static AutumnMongoDBSettings GetAutumnMongoDBSettings(this IApplicationBuilder app)
        {
            return (AutumnMongoDBSettings) app.ApplicationServices.GetService(typeof(AutumnMongoDBSettings));
        }
    }
}