using System;
using System.Net;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Middlewares
{
    public class AutumnErrorHandlingMiddlewareTest
    {
        [Fact]
        public async void ThrowException()
        {
            AutumnApplication.Initialize(new AutumnOptions(), GetType().Assembly);
            var mockHttpContext = new Moq.Mock<HttpContext>();
            mockHttpContext.SetupSet(o => o.Response.StatusCode = (int) HttpStatusCode.InternalServerError)
                .Verifiable();
            mockHttpContext.SetupSet(o => o.Response.ContentType = "application/json").Verifiable();
            var middleware = new AutumnErrorHandlingMiddleware(c => { throw new Exception("test"); });
            await middleware.Invoke(mockHttpContext.Object);
            mockHttpContext.Verify();
        }
    }
}