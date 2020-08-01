using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using MyWineDb.Api.Models;
using System.Collections.Generic;
using System.Linq;
using MyWindDb.Api.Models;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace MyWineDb.Api.Services
{
    public class DataStore : IDataStore
    {
        private CloudTableClient TableClient { get; set; }
        public DataStore(ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation($"Attempting AzureApi Connection");
            var constr = config["DbConnectViaKeyVault"];
            CloudStorageAccount storeAccount =
                CloudStorageAccount.Parse(constr);
            TableClient = storeAccount.CreateCloudTableClient();
            log.LogInformation($"AzureApi Connection Established");
        }
        public async Task<IList<CellarSummaryModel>>GetCellarList()
        {
            var cellars = new List<CellarSummaryModel>();
            var cellarTable = TableClient.GetTableReference("Cellar");

            TableQuery<AzureTableCellarModel> query = new TableQuery<AzureTableCellarModel>();
            var cellarsBriefDetail = new List<CellarSummaryModel>();
            var aztCellars = await cellarTable.ExecuteQueryAsync(query);
            foreach(var aztCellar in aztCellars)
            {
                var cellar = new CellarSummaryModel()
                {
                    CellarId = new AzureTableKey()
                    {
                        RowKey = aztCellar.RowKey,
                        PartitionKey = aztCellar.PartitionKey,
                        TimeStamp = aztCellar.Timestamp
                    },
                    Name = aztCellar.Name,
                    Description = aztCellar.Description,
                    Capacity = aztCellar.Capacity
                };
                cellar.BottleCount = await GetCellarBottleCount(cellar.CellarId);
                cellars.Add(cellar);
            }

            return cellars;
        }

        public async Task<IEnumerable<BottleBriefDataModel>> GetCellarSummaryBottles(AzureTableKey CellarId)
        {
            var Bottles = new List<BottleBriefDataModel>();
            var aztBottles = await GetAzureTableBottlesInCellar(CellarId);
            foreach(var aztBottle in aztBottles)
            {
                var bottle = new BottleBriefDataModel()
                {
                    BottleId = new AzureTableKey { PartitionKey = aztBottle.PartitionKey, RowKey = aztBottle.RowKey, TimeStamp = aztBottle.Timestamp },
                    BarCode = aztBottle.BarCode,
                    PricePaid = aztBottle.PricePaid
                };

                var aztWineBottle = await GetAzureTableWineBottle(bottle.BottleId);
                if (aztWineBottle != null)
                {
                    bottle.CountryOfOrigin = aztWineBottle.PartitionKey;
                    bottle.Size = aztWineBottle.Size;
                    bottle.RetailPrice = aztWineBottle.RetailPrice;
                    var aztWine = await GetAzureTableWine(bottle.CountryOfOrigin, aztWineBottle.RowKey);
                    if (aztWine != null)
                    {
                        bottle.WineName = aztWine.Name;
                        bottle.ProductLine = aztWine.ProductLine;
                        bottle.Vintage = aztWine.Vintage;
                        bottle.Color = aztWine.Color;
                        var aztWineSpec = await GetAzureTableWineSpec(bottle.CountryOfOrigin, aztWine.RowKey);
                        if(aztWineSpec != null)
                        {
                            bottle.VarietalType = aztWineSpec.VarietalType;
                            bottle.Color = aztWineSpec.isRose ? "Rose" : bottle.Color;
                            bottle.IsDesert = aztWineSpec.isDessert;
                            var aztWinery = await GetAzureTableWinery(bottle.CountryOfOrigin, aztWineSpec.RowKey);
                            if(aztWinery != null)
                            {
                                bottle.WineryName = aztWinery.Name;
                            }
                        }
                    }
                }
                Bottles.Add(bottle);
            }

            return Bottles.AsEnumerable();
        }

        public async Task<BottleDetailModel> GetCellarBottleDetails(AzureTableKey BottleId)
        {
            var bottle = new BottleDetailModel();
            bottle.EntityKeys = new AzureTableEntityKeys() {BottleId = BottleId};
            var rowEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, BottleId.RowKey);
            var partEqualCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, BottleId.PartitionKey);
            var combinedFilter = TableQuery.CombineFilters(rowEqualCond, TableOperators.And, partEqualCond);
            var bottleDetailQuery = new TableQuery<AzureTableBottleModel>().Where(combinedFilter);
            var aztBottle = (await TableClient.GetTableReference("Bottle").ExecuteQueryAsync(bottleDetailQuery)).FirstOrDefault();
            if (aztBottle != null)
            {

                bottle.BarCode = aztBottle.BarCode;
                bottle.PricePaid = aztBottle.PricePaid;
                bottle.CellarDate = aztBottle.CellarDate;
            }
            var aztWineBottle = await GetAzureTableWineBottle(BottleId);
            if (aztBottle != null && aztWineBottle != null)
                {
                bottle.EntityKeys.WineBottleId = new AzureTableKey() { PartitionKey = aztWineBottle.PartitionKey, RowKey = aztWineBottle.RowKey, TimeStamp = aztWineBottle.Timestamp };
                bottle.Size = aztWineBottle.Size;
                bottle.RetailPrice = aztWineBottle.RetailPrice;
                bottle.UPC = aztWineBottle.UPC;
                bottle.QuantityProduced = aztWineBottle.QuantityProduced.ToString();
                bottle.BottleNumber = aztWineBottle.BottleNumber.ToString();
            }
            var color = string.Empty;
            if (bottle.EntityKeys.WineBottleId != null)
            {
                var aztWine = await GetAzureTableWine(aztWineBottle.PartitionKey, aztWineBottle.RowKey);
                if (aztWine != null)
                {
                    bottle.EntityKeys.WineId = new AzureTableKey() { PartitionKey = aztWine.PartitionKey, RowKey = aztWine.RowKey, TimeStamp = aztWine.Timestamp };
                    bottle.WineName = aztWine.Name; ;
                    bottle.WineProductLine = aztWine.ProductLine;
                    bottle.Vintage = aztWine.Vintage;
                    bottle.Color = color = aztWine.Color;
                    var aztWineSpec = await GetAzureTableWineSpec(aztWine.PartitionKey, aztWine.RowKey);
                    if (aztWineSpec != null)
                    {
                        bottle.WineVarietalType = aztWineSpec.VarietalType;
                        bottle.Color = aztWineSpec.isRose ? "Rose" : bottle.Color;
                        bottle.isDessert = aztWineSpec.isDessert;
                        bottle.isEstate = aztWineSpec.isEstate;
                        bottle.isRose = aztWineSpec.isRose;
                        bottle.ResSugar = (aztWineSpec.ResSugar/100).ToString("P");
                        bottle.AlcPercent = (aztWineSpec.AlcPercent/100).ToString("P");
                        bottle.WineAppellation = aztWineSpec.Appellation;

                    }
                }
            }
            if(bottle.EntityKeys.WineId != null)
            { 
                var aztWinery = await GetAzureTableWinery(bottle.EntityKeys.WineId.PartitionKey, bottle.EntityKeys.WineId.RowKey);
                if (aztWinery != null)
                {
                    bottle.EntityKeys.WineryId = new AzureTableKey() { PartitionKey = aztWinery.PartitionKey, RowKey = aztWinery.RowKey, TimeStamp = aztWinery.Timestamp };
                    bottle.WineryName = aztWinery.Name;
                    bottle.WineryRegion = aztWinery.Region;
                    bottle.WineryAddress = aztWinery.Address;
                    bottle.WineryGeoCoordinates = aztWinery.GeoCoordinates;
                }
            }
            var wineVarietalDetailsList = new List<WineVarietalDetailModel>();
            if(!string.IsNullOrEmpty(color) && bottle.EntityKeys.WineId != null)
            {
                //check for varietal info
                var aztWineVarietals = await GetBottleDetailWineVarietals(color, bottle.EntityKeys.WineId.RowKey);
                foreach(var aztWineVarietal in aztWineVarietals)
                {
                    var aztVarietalKey = aztWineVarietal.RowKey.Substring(aztWineVarietal.RowKey.LastIndexOf("-")+1);
                    var wineVarietalDetail = new WineVarietalDetailModel()
                    {
                        WineVarietalId = new AzureTableKey() { PartitionKey = aztWineVarietal.PartitionKey, RowKey = aztWineVarietal.RowKey, TimeStamp = aztWineVarietal.Timestamp },
                        Percentage = aztWineVarietal.Percentage,
                        VarietalName = await GetAzurTableVarietalName(aztWineVarietal.PartitionKey, aztVarietalKey)
                    };
                    wineVarietalDetailsList.Add(wineVarietalDetail);
                }
            }
            if(wineVarietalDetailsList.Count > 0)
            {
                foreach(var item in wineVarietalDetailsList)
                {
                    item.VineyardDetail = await GetVinyardVarietalDetail(item.WineVarietalId);                    
                }
                bottle.VarietalDetails = wineVarietalDetailsList.AsEnumerable();
            }

            return bottle;
        }

        private async Task<int>GetCellarBottleCount(AzureTableKey CellarId)
        {
            return (await GetAzureTableBottlesInCellar(CellarId)).Count();
        }

        private async Task<IList<AzureTableWineVarietalModel>> GetBottleDetailWineVarietals(string PartitionKey, string WineIdRowKey)
        {
            var lowerKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, $"{WineIdRowKey}-");
            var upperKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, $"{WineIdRowKey}~");
            var rowKeyCombineFilter = TableQuery.CombineFilters(lowerKeyCond, TableOperators.And, upperKeyCond);
            var partEqualKeyCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
            var combineFilter = TableQuery.CombineFilters(rowKeyCombineFilter, TableOperators.And, partEqualKeyCond);

            var wineVarietalQuery = new TableQuery<AzureTableWineVarietalModel>().Where(combineFilter);

            var result =  await TableClient.GetTableReference("WineVarietal").ExecuteQueryAsync(wineVarietalQuery);
            return result;
        }

        private async Task<IEnumerable<AzureTableBottleModel>> GetAzureTableBottlesInCellar(AzureTableKey CellarId)
        {
            var lowerKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, $"{CellarId.RowKey}-");
            var upperKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, $"{CellarId.RowKey}~");

            var combinedFilter = TableQuery.CombineFilters(lowerKeyCond, TableOperators.And, upperKeyCond);
            var bottleQuery = new TableQuery<AzureTableBottleModel>().Where(combinedFilter);
            var result = await TableClient.GetTableReference("Bottle").ExecuteQueryAsync(bottleQuery);
            return result;
        }

        //private async Task<AzureTableBottleModel> GetAzureTableBottle(AzureTableKey BottleId)
        //{
        //    var keyEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, BottleId.RowKey);
        //    var partitionEqualCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, BottleId.PartitionKey);

        //    var combinedFilter = TableQuery.CombineFilters(keyEqualCond, TableOperators.And, partitionEqualCondition);
        //    var bottleQuery = new TableQuery<AzureTableBottleModel>().Where(combinedFilter);

        //    return (await TableClient.GetTableReference("Bottle").ExecuteQueryAsync(bottleQuery)).FirstOrDefault();
        //}

        private async Task<AzureTableWineBottle> GetAzureTableWineBottle(AzureTableKey BottleId)
        {
            var wineBottleRowKey = BottleId.RowKey.Substring(BottleId.RowKey.IndexOf("-") + 1);
            wineBottleRowKey = wineBottleRowKey.Substring(0, wineBottleRowKey.LastIndexOf("-"));

            var equalCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, wineBottleRowKey);
            var wineBottleQuery = new TableQuery<AzureTableWineBottle>().Where(equalCond);
            var result = await TableClient.GetTableReference("WineBottle").ExecuteQueryAsync(wineBottleQuery);

            return result.FirstOrDefault();
        }

        private async Task<AzureTableWine> GetAzureTableWine(string PartitionKey, string WineBottleRowKey)
        {
            var wineKey = WineBottleRowKey.Substring(0, WineBottleRowKey.LastIndexOf("-"));
            var rowEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, wineKey);
            var partEqualCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
            var combinedFilter = TableQuery.CombineFilters(rowEqualCond, TableOperators.And, partEqualCond);
            var wineQuery = new TableQuery<AzureTableWine>().Where(combinedFilter);
            var result = await TableClient.GetTableReference("Wine").ExecuteQueryAsync(wineQuery);

            return result.FirstOrDefault();
        }
        private async Task<AzureTableWineSpec> GetAzureTableWineSpec(string PartitionKey, string WineRowKey)
        {
            var rowEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, WineRowKey);
            var partEqualCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
            var combinedFilter = TableQuery.CombineFilters(rowEqualCond, TableOperators.And, partEqualCond);
            var wineSpecQuery = new TableQuery<AzureTableWineSpec>().Where(combinedFilter);
            var result = await TableClient.GetTableReference("WineSpec").ExecuteQueryAsync(wineSpecQuery);

            return result.FirstOrDefault();
        }
        private async Task<AzureTableWinery> GetAzureTableWinery(string PartitionKey, string WineRowKey)
        {
            var wineryKey = WineRowKey.Substring(0, WineRowKey.LastIndexOf("-"));
            var rowEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, wineryKey);
            var partEqualCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
            var combinedFilter = TableQuery.CombineFilters(rowEqualCond, TableOperators.And, partEqualCond);
            var wineryQuery = new TableQuery<AzureTableWinery>().Where(combinedFilter);
            var result = await TableClient.GetTableReference("Winery").ExecuteQueryAsync(wineryQuery);

            return result.FirstOrDefault();
        }

        private async Task<string> GetAzurTableVarietalName(string PartitionKey, string VarietalRowKey)
        {
            var rowEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, VarietalRowKey);
            var partEqualCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey);
            var combinedFilter = TableQuery.CombineFilters(rowEqualCond, TableOperators.And, partEqualCond);
            var varietalQuery = new TableQuery<AzureTableVarietalModel>().Where(combinedFilter);
            var result = await TableClient.GetTableReference("Varietal").ExecuteQueryAsync(varietalQuery);

            return result.FirstOrDefault() == null ? string.Empty : result.First().Name;
        }

        private async Task<VineyardVarietalDetail> GetVinyardVarietalDetail(AzureTableKey WineVarietalId)
        {
            var vineyardVarietalDetail = new VineyardVarietalDetail();
            var varietalVineyardKey = string.Empty;
            var vineyardKey = string.Empty;

            var lowerKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, $"{WineVarietalId.RowKey}-");
            var upperKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, $"{WineVarietalId.RowKey}-~");
            var partEqualCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, WineVarietalId.PartitionKey);

            var combinedKey = TableQuery.CombineFilters(lowerKeyCond, TableOperators.And, upperKeyCond);
            var combineFilter = TableQuery.CombineFilters(combinedKey, TableOperators.And, partEqualCond);

            var wineVarietalVineyardVarietalQuery = new TableQuery<AzureTableWineVarietalVineyardVarietalModel>().Where(combineFilter);
            var aztWineVarietalVineyardVariatleTable = await TableClient.GetTableReference("WineVarietalVineyardVarietal").ExecuteQueryAsync(wineVarietalVineyardVarietalQuery);

            if (aztWineVarietalVineyardVariatleTable.Count > 0)
            {
                vineyardVarietalDetail.VineyardPercentage = (aztWineVarietalVineyardVariatleTable[0].VineyardPercentage/100).ToString("P");
                varietalVineyardKey = aztWineVarietalVineyardVariatleTable[0].RowKey.Replace($"{WineVarietalId.RowKey}-", string.Empty);
            }
            else
            {
                return vineyardVarietalDetail;
            }

            if (!string.IsNullOrEmpty(varietalVineyardKey))
            {
                var vvvEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, varietalVineyardKey);
                var combineVvFilter = TableQuery.CombineFilters(vvvEqualCond, TableOperators.And, partEqualCond);
                var vineyardVarietalQuery = new TableQuery<AzureTableVineyardVarietalModel>().Where(combineVvFilter);
                var aztVineyardVarietalTable = await TableClient.GetTableReference("VineyardVarietal").ExecuteQueryAsync(vineyardVarietalQuery);
                if(aztVineyardVarietalTable.Count >0)
                {
                    vineyardVarietalDetail.CloneName = aztVineyardVarietalTable[0].CloneName;
                    vineyardKey = aztVineyardVarietalTable[0].RowKey.Substring(0, aztVineyardVarietalTable[0].RowKey.IndexOf("-"));
                }
                else
                {
                    return vineyardVarietalDetail;
                }
            }
            if (!string.IsNullOrEmpty(vineyardKey))
            {
                var vineyardEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, vineyardKey);
                var vineyardQuery = new TableQuery<AzureTableVineyardModel>().Where(vineyardEqualCond);
                var aztVineyard = await TableClient.GetTableReference("Vineyard").ExecuteQueryAsync(vineyardQuery);
                if(aztVineyard.Count > 0)
                {
                    vineyardVarietalDetail.VineyardName = aztVineyard[0].Name;
                    vineyardVarietalDetail.Region = aztVineyard[0].Region;
                    vineyardVarietalDetail.VineyardAppellation = aztVineyard[0].Appellation;
                    vineyardVarietalDetail.Address = aztVineyard[0].Address;
                    vineyardVarietalDetail.PhoneNumber = aztVineyard[0].PhoneNumber;
                    vineyardVarietalDetail.Email = aztVineyard[0].Email;
                    vineyardVarietalDetail.GeoCoordinates = aztVineyard[0].GeoCoordinates;
                }
                else
                {
                    return vineyardVarietalDetail;
                }
            }

            return vineyardVarietalDetail;
        }
    }
}
