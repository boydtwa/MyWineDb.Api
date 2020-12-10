using Moq;
using Microsoft.AspNetCore.Http;
using MyWineDb.Api.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Compatibility;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Policy;
using Microsoft.Azure.Services.AppAuthentication;

namespace MyWineDb.Api.UnitTest
{
    [TestFixture]
    public class ModelHelperTests
    {
        private AzureTableEntityKeys _keys;

        [SetUp]
        public void Setup()
        {
            _keys = new AzureTableEntityKeys();

        }

        /// <summary>
        /// Tests Addition of key when only one value is in the AzureTableKey
        /// (i.e. a Cellar Id)
        /// Success is expected
        /// </summary>
        [TestCase("Cellar")]
        public void AddParsedEntityKey_WithOnlyLeaf_Success(string EntityName)
        {
            var rootKey = TestParams.TestExpectedAzureTableKeyForCellar;
            Models.ModelHelper.AddParsedEntityKey(EntityName, ref _keys, rootKey);
            Assert.IsNotNull(_keys.CellarId);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForCellar.PartitionKey, _keys.CellarId.PartitionKey);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForCellar.RowKey, _keys.CellarId.RowKey);
        }

        /// <summary>
        /// Tests addition of all entity Keys from a Bottle Entity Id
        /// success is expected
        /// </summary>
        [Test]
        public void AddParsedEntityKey_AllAncestorsPopulatedFromBottleEntity_Success()
        {
            var rootKey = TestParams.TestExpectedAzureTableKeyForBottle;
            Models.ModelHelper.AddParsedEntityKey("Bottle", ref _keys, rootKey, 0, rootKey, Models.ModelHelper.BottlePath);

            Assert.IsNotNull(_keys.BottleId);
            Assert.IsNotNull(_keys.CellarId);
            Assert.IsNotNull(_keys.WineryId);
            Assert.IsNotNull(_keys.WineId);
            Assert.IsNotNull(_keys.WineBottleId);

            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForBottle.PartitionKey, _keys.BottleId.PartitionKey);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForBottle.RowKey, _keys.BottleId.RowKey);

            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForCellar.PartitionKey, _keys.CellarId.PartitionKey);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForCellar.RowKey, _keys.CellarId.RowKey);

            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForWinery.PartitionKey, _keys.WineryId.PartitionKey);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForWinery.RowKey, _keys.WineryId.RowKey);

            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForWine.PartitionKey, _keys.WineId.PartitionKey);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForWine.RowKey, _keys.WineId.RowKey);

            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForWineBottle.PartitionKey, _keys.WineBottleId.PartitionKey);
            Assert.AreEqual(TestParams.TestExpectedAzureTableKeyForWineBottle.RowKey, _keys.WineBottleId.RowKey);
        }

        /// <summary>
        /// Tests successful retrieval of a CultureInfo object
        /// based on a country location for a Cellar
        /// </summary>
        /// <param name="Country">Three letter Country Abbreviation</param>
        [TestCase("USA")]
        public void GetCultureInfo_Success(string Country)
        {
            Assert.AreEqual(CultureInfo.CurrentCulture, Models.ModelHelper.GetCultureInfo(Country));
        }

        /// <summary>
        /// Tests failure to assign a CultureInfo object for
        /// provided Country Code;
        /// Argument Exception is expected
        /// </summary>
        /// <param name="Country">Three letter Country Abbreviation</param>
        /// <returns>Type of expected Exception</returns>
        [TestCase("foo", ExpectedResult = typeof(ArgumentException))]
        public Type GetCultureInfo_Failure(string Country)
        {
            return Assert.Catch(() => Models.ModelHelper.GetCultureInfo(Country)).GetType();
        }

        /// <summary>
        /// Test the Breaks up of a Key into its individual Entity Ids
        /// Expectation is a list 
        /// </summary>
        /// <param name="EntityRowId">A Multi-Part Entity RowId</param>
        [TestCase("a-b-c")]
        public void GetPartsOfKey_EntityKeyNotNull_Success(string EntityRowId)
        {
            var entityId = new AzureTableKey() {RowKey = EntityRowId};
            var expectedResult = new string[] {"a", "b", "c"};
            Assert.AreEqual(expectedResult,Models.ModelHelper.GetPartsOfKey(entityId));
        }

        [Test]
        public void GetPartsOfKey_EntityKeyNull_Success()
        {
            var expectedResult = new List<string>();
            Assert.AreEqual(expectedResult, Models.ModelHelper.GetPartsOfKey(null));
        }

    }
}