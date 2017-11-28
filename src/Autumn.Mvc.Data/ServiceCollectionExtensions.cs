using System;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Autumn.Mvc.Data
{
    public static class ServiceCollectionExtensions
    {
    
        private static string Logo()
        {
            return string.Format(@"               //               
             `+syo`             
     .`     `ossyys`     `.     
     /o+-` .osssyyys- `:oy/     
     +ssssosssssyyyyyyyyyyo     
    /sssssssssssyyyyyyyyyyy/    
 -/ossssssssssshdyyyyyyyyyyys+-           ___      __    __  .___________. __    __  .___  ___. .__   __. 
  :ossssssssyssNNyyhyyyyyyyys:`          /   \    |  |  |  | |           ||  |  |  | |   \/   | |  \ |  | 
    :osssssdNmhNNdmNdyyyyys:            /  ^  \   |  |  |  | `---|  |----`|  |  |  | |  \  /  | |   \|  |    
     .osssssydNNNNmhyyyyys.            /  /_\  \  |  |  |  |     |  |     |  |  |  | |  |\/|  | |  . `  | 
   `/ossssssssyNNhyyyyyyyyy/`         /  _____  \ |  `--'  |     |  |     |  `--'  | |  |  |  | |  |\   | 
  `/osssssssss+NNoyyyyyyyyys+`       /__/     \__\ \______/      |__|      \______/  |__|  |__| |__| \__| 
     .:ossss/..NN..oyyyyy/.     
       `+s/.  .NN.  .oyo`       
         `    .NN.    .         							Version : {0}
              .NN.", "0.0.1");
        }

        /// <summary>
        /// add autumn configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="autumnOptionsAction"></param>
        /// <param name="loggerFactory"></param>
        public static IServiceCollection AddAutumn(this IServiceCollection services,
            Action<AutumnOptionsBuilder> autumnOptionsAction, ILoggerFactory loggerFactory = null)
        {

            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (autumnOptionsAction == null)
                throw new ArgumentNullException(nameof(autumnOptionsAction));

            var logger = loggerFactory?.CreateLogger("AutumnServiceCollection");
            Console.WriteLine(Logo());
           
            var autumnConfigurationBuilder = new AutumnOptionsBuilder();
            autumnOptionsAction(autumnConfigurationBuilder);
            var settings = autumnConfigurationBuilder.Build();
            
            AutumnApplication.Initialize(settings,Assembly.GetCallingAssembly());
            
            var mvcBuilder = services.AddMvc(c =>
            {
                c.ModelBinderProviders.Insert(0,
                    new PageableModelBinderProvider());
                c.ModelBinderProviders.Insert(1,
                    new QueryModelBinderProvider());
            });

            var contractResolver =
                new DefaultContractResolver() {NamingStrategy = AutumnApplication.Current.NamingStrategy};
            mvcBuilder.AddJsonOptions(o => { o.SerializerSettings.ContractResolver = contractResolver; });

            mvcBuilder.ConfigureApplicationPartManager(p =>
            {
                p.FeatureProviders.Add(new RespositoryControllerFeatureProvider());
            });

            if (settings.UseSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    foreach (var version in AutumnApplication.Current.EntitiesInfos.Values.Select(e => e.ApiVersion)
                        .Distinct())
                    {
                        c.SwaggerDoc(version, new Info {Title = "api", Version = version});
                    }
                    c.DocumentFilter<AutumnSwaggerDocumentFilter>();
                    c.OperationFilter<AutumnSwaggerOperationFilter>();

                });
            }

            return services;
        }

    }
}