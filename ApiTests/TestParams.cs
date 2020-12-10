using System;
using System.Collections.Generic;
using System.Text;
using MyWineDb.Api.Models;
using Newtonsoft.Json;

namespace MyWineDb.Api.UnitTest
{
    public static class TestParams
    {
        public static readonly string TestAzureBottleEntity = "{\"PartitionKey\":\"USA\",\"RowKey\":\"1e5d14b4ff0e44bf8b33af2b56c00817-59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad-b44bc2fa34f5422086b2accaf6346a40\",\"PricePaid\":40,\"CellarDate\":\"2020-07-08T00:00:00.000Z\",\"BarCode\":\"C001000\",\"Timestamp\":\"2020-07-15T21:05:37.947Z\"}";
        public static readonly string TestBottleEntityRowKey = "1e5d14b4ff0e44bf8b33af2b56c00817-59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad-b44bc2fa34f5422086b2accaf6346a40";
        public static readonly string TestCellarEntityRowKey = "1e5d14b4ff0e44bf8b33af2b56c00817";
        public static readonly string TestWineBottleEntityRowKey = "59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad";
        public static readonly string TestWineEntityRowKey = "59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a";
        public static readonly string TestWineryEntityRowKey = "59500cbe53e24cd49b955f36381c4243";

        public static readonly string TestWineVarietalEntity = "{\"PartitionKey\":\"RED\",\"RowKey\":\"59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-CABSAUV-2766bc2b663546b999a7830eb8c20c8e-e251f1aebaf64834ae8c9e58111f55ee\"}";

        public static readonly string TestBottleDetailJson =
            "{\"EntityKeys\":{\"BottleId\":{\"PartitionKey\":\"boydtwa@yahoo.com\"" +
            ",\"RowKey\":\"1e5d14b4ff0e44bf8b33af2b56c00817-" +
            "59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-" +
            "dfa1c289d5e745fbb78b16112f7b83ad-b44bc2fa34f5422086b2accaf6346a40\"," +
            "\"TimeStamp\":\"0001-01-01T00:00:00+00:00\"},\"CellarId\":null," +
            "\"WineryId\":{\"PartitionKey\":\"USA\",\"RowKey\":" +
            "\"59500cbe53e24cd49b955f36381c4243\",\"TimeStamp\":" +
            "\"2020-07-22T19:05:21.6785579+00:00\"},\"WineId\":{\"PartitionKey\"" +
            ":\"USA\",\"RowKey\":\"59500cbe53e24cd49b955f36381c4243-" +
            "be43ba79c3c14777b0bb2848310dc11a\",\"TimeStamp\":" +
            "\"2020-07-21T19:47:29.4632569+00:00\"},\"WineBottleId\":{\"PartitionKey" +
            "\":\"USA\",\"RowKey\":\"59500cbe53e24cd49b955f36381c4243-" +
            "be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad\"" +
            ",\"TimeStamp\":\"2020-07-25T20:35:47.4041604+00:00\"}},\"BarCode\":" +
            "\"C0011000\",\"CellarDate\":\"2020-07-08T00:00:00Z\",\"DrinkDate\"" +
            ":null,\"PricePaid\":40.0,\"RetailPrice\":40.0,\"UPC\":\"\",\"Size\"" +
            ":750,\"QuantityProduced\":\"0\",\"BottleNumber\":\"0\",\"WineName\":" +
            "\"Red Wine\",\"WineProductLine\":\"Reserve\",\"Color\":\"RED\"," +
            "\"WineReleaseDate\":null,\"WineVarietalType\":\"Red Blend\"," +
            "\"WineMaker\":null,\"WineAppellation\":null,\"ResSugar\":\"0.00 %\"" +
            ",\"AlcPercent\":\"14.40 %\",\"isDessert\":false,\"isRose\":false," +
            "\"isEstate\":false,\"WineryName\":\"Barnard Griffin Winery\"," +
            "\"WineryRegion\":\"Washington\",\"WineryAddress\":\"878 Tulip Ln, " +
            "Richland, WA 99352\",\"WineryGeoCoordinates\":\"46.254489, -119.298418" +
            "\",\"Vintage\":2011,\"VarietalDetails\":[{\"WineVarietalId\":" +
            "{\"PartitionKey\":\"RED\",\"RowKey\":\"59500cbe53e24cd49b955f36381c4243" +
            "-be43ba79c3c14777b0bb2848310dc11a-CABSAUV\",\"TimeStamp\":" +
            "\"2020-07-25T22:19:16.2211262+00:00\"},\"VarietalName\":\"Cabernet " +
            "Sauvignon\",\"Percentage\":0.0,\"VineyardDetail\":{\"VineyardVarietalId\"" +
            ":{\"PartitionKey\":\"RED\",\"RowKey\":\"2766bc2b663546b999a7830eb8c20c8e-" +
            "CABSAUV\",\"TimeStamp\":\"2020-07-25T18:29:10.8292744+00:00\"}," +
            "\"VineyardName\":\"Red Haven Vineyard\",\"Region\":\"Washington\"" +
            ",\"VineyardAppellation\":\"Red Mountain\",\"Address\":null," +
            "\"PhoneNumber\":null,\"Email\":null,\"GeoCoordinates\":null," +
            "\"CloneName\":null,\"VineyardPercentage\":\"63.00 %\",\"Notes\":" +
            "null}},{\"WineVarietalId\":{\"PartitionKey\":\"RED\",\"RowKey\":" +
            "\"59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a" +
            "-MERLOT\",\"TimeStamp\":\"2020-07-17T22:41:05.1482601+00:00\"}," +
            "\"VarietalName\":\"Merlot\",\"Percentage\":0.0,\"VineyardDetail\"" +
            ":{\"VineyardVarietalId\":{\"PartitionKey\":\"RED\",\"RowKey\":" +
            "\"4f89e82098cc41398b1d320a65b0253c-MERLOT\",\"TimeStamp\":" +
            "\"2020-07-25T18:29:49.5452006+00:00\"},\"VineyardName\":" +
            "\"Ciel Du Cheval Vineyard\",\"Region\":\"Washington\",\"VineyardAppellation" +
            "\":\"Red Mountain\",\"Address\":\"4361 King Drive, West Richland, " +
            "WA 99353\",\"PhoneNumber\":null,\"Email\":\"info@cielducheval.com\"" +
            ",\"GeoCoordinates\":null,\"CloneName\":\" \",\"VineyardPercentage\":" +
            "\"37.00 %\",\"Notes\":null}}],\"Notes\":null}";


        public static readonly string TestCellarListJson =
            "[{\"CellarId\":{\"PartitionKey\":\"boydtwa@yahoo.com\",\"RowKey\":" +
            "\"1e5d14b4ff0e44bf8b33af2b56c00817\",\"TimeStamp\":" +
            "\"2020-07-21T17:07:32.6905492+00:00\"},\"Name\":\"Wine Closet\"," +
            "\"Description\":\"A Closet converted to a temperature controlled " +
            "wine cellar\",\"Capacity\":1000,\"BottleCount\":4,\"Value\":0.0," +
            "\"Bottles\":null,\"BottlesWithDetails\":null}]";

        public static readonly string TestCellarSummaryBottlesJson =
            "[{\"BottleId\":{\"PartitionKey\":\"boydtwa@yahoo.com\",\"RowKey\":" +
            "\"1e5d14b4ff0e44bf8b33af2b56c00817-99797b59316149b5890f5788e1c1bbe9-" +
            "788da822bd244ce68dca87abc2aab794-812bcebd7e6945aabd660f40a9027d28-" +
            "4b177cb5351849f6a90eaefe8891e9cf\",\"TimeStamp\":" +
            "\"2020-07-25T22:46:40.0960662+00:00\"},\"CountryOfOrigin\":\"USA\"," +
            "\"Region\":\"Washington\",\"WineryName\":\"Terra Blanca\",\"WineName\":" +
            "\"Onyx\",\"ProductLine\":null,\"Color\":\"RED\",\"Vintage\":2007," +
            "\"VarietalType\":\"Red Blend\",\"Size\":750,\"RetailPrice\":70.0,\"PricePaid"+
            "\":0.0,\"BarCode\":\"C0011003\",\"IsDesert\":false},{\"BottleId\":" +
            "{\"PartitionKey\":\"boydtwa@yahoo.com\",\"RowKey\":" +
            "\"1e5d14b4ff0e44bf8b33af2b56c00817-59500cbe53e24cd49b955f36381c4243-" +
            "6f94e4f4a4d6406d9939d69987945375-f099455be81a4eb79b20fad47063dd63-" + 
            "2e2ac23a19264395a8178f40d5ed7bf0\",\"TimeStamp\":" +
            "\"2020-07-25T22:59:55.3910635+00:00\"},\"CountryOfOrigin\":\"USA" +
            "\",\"Region\":\"Washington\",\"WineryName\":\"Barnard Griffin Winery" +
            "\",\"WineName\":\"raport\",\"ProductLine\":null,\"Color\":\"RED\"," +
            "\"Vintage\":2009,\"VarietalType\":\"Port\",\"Size\":500,\"RetailPrice" +
            "\":20.0,\"PricePaid\":0.0,\"BarCode\":\"C0011001\",\"IsDesert\":true}" +
            ",{\"BottleId\":{\"PartitionKey\":\"boydtwa@yahoo.com\",\"RowKey\":" +
            "\"1e5d14b4ff0e44bf8b33af2b56c00817-59500cbe53e24cd49b955f36381c4243-" +
            "be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad-" +
            "b44bc2fa34f5422086b2accaf6346a40\",\"TimeStamp\":" +
            "\"2020-08-24T23:16:19.2686192+00:00\"},\"CountryOfOrigin\":\"USA\"" +
            ",\"Region\":\"Washington\",\"WineryName\":\"Barnard Griffin Winery:=" +
            "\",\"WineName\":\"Red Wine\",\"ProductLine\":\"Reserve\",\"Color\":"+
            "\"RED\",\"Vintage\":2011,\"VarietalType\":\"Red Blend\",\"Size\":" +
            "750,\"RetailPrice\":40.0,\"PricePaid\":40.0,\"BarCode\":\"C0011000\"" +
            ",\"IsDesert\":false},{\"BottleId\":{\"PartitionKey\":\"boydtwa@yahoo.com\"" +
            ",\"RowKey\":\"1e5d14b4ff0e44bf8b33af2b56c00817-204ad0e02a304c14bebd815223060702" +
            "-c353c4987d8a422ba1a3ad69977874fa-19c0fb8ff0754cc1abfd79ade1744ac5-" +
            "1edad8788dea44a6b12c6905e940dfc2\",\"TimeStamp\":\"2020-07-25T20:53:33.2412795+00:00" +
            "\"},\"CountryOfOrigin\":\"USA\",\"Region\":\"Washington\",\"" +
            "WineryName\":\"EFESTĒ\",\"WineName\":\"Tough Guy\",\"ProductLine\"" +
            ":null,\"Color\":\"RED\",\"Vintage\":2016,\"VarietalType\":" +
            "\"Red Blend\",\"Size\":750,\"RetailPrice\":60.0,\"PricePaid\":0.0," +
            "\"BarCode\":\"C0011002\",\"IsDesert\":false}]";

        public static AzureTableKey TestExpectedAzureTableKeyForBottle = new AzureTableKey() { PartitionKey = "USA", RowKey = TestBottleEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForCellar = new AzureTableKey() { PartitionKey = "USA", RowKey = TestCellarEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForWineBottle = new AzureTableKey() { PartitionKey = "USA", RowKey = TestWineBottleEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForWine = new AzureTableKey() { PartitionKey = "USA", RowKey = TestWineEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForWinery = new AzureTableKey() { PartitionKey = "USA", RowKey = TestWineryEntityRowKey };

        public static BottleDetailModel TestExpectedBottleDetailModel =
            JsonConvert.DeserializeObject<BottleDetailModel>(TestBottleDetailJson);

        public static List<CellarSummaryModel> TestExpectedCellarList =
            JsonConvert.DeserializeObject<List<CellarSummaryModel>>(TestCellarListJson);

        public static List<BottleBriefDataModel> TestExpectedBottleSummaryList =
            JsonConvert.DeserializeObject<List<BottleBriefDataModel>>(TestCellarSummaryBottlesJson);

    }
}
