using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWineDb.Api.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MywineDb.Api;
using MyWineDb.Api.Services;


namespace MyWineDb.Api.UnitTest
{
    [TestFixture]
    public class GetCellarSummaryBottlesTests
    {
       [Test]
        public async Task Run_FailureToDeserializeRequest_Returns400StatusCode()
        {
            var ds = new Mock<IDataStore>();
            GetCellarSummaryBottles.DataStore = ds.Object;
            var sut = await GetCellarSummaryBottles.Run(TestHelpers.CreateMockRequest().Object, 
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(StatusCodeResult),sut);
            Assert.AreEqual(400, ((StatusCodeResult) sut).StatusCode);
        }

        [Test]
        public async Task Run_FailureToExecuteApiSuccessfully_Returns500Code()
        {
            var ds = TestHelpers.CreateMockDataStore();
            ds.Setup(s => s.GetCellarSummaryBottles(new AzureTableKey(){PartitionKey = "foo",RowKey = "bar"})).Throws<NullReferenceException>();
            GetCellarSummaryBottles.DataStore = ds.Object;
            var sut = await GetCellarSummaryBottles.Run(TestHelpers.CreateMockRequest(TestParams.TestExpectedAzureTableKeyForBottle).Object,
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(StatusCodeResult), sut);
            Assert.AreEqual(500, ((StatusCodeResult)sut).StatusCode);
        }

        [Test]
        public async Task Run_Success_ReturnsExpectedBottleDetails()
        {
            var ds = TestHelpers.CreateMockDataStore();
            ds.Setup(s => s.GetCellarSummaryBottles(It.IsAny<AzureTableKey>()))
                .ReturnsAsync(TestParams.TestExpectedBottleSummaryList);
            GetCellarSummaryBottles.DataStore = ds.Object;
            var sut = await GetCellarSummaryBottles.Run(
                TestHelpers.CreateMockRequest(TestParams.TestExpectedAzureTableKeyForBottle).Object,
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(OkObjectResult), sut);
            Assert.IsInstanceOf<IList<BottleBriefDataModel>>(JsonConvert.DeserializeObject<IList<BottleBriefDataModel>>((((OkObjectResult)sut).Value).ToString()));
        }
    }
}
