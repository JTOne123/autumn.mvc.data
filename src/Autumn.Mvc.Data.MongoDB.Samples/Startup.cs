using System;
using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.MongoDB.Samples.Models;
using Autumn.Mvc.Data.MongoDB.Samples.Models.Generators;
using Autumn.Mvc.Data.Swagger;
using Foundation.ObjectHydrator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Autumn.Mvc.Data.MongoDB.Samples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env,IConfiguration configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = env;
        }

        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
       
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutumn(config =>
                    config
                        .QueryFieldName("q")
                        .PageNumberFieldName("pNum")
                        .PageSizeFieldName("pSiz")
                        .NamingStrategy(new SnakeCaseNamingStrategy())
                        .PageSize(50)
                        )
                .AddAutumnData()
                .AddAutumnMongo(config =>
                    config
                        .ConnectionString(
                            "mongodb://localhost:27017")
                        .Database("samples")
                )
                .AddSwaggerGen(c =>
                {
                 
                    foreach (var version in services.GetAutumnDataSettings().ApiVersions)
                    {
                        c.SwaggerDoc(version, new Info {Title = "api", Version = version});
                    }
                    c.DocumentFilter<SwaggerDocumentFilter>();
                    c.OperationFilter<SwaggerOperationFilter>();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            populateDatabase(app.GetAutumnMongoDBSettings());
            app
                .UseAutumnData()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    foreach (var version in app.GetAutumnDataSettings().ApiVersions)
                    {
                        c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                            string.Format("API {0}", version));
                    }
                })
                .UseMvc();
        }

        public async void populateDatabase(AutumnMongoDBSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Database);

            #region Customers

            var collection = database.GetCollection<CustomerV4>("customers");
            var count = collection.Count(e => e.Id != null);
            if (count == 0)
            {
                var custormerHydrator = new Hydrator<CustomerV4>()
                    .Ignoring(x => x.Id)
                    .With(x => x.Address, new Hydrator<Address>()
                        .WithAmericanCity(a => a.City)
                        .WithAmericanPostalCode(a => a.PostalCode, 80)
                        .WithAmericanAddress(a => a.Street)
                        .WithAmericanState(a => a.State))
                    .With(x => x.BirthDate, new BirthDateGenerator())
                    .With(x => x.Active, new ActiveGenerator())
                    .WithEmailAddress(x => x.Email)
                    .WithLastName(x => x.LastName)
                    .WithFirstName(x => x.FirstName)
                    .WithAlphaNumeric(x => x.Account, 10)
                    .WithDouble(x => x.Credit, 0, 10000)
                    .WithDouble(x => x.Credit, 0, 10000);

                var customers = custormerHydrator.GetList(100000);
                await collection.InsertManyAsync(customers);
            }

            #endregion

            #region Articles

            var collection2 = database.GetCollection<ArticleV2>("articles");
            count = collection2.Count(e => e.Id != null);
            if (count == 0)
            {
                var articleHydrator = new Hydrator<ArticleV2>()
                    .Ignoring(x => x.Id)
                    .WithText(x => x.Content, 500)
                    .WithAlphaNumeric(x => x.Title, 30)
                    .WithInteger(x => x.Score, 0, 100)
                    .WithDate(x => x.PublishDate, DateTime.Now.AddDays(-600), DateTime.Now.Date);

                var article = articleHydrator.GetList(1000);
                await collection2.InsertManyAsync(article);


            }

            #endregion

        }
    }
}