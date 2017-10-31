using System;
using System.Data.Common;
using System.Reflection;
using Autumn.Data.Rest.Commons;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Mvc
{
    public class PageableModelBinderProvider : IModelBinderProvider
    {
        private readonly string _pageSizeField;
        private readonly string _pageNumberField;
        private readonly string _sortField;

        public PageableModelBinderProvider(IConfiguration configuration, NamingStrategy namingStrategy = null)
        {
            _pageSizeField = configuration.GetSection("Autumn.Data.Rest.Settings:Pageable_PageSize_Name").Value ?? "PageSize";
            _pageNumberField = configuration.GetSection("Autumn.Data.Rest.Settings:Pageable_PageNumber_Name").Value ?? "PageNumber";
            _sortField = configuration.GetSection("Autumn.Data.Rest.Settings:Pageable_Sort_Name").Value ?? "Sort";
            if (namingStrategy == null) return;
            var method = namingStrategy.GetType().GetMethod("ResolvePropertyName",BindingFlags.Default|BindingFlags.Instance|BindingFlags.NonPublic);
            if (method == null) return;
            
            _pageSizeField = (string) method.Invoke(namingStrategy,new object[]{_pageSizeField});
            _pageNumberField = (string) method.Invoke(namingStrategy,new object[]{_pageNumberField});
            _sortField = (string) method.Invoke(namingStrategy,new object[]{_sortField});
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(IPageable)
                ? new PageableModelBinder(_pageSizeField, _pageNumberField, _sortField)
                : null;
        }
    }
}