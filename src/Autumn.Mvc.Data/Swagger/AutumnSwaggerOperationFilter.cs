using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Models.Paginations;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen; 

namespace Autumn.Mvc.Data.Swagger
{
    public class AutumnSwaggerOperationFilter : IOperationFilter
    {

        private const string ConsumeContentType = "application/json";
        private static readonly ConcurrentDictionary<Type,Dictionary<HttpMethod,Schema>> Caches = new ConcurrentDictionary<Type,Dictionary<HttpMethod,Schema>>();
        private static readonly Schema AutumnErrorModelSchema;
        private readonly AutumnSettings _settings;

        public AutumnSwaggerOperationFilter(AutumnSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        
        /// <summary>
        /// class initializer
        /// </summary>
        static AutumnSwaggerOperationFilter()
        {
            AutumnErrorModelSchema = GetOrRegistrySchema(typeof(AutumnErrorModel), HttpMethod.Get);
        }

        /// <summary>
        /// apply operation description
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;
            if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)) return;
            if (!actionDescriptor.ControllerTypeInfo.IsGenericType &&
                actionDescriptor.ControllerTypeInfo.GetGenericTypeDefinition() !=
                typeof(RepositoryControllerAsync<,,,>)) return;

            // find entity type
            var entityType = actionDescriptor.ControllerTypeInfo.GetGenericArguments()[0];
            // find entity type info
            var entityInfo = _settings.DataSettings().EntitiesInfos[entityType];
            // register response swagger schema for GET request
            var entitySchemaGet = GetOrRegistrySchema(entityType,HttpMethod.Get);
            // register request swagger schema for POST request
            var entitySchemaPost = GetOrRegistrySchema(entityInfo.ProxyRequestTypes[HttpMethod.Post], HttpMethod.Post);
            // register request swagger schema for PUT request
            var entitySchemaPut = GetOrRegistrySchema(entityInfo.ProxyRequestTypes[HttpMethod.Put], HttpMethod.Put);
            
            operation.Responses = new ConcurrentDictionary<string, Response>();
            // add generic reponse for internal error from server
            operation.Responses.Add(((int)HttpStatusCode.InternalServerError).ToString(), new Response() {Schema = AutumnErrorModelSchema});
            operation.Consumes.Clear();
           
            IParameter parameter;
            // create operation description in term of ActionName
            switch (actionDescriptor.ActionName)
            {
                // operation is "Put"
                case "Put":
                    operation.Consumes.Add(ConsumeContentType);

                    parameter = operation.Parameters.Single(p => p.Name == "id");
                    parameter.Description = "Identifier of the object to update";

                    parameter = operation.Parameters.Single(p => p.Name == "entityPutRequest");
                    parameter.Description = "New value of the object";
                    ((BodyParameter) parameter).Schema = entitySchemaPut;
                    parameter.Required = true;

                    operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                        new Response() {Schema = entitySchemaGet});
                    break;
                case "Delete":
                    operation.Consumes.Add(ConsumeContentType);

                    parameter = operation.Parameters.Single(p => p.Name == "id");
                    parameter.Description = "Identifier of the object to delete";
                    parameter.Required = true;

                    operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                        new Response() {Schema = entitySchemaGet});
                    break;
                case "Post":
                    operation.Consumes.Add(ConsumeContentType);

                    parameter = operation.Parameters.Single(p => p.Name == "entityPostRequest");
                    parameter.Description = "Value of the object to create";
                    parameter.Required = true;
                    ((BodyParameter) parameter).Schema = entitySchemaPost;
                    operation.Responses.Add(((int) HttpStatusCode.Created).ToString(),
                        new Response() {Schema = entitySchemaGet, Description = "Created"});
                    break;
                case "GetById":
                    parameter = operation.Parameters.Single(p => p.Name == "id");
                    parameter.Description = "Identifier of the object to search";
                    parameter.Required = true;

                    operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                        new Response() {Schema = entitySchemaGet});
                    operation.Responses.Add(((int) HttpStatusCode.NotFound).ToString(), new Response(){Description = "Not Found"});
                    break;
                default:
                    var genericPageType = typeof(Page<>);
                    var pageType = genericPageType.MakeGenericType(entityType);
                    var schema = GetOrRegistrySchema(pageType, HttpMethod.Get);
                    operation.Responses.Add("200", new Response() {Schema = schema, Description = "OK"});
                    operation.Responses.Add("206", new Response() {Schema = schema, Description = "Partial Content"});
                    operation.Parameters.Clear();
                    parameter = new NonBodyParameter
                    {
                        Type = "string",
                        In = "query",
                        Description = "Query to search (cf. http://tools.ietf.org/html/draft-nottingham-atompub-fiql-00)",
                        Name = _settings.QueryField
                    };
                    operation.Parameters.Add(parameter);

                    parameter = new NonBodyParameter
                    {
                        In = "query",
                        Type = "integer",
                        Minimum = 0,
                        Format = "int32",
                        Description = "Size of the page",
                        Default = _settings.PageSize,
                        Name = _settings.PageSizeField
                    };
                    operation.Parameters.Add(parameter);

                    parameter = new NonBodyParameter
                    {
                        In = "query",
                        Type = "integer",
                        Description = "Paging number (start to zero)",
                        Minimum = 0,
                        Format = "int32",
                        Default = 0,
                        Name = _settings.PageNumberField
                    };
                    operation.Parameters.Add(parameter);
                    break;
            }
        }

        /// <summary>
        /// build schema 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private static Schema BuildSchema(PropertyInfo property,HttpMethod httpMethod)
        {
            if (httpMethod != HttpMethod.Get)
            {
                var attribute = property.GetCustomAttribute<AutumnIgnoreOperationPropertyAttribute>();
                if (attribute != null)
                {
                    if (!attribute.Insertable && httpMethod == HttpMethod.Post) return null;
                    if (!attribute.Updatable && httpMethod == HttpMethod.Put) return null;
                }
            }

            var result = new Schema();
            if (property.PropertyType == typeof(string))
            {
                result.Type = "string";
            }
            else if (property.PropertyType == typeof(short) ||
                     property.PropertyType == typeof(short?) ||
                     property.PropertyType == typeof(int) ||
                     property.PropertyType == typeof(int?))
            {
                result.Type = "integer";
                result.Format = "int32";
            }
            else if (property.PropertyType == typeof(long) ||
                     property.PropertyType == typeof(long?))
            {
                result.Type = "integer";
                result.Format = "int64";
            }
            else if (property.PropertyType == typeof(decimal) ||
                     property.PropertyType == typeof(decimal?) ||
                     property.PropertyType == typeof(double) ||
                     property.PropertyType == typeof(double?))
            {
                result.Type = "number";
                result.Format = "double";
            }
            else if (property.PropertyType == typeof(DateTime) ||
                     property.PropertyType == typeof(DateTime?))
            {
                result.Type = "string";
                result.Format = "date-time";
            }
            else if (property.PropertyType == typeof(byte) ||
                     property.PropertyType == typeof(byte?))
            {
                result.Type = "string";
                result.Format = "byte";
            }
            else if (property.PropertyType == typeof(bool) ||
                     property.PropertyType == typeof(bool?))
            {
                result.Type = "boolean";
            }
            else
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    result.Type = "array";
                    result.Items = GetOrRegistrySchema(property.PropertyType.GetGenericArguments()[0],httpMethod);
                }
                else if (property.PropertyType.IsArray)
                {
                    result.Type = "array";
                    result.Items = GetOrRegistrySchema(property.PropertyType, httpMethod);
                }
                else
                {
                    result = GetOrRegistrySchema(property.PropertyType, httpMethod);
                }
            }
            return result;
        }

        /// <summary>
        /// register in cache ( if not exist) and return swagger schema for the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static Schema GetOrRegistrySchema(Type type,HttpMethod method)
        {
            lock (Caches)
            {
                if (Caches.ContainsKey(type) && Caches[type].ContainsKey(method)) return Caches[type][method];
                if (!Caches.ContainsKey(type)) Caches[type] = new Dictionary<HttpMethod, Schema>();
                var o = Activator.CreateInstance(type);
                var stringify = JsonConvert.SerializeObject(o);
                var expected = JObject.Parse(stringify);
                var result = new Schema {Properties = new ConcurrentDictionary<string, Schema>()};
                foreach (var propertyName in expected.Properties())
                {
                    var name = propertyName.Name.ToCase(AutumnApplication.Current.NamingStrategy);
                    var property = type.GetProperty(propertyName.Name);
                    if (property == null) continue;
                    var propertySchema = BuildSchema(property, method);
                    if (propertySchema != null)
                    {
                        result.Properties.Add(name, propertySchema);
                    }
                }
                Caches[type][method] = result;
                return result;
            }
        }
    }
}