using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Data;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Helpers;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutumnServiceCollectionExtensions
    {

        /// <summary>
        /// add autumn configuration
        /// </summary>
        /// <param name="services"></param>
        public static void AddAutumnMongo(this IServiceCollection services)
        {
            services.AddScoped(typeof(ICrudPageableRepositoryAsync<,>), typeof(MongoCrudPageableRepositoryAsync<,>));
        }
    }
}
