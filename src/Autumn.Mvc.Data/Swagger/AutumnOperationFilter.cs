using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Helpers;
using Autumn.Mvc.Data.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;    
namespace Autumn.Mvc.Data.Swagger
{
    public class AutumnOperationFilter : IOperationFilter
    {

        private const string ConsumeContentType = "application/json";
        private static readonly ConcurrentDictionary<Type,Dictionary<string,Schema>> Caches = new ConcurrentDictionary<Type,Dictionary<string,Schema>>();
        private static readonly Schema AutumnErrorModelSchema;

        static AutumnOperationFilter()
        {
            AutumnErrorModelSchema = GetOrRegistrySchema(typeof(AutumnErrorModel), "GET");
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;
            if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)) return;
            if (!actionDescriptor.ControllerTypeInfo.IsGenericType &&
                actionDescriptor.ControllerTypeInfo.GetGenericTypeDefinition() !=
                typeof(RepositoryControllerAsync<,,,>)) return;

            
            
            var entityType = actionDescriptor.ControllerTypeInfo.GetGenericArguments()[0];
            var entityInfo = AutumnSettings.Instance.EntitiesInfos[entityType];
            var entitySchemaGet = GetOrRegistrySchema(entityType,"GET");
            var entitySchemaPost = GetOrRegistrySchema(entityInfo.ProxyTypes[AutumnIgnoreType.Post], "POST");
            var entitySchemaPut = GetOrRegistrySchema(entityInfo.ProxyTypes[AutumnIgnoreType.Put], "PUT");
            
            operation.Responses = new ConcurrentDictionary<string, Response>();
            operation.Responses.Add(((int)HttpStatusCode.InternalServerError).ToString(), new Response() {Schema = AutumnErrorModelSchema});
            operation.Consumes.Clear();
           
            IParameter parameter;
            
            operation.Parameters.Add(new NonBodyParameter()
            {
                Name="Accept-Language",
                @In="header",
                Type="string",
                Required = false,
                Description = "Accept language ( cf https://en.wikipedia.org/wiki/Content_negotiation)"
            });
   
            if (actionDescriptor.ActionName == "Put")
            {
                operation.Consumes.Add(ConsumeContentType);

                parameter = operation.Parameters.Single(p => p.Name == "id");
                parameter.Description = "Identifier of the object to update";

                parameter = operation.Parameters.Single(p => p.Name == "entityPut");
                parameter.Description = "New value of the object";
                ((BodyParameter) parameter).Schema = entitySchemaPut;
                parameter.Required = true;

                operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                    new Response() {Schema = entitySchemaGet});
            }
            else if (actionDescriptor.ActionName == "Delete")
            {
                operation.Consumes.Add(ConsumeContentType);

                parameter = operation.Parameters.Single(p => p.Name == "id");
                parameter.Description = "Identifier of the object to delete";
                parameter.Required = true;

                operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                    new Response() {Schema = entitySchemaGet});
            }
            else if (actionDescriptor.ActionName == "Post")
            {
                operation.Consumes.Add(ConsumeContentType);

                parameter = operation.Parameters.Single(p => p.Name == "entityPost");
                parameter.Description = "Value of the object to create";
                parameter.Required = true;
                ((BodyParameter) parameter).Schema = entitySchemaPost;
                operation.Responses.Add(((int) HttpStatusCode.Created).ToString(),
                    new Response() {Schema = entitySchemaGet, Description = "Created"});
            }
            else if (actionDescriptor.ActionName == "GetById")
            {
                parameter = operation.Parameters.Single(p => p.Name == "id");
                parameter.Description = "Identifier of the object to search";
                parameter.Required = true;

                operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                    new Response() {Schema = entitySchemaGet});
                operation.Responses.Add(((int) HttpStatusCode.NotFound).ToString(), new Response(){Description = "Not Found"});
            }
            else
            {
                var genericPageType = typeof(Models.Paginations.Page<>);
                var pageType = genericPageType.MakeGenericType(entityType);
                var schema = GetOrRegistrySchema(pageType, "GET");
                operation.Responses.Add("200", new Response() {Schema = schema, Description = "OK"});
                operation.Responses.Add("206", new Response() {Schema = schema, Description = "Partial Content"});
                operation.Parameters.Clear();
                parameter = new NonBodyParameter
                {
                    Type = "string",
                    In = "query",
                    Description = "Query to search (cf. http://tools.ietf.org/html/draft-nottingham-atompub-fiql-00)",
                    Name = AutumnSettings.Instance.QueryFieldName
                };
                operation.Parameters.Add(parameter);

                parameter = new NonBodyParameter
                {
                    In = "query",
                    Type = "integer",
                    Minimum = 0,
                    Format = "int32",
                    Description = "Size of the page",
                    Default = AutumnSettings.Instance.DefaultPageSize,
                    Name = AutumnSettings.Instance.PageSizeFieldName
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
                    Name = AutumnSettings.Instance.PageNumberFieldName
                };
                operation.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// build schema
        /// </summary>
        /// <param name="property"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static Schema BuildSchema(PropertyInfo property,string method = "GET")
        {
            if (method != "GET")
            {
                var attribute = property.GetCustomAttribute<AutumnIgnoreAttribute>();
                if (attribute != null)
                {
                    switch (attribute.Type)
                    {
                        case AutumnIgnoreType.Post  when method == "POST":
                            return null;
                        case AutumnIgnoreType.Put when method == "PUT":
                            return null;
                        // exclusion property vert PUT or POST and AutumnIgnore.Type==All
                        default:
                            return null;
                    }
                    
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
                    result.Items = GetOrRegistrySchema(property.PropertyType.GetGenericArguments()[0],method);
                }
                else if (property.PropertyType.IsArray)
                {
                    result.Type = "array";
                    result.Items = GetOrRegistrySchema(property.PropertyType, method);
                }
                else
                {
                    result = GetOrRegistrySchema(property.PropertyType, method);
                }
            }
            return result;
        }

        private static Schema GetOrRegistrySchema(Type type,string method)
        {
            lock (Caches)
            {
                if (Caches.ContainsKey(type) && Caches[type].ContainsKey(method)) return Caches[type][method];
                if (!Caches.ContainsKey(type)) Caches[type] = new Dictionary<string, Schema>();
                var o = Activator.CreateInstance(type);
                var stringify = JsonConvert.SerializeObject(o);
                var expected = JObject.Parse(stringify);
                var result = new Schema {Properties = new ConcurrentDictionary<string, Schema>()};
                foreach (var propertyName in expected.Properties())
                {
                    var name = propertyName.Name.ToCase(AutumnSettings.Instance.NamingStrategy);
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