using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Models.Paginations;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen; 

namespace Autumn.Mvc.Data.Swagger
{
    public class SwaggerOperationFilter : IOperationFilter
    {

        private const string ConsumeContentType = "application/json";
        private static readonly ConcurrentDictionary<Type,Dictionary<HttpMethod,Schema>> Caches = new ConcurrentDictionary<Type,Dictionary<HttpMethod,Schema>>();
        private readonly AutumnSettings _settings;

        public SwaggerOperationFilter(AutumnSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// apply operation description
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public virtual void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;
            if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)) return;

            if (IsRepositoryControllerAsync(actionDescriptor))
            {
                ProcessToRepositoryControllerAsync(_settings, operation, actionDescriptor);
            }
            else
            {
                ProcessToOtherController(_settings, operation, actionDescriptor);
            }
        }

        private static void ProcessToOtherController(AutumnSettings settings, Operation operation,
            ControllerActionDescriptor actionDescriptor)
        {
            if (operation.Parameters.Count > actionDescriptor.MethodInfo.GetParameters().Length)
            {
                operation.Parameters.Clear();
            }
            foreach (var parameterInfo in actionDescriptor.MethodInfo.GetParameters())
            {
                var parameterType = parameterInfo.ParameterType;

                if (parameterType.IsGenericType && (typeof(IPageable<>).IsSubclassOfRawGeneric(parameterType) ||
                                                    typeof(Page<>).IsSubclassOfRawGeneric(parameterType)))
                {
                    BuildPageableParameters(settings, operation, parameterType.GetGenericArguments()[0]);
                }

                if (!parameterType.IsGenericType ||
                    (!typeof(Expression<>).IsSubclassOfRawGeneric(parameterType))) continue;
                {
                    BuildQueryParameters(settings, operation);
                }
            }
        }

        private static void ProcessToRepositoryControllerAsync(AutumnSettings settings, Operation operation,
            ControllerActionDescriptor actionDescriptor)
        {
            
            // find entity type
            var entityType = actionDescriptor.ControllerTypeInfo.GetGenericArguments()[0];
            // find entity type info
            if (!settings.DataSettings().ResourceInfos.ContainsKey(entityType)) return;
            var resourceInfo = settings.DataSettings().ResourceInfos[entityType];
            // register response swagger schema for GET request
            var entitySchemaGet = GetOrRegistrySchema(entityType,HttpMethod.Get,settings.NamingStrategy);
            // register request swagger schema for POST request
            var entitySchemaPost = GetOrRegistrySchema(resourceInfo.ProxyRequestTypes[HttpMethod.Post], HttpMethod.Post,settings.NamingStrategy);
            // register request swagger schema for PUT request
            var entitySchemaPut = GetOrRegistrySchema(resourceInfo.ProxyRequestTypes[HttpMethod.Put], HttpMethod.Put,settings.NamingStrategy);
            var errorSchemaBadRequest = GetOrRegistrySchema(typeof(ErrorModelBadRequest), HttpMethod.Get, settings.NamingStrategy);
            var errorSchemaInternalErrorRequest = GetOrRegistrySchema(typeof(ErrorModelInternalError), HttpMethod.Get, settings.NamingStrategy);
            operation.Responses = new ConcurrentDictionary<string, Response>();
            // add generic reponse for internal error from server
            operation.Responses.Add(((int)HttpStatusCode.BadRequest).ToString(), new Response() {Schema = errorSchemaBadRequest});
            // add generic reponse for internal error from server
            operation.Responses.Add(((int)HttpStatusCode.InternalServerError).ToString(), new Response() {Schema = errorSchemaInternalErrorRequest});
            operation.Consumes.Clear();
           
            IParameter parameter;
            // create operation description in term of ActionName
            switch (actionDescriptor.ActionName)
            {
                // operation is "Put"
                case "Put":
                    operation.Consumes.Add(ConsumeContentType);
                    parameter = operation.Parameters.Single(p => p.Name == "id");
                    parameter.Description = "ID of the object to update";
                    parameter = operation.Parameters.Single(p => p.Name == "entityPutRequest");
                    parameter.Name = "entity";
                    parameter.Description = "New value of the object";
                    ((BodyParameter) parameter).Schema = entitySchemaPut;
                    parameter.Required = true;

                    operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                        new Response() {Schema = entitySchemaGet,Description = "Resource is Updated"});
                    break;
                
                case "Delete":
                    operation.Consumes.Add(ConsumeContentType);
                    parameter = operation.Parameters.Single(p => p.Name == "id");
                    parameter.Description = "ID of the object to delete";
                    parameter.Required = true;
                    operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                        new Response() {Schema = entitySchemaGet,Description = "Resource is deleted"});
                    break;

                case "Post":
                    operation.Consumes.Add(ConsumeContentType);
                    parameter = operation.Parameters.Single(p => p.Name == "entityPostRequest");
                    parameter.Name = "entity";
                    parameter.Description = "Object to create";
                    parameter.Required = true;
                    ((BodyParameter) parameter).Schema = entitySchemaPost;
                    operation.Responses.Add(((int) HttpStatusCode.Created).ToString(),
                        new Response() {Schema = entitySchemaGet, Description = "Resource is created"});
                    break;

                case "GetById":
                    parameter = operation.Parameters.Single(p => p.Name == "id");
                    parameter.Description = "ID of resource to search";
                    parameter.Required = true;
                    operation.Responses.Add(((int) HttpStatusCode.OK).ToString(),
                        new Response() {Schema = entitySchemaGet});
                    operation.Responses.Add(((int) HttpStatusCode.NotFound).ToString(),
                        new Response() {Description = "Resource not Found"});
                    break;

                default:
                    operation.Parameters.Clear();
                    BuildQueryParameters(settings, operation);
                    BuildPageableParameters(settings, operation, entityType);
                    break;
            }
        }

        private static void BuildQueryParameters(AutumnSettings settings, Operation operation)
        {
            IParameter parameter = new NonBodyParameter
            {
                Type = "string",
                In = "query",
                Description = "Query to search (cf. http://tools.ietf.org/html/draft-nottingham-atompub-fiql-00)",
                Name = settings.QueryField
            };
            operation.Parameters.Add(parameter);
        }

        private static void BuildPageableParameters(AutumnSettings settings, Operation operation, Type entityType)
        {
            var genericPageType = typeof(Page<>);
            var pageType = genericPageType.MakeGenericType(entityType);
            var schema = GetOrRegistrySchema(pageType, HttpMethod.Get,settings.NamingStrategy);
            operation.Responses = new Dictionary<string, Response>
            {
                {"200", new Response() {Schema = schema, Description = "OK"}},
                {"206", new Response() {Schema = schema, Description = "Partial Content"}}
            };

            IParameter parameter;
            parameter = new NonBodyParameter
            {
                In = "query",
                Type = "integer",
                Minimum = 0,
                Format = "int32",
                Description = "Size of the page",
                Default = settings.PageSize,
                Name = settings.PageSizeField
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
                Name = settings.PageNumberField
            };
            operation.Parameters.Add(parameter);
        }

        private static bool IsRepositoryControllerAsync(ControllerActionDescriptor actionDescriptor)
        {
            if (!actionDescriptor.ControllerTypeInfo.IsGenericType) return false;
            return actionDescriptor.ControllerTypeInfo.GetGenericTypeDefinition() ==
                   typeof(RepositoryControllerAsync<,,,>) ||
                   typeof(RepositoryControllerAsync<,,,>).IsSubclassOfRawGeneric(actionDescriptor.ControllerTypeInfo);
        }

        private static bool IsPrimitiveType(Type type)
        {
            return type == typeof(string) ||
                   type == typeof(short) ||
                   type == typeof(short?) ||
                   type == typeof(int) ||
                   type == typeof(int?) ||
                   type == typeof(long) ||
                   type == typeof(long?) || type == typeof(decimal) ||
                   type == typeof(decimal?) ||
                   type == typeof(double) ||
                   type == typeof(double?) || type == typeof(DateTime) ||
                   type == typeof(DateTime?) || type == typeof(DateTimeOffset) ||
                   type == typeof(DateTimeOffset?) || type == typeof(byte) ||
                   type == typeof(byte?) || type == typeof(bool) ||
                   type == typeof(bool?);
        }

        private static Schema BuildSchema(Type type, HttpMethod httpMethod, NamingStrategy namingStrategy)
        {
            var result = new Schema();
            if (type == typeof(string))
            {
                result.Type = "string";
            }
            else if (type == typeof(short) ||
                     type == typeof(short?) ||
                     type == typeof(int) ||
                     type == typeof(int?))
            {
                result.Type = "integer";
                result.Format = "int32";
            }
            else if (type == typeof(long) ||
                     type == typeof(long?))
            {
                result.Type = "integer";
                result.Format = "int64";
            }
            else if (type == typeof(decimal) ||
                     type == typeof(decimal?) ||
                     type == typeof(double) ||
                     type == typeof(double?))
            {
                result.Type = "number";
                result.Format = "double";
            }
            else if (type == typeof(DateTime) ||
                     type == typeof(DateTime?))
            {
                result.Type = "string";
                result.Format = "date-time";
            }
            else if (type == typeof(DateTimeOffset) ||
                     type == typeof(DateTimeOffset?))
            {
                result.Type = "string";
                result.Format = "date-time";
            }
            else if (type == typeof(byte) ||
                     type == typeof(byte?))
            {
                result.Type = "string";
                result.Format = "byte";
            }
            else if (type == typeof(bool) ||
                     type == typeof(bool?))
            {
                result.Type = "boolean";
            }
            else if (type.IsEnum ||
                     (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                      type.GetGenericArguments()[0].IsEnum))
            {
                result.Type = "string";
            }
            else
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    result.Type = "array";
                    result.Items = GetOrRegistrySchema(type.GetGenericArguments()[0],httpMethod,namingStrategy);
                }
                else if (type.IsArray)
                {
                    result.Type = "array";
                    result.Items = GetOrRegistrySchema(type, httpMethod,namingStrategy);
                }
                else
                {
                    result = GetOrRegistrySchema(type, httpMethod,namingStrategy);
                }
            }
            return result;
        }


        private static Schema BuildSchema(PropertyInfo property, HttpMethod httpMethod, NamingStrategy namingStrategy)
        {
            if (httpMethod == HttpMethod.Get) return BuildSchema(property.PropertyType, httpMethod, namingStrategy);
            var attribute = property.GetCustomAttribute<IgnoreAttribute>();
            if (attribute == null) return BuildSchema(property.PropertyType, httpMethod, namingStrategy);
            if (!attribute.Insertable && httpMethod == HttpMethod.Post) return null;
            if (!attribute.Updatable && httpMethod == HttpMethod.Put) return null;
            return BuildSchema(property.PropertyType, httpMethod, namingStrategy);
        }

        private static Schema GetOrRegistrySchema(Type type, HttpMethod method, NamingStrategy namingStrategy)
        {
            lock (Caches)
            {
                if (Caches.ContainsKey(type) && Caches[type].ContainsKey(method)) return Caches[type][method];
                if (!Caches.ContainsKey(type)) Caches[type] = new Dictionary<HttpMethod, Schema>();
                try
                {
                    if (IsPrimitiveType(type))
                    {
                        return BuildSchema(type, method, namingStrategy);
                    }

                    if (type.IsInterface)
                    {
                        if (type.IsGenericType)
                        {
                            if (type.GetGenericTypeDefinition() == typeof(IPageable<>))
                            {
                                type = typeof(Pageable<>).MakeGenericType(type.GetGenericArguments()[0]);
                            }

                            if (type.GetGenericTypeDefinition() == typeof(IPage<>))
                            {
                                type = typeof(Page<>).MakeGenericType(type.GetGenericArguments()[0]);
                            }

                            if (type.GetGenericTypeDefinition() == typeof(IList<>))
                            {
                                var schema = GetOrRegistrySchema(type.GetGenericArguments()[0], method, namingStrategy);
                                return new Schema()
                                {
                                    Type = "array",
                                    Items = schema
                                };
                            }
                        }
                    }

                    var o = Activator.CreateInstance(type);
                    var stringify = JsonConvert.SerializeObject(o);
                    var expected = JObject.Parse(stringify);
                    var result = new Schema {Properties = new ConcurrentDictionary<string, Schema>()};
                    foreach (var propertyName in expected.Properties())
                    {
                        var name = namingStrategy.GetPropertyName(propertyName.Name, false);
                        var property = type.GetProperty(propertyName.Name);
                        if (property == null) continue;
                        var propertySchema = BuildSchema(property, method, namingStrategy);
                        if (propertySchema != null)
                        {
                            result.Properties.Add(name, propertySchema);
                        }
                    }

                    Caches[type][method] = result;
                }
                catch
                {
                    // impossible de convertir en schema
                    Caches[type][method] = new Schema() {Type = "object"};
                }

                return Caches[type][method];
            }
        }
    }
}