using Autumn.Mvc.Data.EF.Configuration;
using Microsoft.AspNetCore.Builder;

namespace Autumn.Mvc.Data.EF
{
    public static class ApplicationBuilderExtensions
    {
        public static AutumnEntityFrameworkCoreSettings GetAutumnEntityFrameworkCoreSettings(this IApplicationBuilder app)
        {
            return (AutumnEntityFrameworkCoreSettings) app.ApplicationServices.GetService(typeof(AutumnEntityFrameworkCoreSettings));
        }
    }
}