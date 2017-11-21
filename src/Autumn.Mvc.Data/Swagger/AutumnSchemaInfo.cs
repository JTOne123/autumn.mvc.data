using System;
using Swashbuckle.AspNetCore.Swagger;

namespace Autumn.Mvc.Data.Swagger
{
    public class AutumnSchemaInfo
    {
        public Type OriginType { get; set; }
        public Schema Schema { get; set; }
        public Type DtoType { get; set; }
    }
}