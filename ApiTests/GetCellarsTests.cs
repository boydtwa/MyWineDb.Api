using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWineDb.Api.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MyWineDb.Api.UnitTest
{
    [TestFixture]
    public class GetCellarsTests
    {
        [Test]
        public async Task Run_FailureToExecuteApiSuccessfully_Returns500Code()
        {
            var ds = TestHelpers.CreateMockDataStore();
            ds.Setup(s => s.GetCellarList()).Throws<NullReferenceException>();
            GetCellars.DataStore = ds.Object;
            var sut = await GetCellars.Run(TestHelpers.CreateMockRequest().Object,
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(StatusCodeResult), sut);
            Assert.AreEqual(500, ((StatusCodeResult)sut).StatusCode);
        }
        [Test]
        public async Task Run_Success_ReturnsExpectedCellarList()
        {
            var ds = TestHelpers.CreateMockDataStore();
            ds.Setup(s => s.GetCellarList())
                .ReturnsAsync(TestParams.TestExpectedCellarList);
            GetCellars.DataStore = ds.Object;
            var sut = await GetCellars.Run(
                TestHelpers.CreateMockRequest().Object,
                TestHelpers.CreateMockLogger().Object,
                TestHelpers.CreateMockExecutionContext().Object);
            Assert.IsInstanceOf(typeof(OkObjectResult), sut);
            Assert.IsInstanceOf<List<CellarSummaryModel>>(JsonConvert
                .DeserializeObject<List<CellarSummaryModel>>(((OkObjectResult) sut).Value.ToString()));
        }
    }
}
