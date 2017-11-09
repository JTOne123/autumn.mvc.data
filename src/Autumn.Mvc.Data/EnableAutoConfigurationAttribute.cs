using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        
        internal static List<EnableAutoConfigurationAttribute> Configurations { get; private set; }

        /// <summary>
        /// configure l'application
        /// </summary>
        /// <param name="assembly"></param>
        internal static List<EnableAutoConfigurationAttribute> Initialize(Assembly assembly)
        {
            Configurations = new List<EnableAutoConfigurationAttribute>();
            // find autoconfigurations
            foreach (var type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes()
                    .Where(a => a.GetType().IsSubclassOf(typeof(EnableAutoConfigurationAttribute)))
                    .OfType<EnableAutoConfigurationAttribute>();
                Configurations.AddRange(attributes);
            }

            Configurations = Configurations.OrderBy(c => c.Order)
                .ToList();

            return Configurations;
        }


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