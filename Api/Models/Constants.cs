using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public static class Constants
    {
        public static readonly string[] BottlePath = new string[] { "Winery", "Wine", "WineBottle" };
        private static readonly string[] WineVarietalPath = new string[] { "Varietal" };
        public static readonly string[] WineVarietalVineyardVarientalPath = { "Vineyard", "VineyardVarietal" };
        public static readonly string TestAzureBottleEntity = "{\"PartitionKey\":\"USA\",\"RowKey\":\"1e5d14b4ff0e44bf8b33af2b56c00817-59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad-b44bc2fa34f5422086b2accaf6346a40\",\"PricePaid\":40,\"CellarDate\":\"2020-07-08T00:00:00.000Z\",\"BarCode\":\"C001000\",\"Timestamp\":\"2020-07-15T21:05:37.947Z\"}";
        public static readonly string TestBottleEntityRowKey = "1e5d14b4ff0e44bf8b33af2b56c00817-59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad-b44bc2fa34f5422086b2accaf6346a40";
        public static readonly string TestCellarEntityRowKey = "1e5d14b4ff0e44bf8b33af2b56c00817";
        public static readonly string TestWineBottleEntityRowKey = "59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-dfa1c289d5e745fbb78b16112f7b83ad";
        public static readonly string TestWineEntityRowKey = "59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a";
        public static readonly string TestWineryEntityRowKey = "59500cbe53e24cd49b955f36381c4243";

        public static readonly string TestWineVarietalEntity = "{\"PartitionKey\":\"RED\",\"RowKey\":\"59500cbe53e24cd49b955f36381c4243-be43ba79c3c14777b0bb2848310dc11a-CABSAUV-2766bc2b663546b999a7830eb8c20c8e-e251f1aebaf64834ae8c9e58111f55ee\"}";



        public static AzureTableKey TestExpectedAzureTableKeyForBottle = new AzureTableKey() { PartitionKey = "USA", RowKey = TestBottleEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForCellar = new AzureTableKey() { PartitionKey = "USA", RowKey = TestCellarEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForWineBottle = new AzureTableKey() { PartitionKey = "USA", RowKey = TestWineBottleEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForWine = new AzureTableKey() { PartitionKey = "USA", RowKey = TestWineEntityRowKey };
        public static AzureTableKey TestExpectedAzureTableKeyForWinery = new AzureTableKey() { PartitionKey = "USA", RowKey = TestWineryEntityRowKey };

        public static CultureInfo GetCultureInfo(string cellarCountry)
        {
            switch (cellarCountry)
            {
                case "USA":
                {
                    return CultureInfo.CreateSpecificCulture("en-US");
                }

            }
            return CultureInfo.CreateSpecificCulture("en-US");
        }
        public static List<string> GetPartsOfKey(AzureTableKey key = null)
        {

            return key == null? new List<string>():key.RowKey.Split("-").ToList<string>();
        }

        /// <summary>
        /// Adds ParsedKey to EntiyKeys
        /// Timestamp must be loaded when object(s) are retireved
        /// </summary>
        /// <param name="Entity">Name of Azure Table</param>
        /// <param name="EntityKeys">The Collection of EntityKeys in the Model</param>
        /// <param name="ParsedKey">An AzureTableKey to load into the associated EntitId object in EntityKeys</param>
        /// <param name="ancestorPosition">Determins current Positon in Entity Path</param>
        /// <param name="leafKey">The last Key in the Entity Path (has all of the keys internal to itself)</param>
        /// <param name="EntityPath">a string array that determins the order of loading Ancestor Entity Keys</param>
        public static void AddParsedEntityKey(string Entity, ref AzureTableEntityKeys EntityKeys, AzureTableKey ParsedKey = null,
            int ancestorPosition = 0, AzureTableKey leafKey = null, string[] EntityPath = null)
        {
            var keyParts = GetPartsOfKey(leafKey);
            switch (Entity)
            {
                case "Cellar":
                    {
                        //not part of an Entity Path assumes ParsedKey is CellarId
                        if (ParsedKey != null)
                        {
                            EntityKeys.CellarId = ParsedKey;
                        }
                        break;
                    }

                case "Winery":
                    {
                        //leaf key must be valid BottleId (i.e. Has 5 keyParts) or 
                        //if null leafKey must be valid WineryKey
                        if(ParsedKey != null)
                        {
                            EntityKeys.WineryId = ParsedKey;
                        }
                        else if(keyParts.Count() == 1)
                        {
                            //leafKey is WineryKey
                            EntityKeys.WineryId = leafKey;
                        }
                        if (EntityPath != null && keyParts.Count() == 5)
                        {
                            MakeAncestorKeys(++ancestorPosition, leafKey, ref EntityKeys, Constants.BottlePath);
                        }
                        break;
                    }
                case "Wine":
                    {
                        if(ParsedKey != null)
                        {
                            EntityKeys.WineId = ParsedKey;
                        }
                        else if(keyParts.Count == 2)
                        {
                            EntityKeys.WineId = leafKey;
                        }
                        if (EntityPath != null && keyParts.Count() == 5)
                        {
                            MakeAncestorKeys(++ancestorPosition, leafKey, ref EntityKeys, Constants.BottlePath);
                        }
                        break;
                    }
                case "WineBottle":
                    {
                        if (ParsedKey != null)
                        {
                            EntityKeys.WineBottleId = ParsedKey;
                        }
                        else if (keyParts.Count == 3)
                        {
                            EntityKeys.WineId = leafKey;
                        }
                        // WineBottle is end of line in BottlePath
                        break;
                    }
                case "Bottle": 
                    {
                        if (ParsedKey != null)
                        {
                            EntityKeys.BottleId = ParsedKey;
                        }
                        else if (keyParts.Count == 5)
                        {
                            EntityKeys.WineId = leafKey;
                        }
                        if (EntityKeys.CellarId == null && EntityKeys.BottleId != null)
                        {
                            EntityKeys.CellarId = new AzureTableKey() { PartitionKey = EntityKeys.BottleId.PartitionKey, RowKey = EntityKeys.BottleId.RowKey.Substring(0, EntityKeys.BottleId.RowKey.IndexOf("-")) };
                        }
                        if (EntityPath != null && keyParts.Count() == 5)
                        {
                            MakeAncestorKeys(ancestorPosition, leafKey, ref EntityKeys, Constants.BottlePath);
                        }
                        break;
                    }
                    //case "WineVarietalVineyardVarietal":
                    //    {
                    //        WineVarietalVineyardVarietalId = key;
                    //        break;

                    //    }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ancestorPosition"></param>
        /// <param name="leafKey"></param>
        /// <param name="keys"></param>
        /// <param name="PathName"></param>
        private static void MakeAncestorKeys(int ancestorPosition, AzureTableKey leafKey, 
            ref AzureTableEntityKeys keys, string[] path = null)
        {
            path ??= new string[0];

            var partKeys = Constants.GetPartsOfKey(leafKey);
            var EntityName = path[ancestorPosition];

            for (var i = ancestorPosition; i < path.Length; i++)
            {
                if (EntityName == path[i])
                {
                    //build a key for the ancestor
                    var partKey = string.IsNullOrEmpty(leafKey.PartitionKey) ? string.Empty : leafKey.PartitionKey;
                    var rowKey = "";
                    var bottleAdjustLength = partKeys.Count() > 4 ? ancestorPosition + 2 : ancestorPosition;
                    var startIndex = partKeys.Count() > 4 ? 1 : 0;
                    for (var j = startIndex; j < bottleAdjustLength; j++)
                    {
                        rowKey += rowKey.Length > 0 ? $"-{partKeys[j]}" : partKeys[j];
                    }

                    var key = new AzureTableKey() { PartitionKey = partKey, RowKey = rowKey };
                    //pass it to ParseKeys
                    AddParsedEntityKey(EntityName, ref keys, key, ancestorPosition, leafKey, path);
                    return;
                }
            }
        }













    }

}
