using Autumn.Data.Rest;
using Autumn.Data.Rest.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Autumn.Data.Rest.Samples.Configurations;
using Autumn.Data.Rest.Samples.Repositories;

namespace Autumn.Data.Rest.Samples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var namingStrategy = new SnakeCaseNamingStrategy();
            IMvcBuilder a;
            services.AddMvc(config =>
                    {
                        config.ModelBinderProviders.Insert(0,
                            new PageableModelBinderProvider(Configuration, namingStrategy));
                        config.ModelBinderProviders.Insert(1,
                            new QueryModelBinderProvider(Configuration, namingStrategy));
                    }
                )
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = namingStrategy
                    };
                });

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"}));

            var settings = new Settings()
            {
                ConnectionString = Configuration.GetSection("Settings:ConnectionString").Value,
                DatabaseName = Configuration.GetSection("Settings:DatabaseName").Value
            };


            services.AddSingleton(settings);
            services.AddScoped<IUserRepository, UserRepository>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            }
            app.UseMvc();
        }
    }
}