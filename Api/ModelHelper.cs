using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MyWineDb.Api.Models
{
    public static class ModelHelper
    {
        public static readonly string[] BottlePath = {"Winery", "Wine", "WineBottle"};
        private static readonly string[] WineVarietalPath = {"Varietal"};
        public static readonly string[] WineVarietalVineyardVarietalPath = {"Vineyard", "VineyardVarietal"};


        //Static Methods
        /// <summary>
        ///     Adds ParsedKey to EntityKeys
        ///     Timestamp must be loaded when object(s) are retrieved
        /// </summary>
        /// <param name="Entity">Name of Azure Table</param>
        /// <param name="EntityKeys">The Collection of EntityKeys in the Model</param>
        /// <param name="ParsedKey">An AzureTableKey to load into the associated EntityId object in EntityKeys</param>
        /// <param name="ancestorPosition">Determines current Position in Entity Path</param>
        /// <param name="leafKey">The last Key in the Entity Path (has all of the keys internal to itself)</param>
        /// <param name="EntityPath">a string array that determines the order of loading Ancestor Entity Keys</param>
        public static void AddParsedEntityKey(string Entity, ref AzureTableEntityKeys EntityKeys,
            AzureTableKey ParsedKey = null,
            int ancestorPosition = 0, AzureTableKey leafKey = null, string[] EntityPath = null)
        {
            var keyParts = GetPartsOfKey(leafKey);
            switch (Entity)
            {
                case "Cellar":
                {
                    //not part of an Entity Path assumes ParsedKey is CellarId
                    if (ParsedKey != null) EntityKeys.CellarId = ParsedKey;
                    break;
                }

                case "Winery":
                {
                    //leaf key must be valid BottleId (i.e. Has 5 keyParts) or 
                    //if null leafKey must be valid WineryKey
                    if (ParsedKey != null)
                        EntityKeys.WineryId = ParsedKey;
                    else if (keyParts.Count() == 1)
                        //leafKey is WineryKey
                        EntityKeys.WineryId = leafKey;
                    if (EntityPath != null && keyParts.Count() == 5)
                        MakeAncestorKeys(++ancestorPosition, leafKey, ref EntityKeys, BottlePath);
                    break;
                }
                case "Wine":
                {
                    if (ParsedKey != null)
                        EntityKeys.WineId = ParsedKey;
                    else if (keyParts.Count == 2) EntityKeys.WineId = leafKey;
                    if (EntityPath != null && keyParts.Count() == 5)
                        MakeAncestorKeys(++ancestorPosition, leafKey, ref EntityKeys, BottlePath);
                    break;
                }
                case "WineBottle":
                {
                    if (ParsedKey != null)
                        EntityKeys.WineBottleId = ParsedKey;
                    else if (keyParts.Count == 3) EntityKeys.WineId = leafKey;
                    // WineBottle is end of line in BottlePath
                    break;
                }
                case "Bottle":
                {
                    if (ParsedKey != null)
                        EntityKeys.BottleId = ParsedKey;
                    else if (keyParts.Count == 5) EntityKeys.WineId = leafKey;
                    if (EntityKeys.CellarId == null && EntityKeys.BottleId != null)
                        EntityKeys.CellarId = new AzureTableKey
                        {
                            PartitionKey = EntityKeys.BottleId.PartitionKey,
                            RowKey = EntityKeys.BottleId.RowKey.Substring(0, EntityKeys.BottleId.RowKey.IndexOf("-"))
                        };
                    if (EntityPath != null && keyParts.Count() == 5)
                        MakeAncestorKeys(ancestorPosition, leafKey, ref EntityKeys, BottlePath);
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellarCountry"></param>
        /// <returns></returns>
        public static CultureInfo GetCultureInfo(string cellarCountry)
        {
            switch (cellarCountry)
            {
                case "USA":
                {
                    return CultureInfo.CreateSpecificCulture("en-US");
                }
                default:
                    throw new ArgumentException(
                        $"Country provided ({cellarCountry}) is not associated with a Specific Culture");
            }
        }

        public static List<string> GetPartsOfKey(AzureTableKey key = null)
        {
            return key == null ? new List<string>() : key.RowKey.Split("-").ToList();
        }

        /// <summary>
        /// Given a Leaf Key, create all of it's Ancestor Key
        /// </summary>
        /// <param name="ancestorPosition">Position in Ancestor tree of LeafKey</param>
        /// <param name="leafKey">Starting Key</param>
        /// <param name="keys">Collection of Ancestor Keys</param>
        /// <param name="path">List of Ancestor Entity names</param>
        private static void MakeAncestorKeys(int ancestorPosition, AzureTableKey leafKey,
            ref AzureTableEntityKeys keys, string[] path = null)
        {
            path ??= new string[0];

            var partKeys = GetPartsOfKey(leafKey);
            var EntityName = ancestorPosition < path.Length ? path[ancestorPosition] : string.Empty;

            for (var i = ancestorPosition; i < path.Length; i++)
                if (EntityName == path[i])
                {
                    //build a key for the ancestor
                    var partKey = string.IsNullOrEmpty(leafKey.PartitionKey) ? string.Empty : leafKey.PartitionKey;
                    var rowKey = "";
                    var bottleAdjustLength = partKeys.Count() > 4 ? ancestorPosition + 2 : ancestorPosition;
                    var startIndex = partKeys.Count() > 4 ? 1 : 0;
                    for (var j = startIndex; j < bottleAdjustLength; j++)
                        rowKey += rowKey.Length > 0 ? $"-{partKeys[j]}" : partKeys[j];

                    var key = new AzureTableKey {PartitionKey = partKey, RowKey = rowKey};
                    //pass it to ParseKeys
                    AddParsedEntityKey(EntityName, ref keys, key, ancestorPosition, leafKey, path);
                    return;
                }
        }
    }
}