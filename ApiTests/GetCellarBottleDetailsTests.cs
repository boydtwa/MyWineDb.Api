using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWineDb.Api.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using MyWineDb.Api.Services;

namespace MyWineDb.Api.UnitTest
{
    [TestFixture]
    public class GetCellarBottleDetailsTests
    {
        [Test]
        public async Task Run_FailureToDeserializeRequest_Returns400StatusCode()
        {
            GetCellarBottleDetails.DataStore = Mock.Of<IDataStore>();
            var sut = await GetCellarBottleDetails.Run(TestHelpers.CreateMockRequest().Object, 
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(StatusCodeResult),sut);
            Assert.AreEqual(400, ((StatusCodeResult) sut).StatusCode);
        }

        [Test]
        public async Task Run_FailureToExecuteApiSuccessfully_Returns500Code()
        {
            var ds = TestHelpers.CreateMockDataStore();
            ds.Setup(s => s.GetCellarBottleDetails(new AzureTableKey(){PartitionKey = "foo",RowKey = "bar"})).Throws<NullReferenceException>();
            GetCellarBottleDetails.DataStore = ds.Object;
            var sut = await GetCellarBottleDetails.Run(TestHelpers.CreateMockRequest(TestParams.TestExpectedAzureTableKeyForBottle).Object,
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(StatusCodeResult), sut);
            Assert.AreEqual(500, ((StatusCodeResult)sut).StatusCode);
        }

        [Test]
        public async Task Run_Success_ReturnsExpectedBottleDetails()
        {
            var ds = TestHelpers.CreateMockDataStore();
            ds.Setup(s => s.GetCellarBottleDetails(It.IsAny<AzureTableKey>()))
                .ReturnsAsync(TestParams.TestExpectedBottleDetailModel);
            GetCellarBottleDetails.DataStore = ds.Object;
            var sut = await GetCellarBottleDetails.Run(
                TestHelpers.CreateMockRequest(TestParams.TestExpectedAzureTableKeyForBottle).Object,
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(OkObjectResult), sut);
            Assert.IsInstanceOf<BottleDetailModel>(JsonConvert.DeserializeObject<BottleDetailModel>((((OkObjectResult)sut).Value).ToString()));
        }

    }
}
