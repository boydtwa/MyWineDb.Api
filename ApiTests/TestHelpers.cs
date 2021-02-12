using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.UnitTest
{
    public static class TestHelpers
    {
        public static Mock<IDataStore> CreateMockDataStore()
        {
            return new Mock<IDataStore>();
        }

        public static Mock<ExecutionContext> CreateMockExecutionContext()
        {
            return new Mock<ExecutionContext>();
        }

        public static Mock<ILogger> CreateMockLogger()
        {
            return new Mock<ILogger>();
        }

        /// <summary>
        /// Makes a Mock Request Object with body payload
        /// </summary>
        /// <param name="body">Payload of the Request Object in Json form</param>
        /// <returns> a Mock HttpRequest Object</returns>
        public static Mock<HttpRequest> CreateMockRequest(object body = null)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
}
