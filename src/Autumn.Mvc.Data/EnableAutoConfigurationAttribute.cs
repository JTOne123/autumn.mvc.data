using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class EnableAutoConfigurationAttribute : Attribute
    {
       
        protected EnableAutoConfigurationAttribute()
        {
            Order = 0;
        }
        
        public int Order { get; set; }
       
        public virtual void ConfigureServices(IServiceCollection serviceCollection, ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {

        }

        public virtual IApplicationBuilder Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            return app;
        }
    }
}