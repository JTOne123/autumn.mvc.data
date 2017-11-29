using System.Net;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Autumn.Mvc.Data.Tests.Middlewares
{
    public class AutumnIgnoreOperationMiddlewareTest
    {
        /// <summary>
        /// test return http reponse with 404 on request PUT
        /// </summary>
        [Fact]
        public async void IgnoreOperationPut()
        {
            AutumnApplication.Initialize(new AutumnOptions(), GetType().Assembly);
            var mockHttpContext = new Moq.Mock<HttpContext>();
            mockHttpContext.Setup(o => o.Request.Method).Returns("PUT");
            mockHttpContext.SetupSet(o => o.Response.StatusCode = (int)HttpStatusCode.NotFound).Verifiable();
            mockHttpContext.Setup(o => o.Request.Path).Returns("/v1/AutumnIgnoreOperationMiddlewareTestPut/13");
            var middleware = new AutumnIgnoreOperationMiddleware(c => { return Task.CompletedTask; });
            var context = mockHttpContext.Object;
            await middleware.Invoke(context);
            mockHttpContext.Verify();
        }
        
        /// <summary>
        /// test return http reponse with 404 on request POST
        /// </summary>
        [Fact]
        public async void IgnoreOperationPost()
        {
            AutumnApplication.Initialize(new AutumnOptions(), GetType().Assembly);
            var mockHttpContext = new Moq.Mock<HttpContext>();
            mockHttpContext.Setup(o => o.Request.Method).Returns("POST");
            mockHttpContext.SetupSet(o => o.Response.StatusCode = (int)HttpStatusCode.NotFound).Verifiable();
            mockHttpContext.Setup(o => o.Request.Path).Returns("/v1/AutumnIgnoreOperationMiddlewareTestPost");
            var middleware = new AutumnIgnoreOperationMiddleware(c => { return Task.CompletedTask; });
            var context = mockHttpContext.Object;
            await middleware.Invoke(context);
            mockHttpContext.Verify();
        }
        
        /// <summary>
        /// test return http reponse with 404 on request DELETE
        /// </summary>
        [Fact]
        public async void IgnoreOperationDelete()
        {
            AutumnApplication.Initialize(new AutumnOptions(), GetType().Assembly);
            var mockHttpContext = new Moq.Mock<HttpContext>();
            mockHttpContext.Setup(o => o.Request.Method).Returns("DELETE");
            mockHttpContext.SetupSet(o => o.Response.StatusCode = (int)HttpStatusCode.NotFound).Verifiable();
            mockHttpContext.Setup(o => o.Request.Path).Returns("/v1/AutumnIgnoreOperationMiddlewareTestDelete/13");
            var middleware = new AutumnIgnoreOperationMiddleware(c => { return Task.CompletedTask; });
            var context = mockHttpContext.Object;
            await middleware.Invoke(context);
            mockHttpContext.Verify();
        }
    }
}