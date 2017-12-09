using System;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Microsoft.Extensions.DependencyInjection;

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
         `    .NN.    .         							
              .NN.");
        }

        public static IServiceCollection AddAutumnData(this IServiceCollection services,
            Action<AutumnDataSettingsBuilder> autumnDataSettingsBuilderAction = null)
        {

            if (services == null)
                throw new ArgumentNullException(nameof(services));
     
            Console.WriteLine(Logo());
            var settings = services.GetAutumnSettings();

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

            return services;
        }


        public static AutumnDataSettings GetAutumnDataSettings(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            var service = serviceCollection.SingleOrDefault(c =>
                c.Lifetime == ServiceLifetime.Singleton && c.ServiceType == typeof(AutumnDataSettings));
            return (AutumnDataSettings) service?.ImplementationInstance;
        }

    }
}