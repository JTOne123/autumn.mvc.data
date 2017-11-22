using System;
using System.Reflection;
using Autumn.Mvc.Data;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Models.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutumnServiceCollectionExtensions
    {
        private static ILogger _logger;


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
        /// <param name="configuration"></param>
        public static void AddAutumn(this IServiceCollection services, Func<IServiceCollection, AutumnOptions> config)
        {
            _logger = ApplicationLogging.CreateLogger("AutumnConfiguration");
            _logger.LogInformation(Logo());

            var settings = AutumnSettings.Build(config.Invoke(services), Assembly.GetCallingAssembly());
            services.AddSingleton(settings);

            var mvcBuilder = services.AddMvc(c =>
            {
                c.ModelBinderProviders.Insert(0,
                    new PageableModelBinderProvider(settings));
                c.ModelBinderProviders.Insert(1,
                    new QueryModelBinderProvider(settings));
            });

            var contractResolver = new DefaultContractResolver() {NamingStrategy = settings.NamingStrategy};
            mvcBuilder.AddJsonOptions(options => { options.SerializerSettings.ContractResolver = contractResolver; });

            mvcBuilder.ConfigureApplicationPartManager(p =>
            {
                p.FeatureProviders.Add(new RespositoryControllerFeatureProvider(settings));
            });

            settings.AutoConfigurations.ForEach(c =>
            {
                c.ConfigureServices(services, ApplicationLogging.LoggerFactory);
            });
        }

    }
}