using MyWineDb.Api.Models;
using NUnit.Framework;

namespace MyWindDb.Api.UnitTest.Models
{
    [TestFixture]
    public class ConstantsTest
    {
        private AzureTableEntityKeys keys;

        [SetUp]
        public void Setup()
        {
            keys = new AzureTableEntityKeys();

        }

        [TestCase]
        public void Constants_ParseKeysWithNoLeaf_Success()
        {
            var rootKey = Constants.TestExpectedAzureTableKeyForCellar;
            Constants.AddParsedEntityKey("Cellar", ref keys, rootKey);
            Assert.IsNotNull(keys.CellarId);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForCellar.PartitionKey, keys.CellarId.PartitionKey);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForCellar.RowKey, keys.CellarId.RowKey);
        }

        [TestCase]
        public void Constants_AllAncestorsPopulatedBottlePath_Success()
        {
            var rootKey = Constants.TestExpectedAzureTableKeyForBottle;
            Constants.AddParsedEntityKey("Bottle", ref keys, rootKey, 0, rootKey, Constants.BottlePath);

            Assert.IsNotNull(keys.BottleId);
            Assert.IsNotNull(keys.CellarId);
            Assert.IsNotNull(keys.WineryId);
            Assert.IsNotNull(keys.WineId);
            Assert.IsNotNull(keys.WineBottleId);

            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForBottle.PartitionKey, keys.BottleId.PartitionKey);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForBottle.RowKey, keys.BottleId.RowKey);

            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForCellar.PartitionKey, keys.CellarId.PartitionKey);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForCellar.RowKey, keys.CellarId.RowKey);

            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForWinery.PartitionKey, keys.WineryId.PartitionKey);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForWinery.RowKey, keys.WineryId.RowKey);

            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForWine.PartitionKey, keys.WineId.PartitionKey);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForWine.RowKey, keys.WineId.RowKey);

            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForWineBottle.PartitionKey, keys.WineBottleId.PartitionKey);
            Assert.AreEqual(Constants.TestExpectedAzureTableKeyForWineBottle.RowKey, keys.WineBottleId.RowKey);
        }
    }
}