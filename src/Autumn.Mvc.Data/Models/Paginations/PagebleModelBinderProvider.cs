using System.Reflection;
using Autumn.Mvc.Data.Models.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class PageableModelBinderProvider : IModelBinderProvider
    {
        private readonly string _pageSizeField;
        private readonly string _pageNumberField;
        private readonly string _sortField;
        private readonly NamingStrategy _namingStrategy;

        public PageableModelBinderProvider(IConfiguration configuration, NamingStrategy namingStrategy = null)
        {
            _pageSizeField = configuration.GetSection("Autumn.Data.Mvc:PageSizeParameterName").Value ?? "PageSize";
            _pageNumberField = configuration.GetSection("Autumn.Data.Mvc:PageNumberParameterName").Value ?? "PageNumber";
            _sortField = configuration.GetSection("Autumn.Data.Mvc:SortParameterName").Value ?? "Sort";
            _namingStrategy = namingStrategy;
            if (_namingStrategy == null) return;
            var method = _namingStrategy.GetType().GetMethod("ResolvePropertyName",BindingFlags.Default|BindingFlags.Instance|BindingFlags.NonPublic);
            if (method == null) return;
            
            _pageSizeField = (string) method.Invoke(namingStrategy,new object[]{_pageSizeField});
            _pageNumberField = (string) method.Invoke(namingStrategy,new object[]{_pageNumberField});
            _sortField = (string) method.Invoke(namingStrategy,new object[]{_sortField});
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Pageable<>)) return null;
            return CommonHelper.GetPageableModelBinder(context.Metadata.ModelType, _pageSizeField, _pageNumberField,
                _sortField, _namingStrategy);
        }
    }
}