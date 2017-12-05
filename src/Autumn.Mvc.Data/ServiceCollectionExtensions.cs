using System;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Swagger;
using Microsoft.Extensions.DependencyInjection;
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

        public static IServiceCollection AddAutumnData(this IServiceCollection services,
            Action<AutumnDataSettingsBuilder> autumnDataSettingsBuilderAction = null)
        {

            if (services == null)
                throw new ArgumentNullException(nameof(services));
     
            Console.WriteLine(Logo());
            var service = services.Single(c =>
                c.ServiceType == typeof(AutumnSettings) && c.Lifetime == ServiceLifetime.Singleton);
            var settings = (AutumnSettings) service.ImplementationInstance;

            var autumnDataSettingsBuilder =
                new AutumnDataSettingsBuilder(settings.DataSettings(), Assembly.GetCallingAssembly());

            autumnDataSettingsBuilderAction?.Invoke(autumnDataSettingsBuilder);
            var dataSettings = autumnDataSettingsBuilder.Build();
            services.AddSingleton(dataSettings);
            services.AddMvc
            (
                c =>
                {
                    c.Conventions.Add(new RepositoryControllerNameConvention(dataSettings));
                }
            ).ConfigureApplicationPartManager(p =>
            {
                p.FeatureProviders.Add(new RespositoryControllerFeatureProvider(settings));
            });

            if (dataSettings.UseSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    foreach (var version in dataSettings.EntitiesInfos.Values.Select(e => e.ApiVersion)
                        .Distinct())
                    {
                        c.SwaggerDoc(version, new Info {Title = "api", Version = version});
                    }
                    c.DocumentFilter<SwaggerDocumentFilter>();
                    c.OperationFilter<SwaggerOperationFilter>();

                });
            }

            return services;
        }

    }
}