using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;


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
        public async Task<IList<CellarSummaryModel>> GetCellarList()
        {
            var cellars = new List<CellarSummaryModel>();
            var cellarTable = TableClient.GetTableReference("Cellar");

            TableQuery<AzureTableCellarModel> query = new TableQuery<AzureTableCellarModel>();
            var cellarsBriefDetail = new List<CellarSummaryModel>();
            var aztCellars = await cellarTable.ExecuteQueryAsync(query);
            foreach (var aztCellar in aztCellars)
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
            foreach (var aztBottle in aztBottles)
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
                        if (aztWineSpec != null)
                        {
                            bottle.VarietalType = aztWineSpec.VarietalType;
                            bottle.Color = aztWineSpec.isRose ? "Rose" : bottle.Color;
                            bottle.IsDesert = aztWineSpec.isDessert;
                            var aztWinery = await GetAzureTableWinery(bottle.CountryOfOrigin, aztWineSpec.RowKey);
                            if (aztWinery != null)
                            {
                                bottle.WineryName = aztWinery.Name;
                                bottle.Region = aztWinery.Region;
                            }
                        }
                    }
                }

                Bottles.Add(bottle);
            }
            Bottles.Sort((x, y) => x.Vintage.CompareTo(y.Vintage));
            return Bottles.AsEnumerable();
        }

        public async Task<BottleDetailModel> GetCellarBottleDetails(AzureTableKey BottleId)
        {
            var bottle = new BottleDetailModel();
            bottle.EntityKeys = new AzureTableEntityKeys() { BottleId = BottleId };
            var bottleDetailQuery = new TableQuery<AzureTableBottleModel>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, BottleId.RowKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, BottleId.PartitionKey)));
            var aztBottle = (await TableClient.GetTableReference("Bottle").ExecuteQueryAsync(bottleDetailQuery)).FirstOrDefault();
            if (aztBottle != null)
            {
                bottle.BarCode = aztBottle.BarCode;
                bottle.PricePaid = aztBottle.PricePaid;
                bottle.CellarDate = aztBottle.CellarDate;
            }

            bottle = bottle.CellarDate.CompareTo(System.DateTime.MinValue) > 0 ? await GetBottleDetailsForWineBottle(bottle) : bottle;
            bottle = bottle.EntityKeys.WineBottleId != null ? await GetBottleDetailsForWine(bottle) : bottle;
            bottle = bottle.EntityKeys.WineId != null ? await GetBottleDetailsForWinery(bottle) : bottle;
            bottle = bottle.EntityKeys.WineId != null ? await GetBottleDetailForWineVarietals(bottle) : bottle;
            if(bottle.VarietalDetails.Count() > 0)
            {
                foreach (var varietalDetail in bottle.VarietalDetails)
                {
                    var vvd = await GetDetailsForWineVarietalVineyardVarietal(varietalDetail);
                    varietalDetail.VineyardDetail = vvd.VineyardDetail;                    
                }
            }

            //Get Details before wine spec because it can change color value to Rose
            bottle = bottle.EntityKeys.WineId != null ? await GetBottleDetailsForWineSpec(bottle) : bottle;

            return bottle;
        }

        private async Task<int> GetCellarBottleCount(AzureTableKey CellarId)
        {
            return (await GetAzureTableBottlesInCellar(CellarId)).Count();
        }
        private async Task<IList<AzureTableWineVarietalModel>> GetAzureTableWineVarietals(string Color, string RowKey)
        {
            return await TableClient.GetTableReference("WineVarietal").ExecuteQueryAsync(
                new TableQuery<AzureTableWineVarietalModel>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey",
                                QueryComparisons.GreaterThanOrEqual,
                                $"{RowKey}-"),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey",
                                QueryComparisons.LessThan, $"{RowKey}~")),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("PartitionKey",
                            QueryComparisons.Equal, Color)
                        )
                    ));
        }

        private async Task<IEnumerable<AzureTableBottleModel>> GetAzureTableBottlesInCellar(AzureTableKey CellarId)
        {
            return (await TableClient.GetTableReference("Bottle").ExecuteQueryAsync(
                        new TableQuery<AzureTableBottleModel>().Where(
                             TableQuery.CombineFilters(
                                 TableQuery.CombineFilters(
                                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, $"{CellarId.RowKey}-"),
                                    TableOperators.And,
                                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, $"{CellarId.RowKey}~")),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("PartitionKey",
                                QueryComparisons.Equal, $"{CellarId.PartitionKey}")))));
        }

        private async Task<AzureTableWineBottle> GetAzureTableWineBottle(AzureTableKey BottleId)
        {
            var wineBottleRowKey = BottleId.RowKey.Substring(BottleId.RowKey.IndexOf("-") + 1);
            wineBottleRowKey = wineBottleRowKey.Substring(0, wineBottleRowKey.LastIndexOf("-"));

            return (await TableClient.GetTableReference("WineBottle")
                .ExecuteQueryAsync(
                    new TableQuery<AzureTableWineBottle>().Where(
                        TableQuery.GenerateFilterCondition("RowKey", 
                            QueryComparisons.Equal, wineBottleRowKey)))
                ).FirstOrDefault();
        }

        private async Task<AzureTableWine> GetAzureTableWine(string PartitionKey, string WineBottleRowKey)
        {
            return (await TableClient.GetTableReference("Wine").ExecuteQueryAsync(
                new TableQuery<AzureTableWine>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey",QueryComparisons.Equal,
                        WineBottleRowKey.Substring(0,WineBottleRowKey.LastIndexOf("-"))),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("PartitionKey", 
                            QueryComparisons.Equal, PartitionKey))
                    )
                )
            ).FirstOrDefault();
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
            var result = await TableClient.GetTableReference("Varietal").ExecuteQueryAsync(
                    new TableQuery<AzureTableVarietalModel>().Where(
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("RowKey",
                                    QueryComparisons.Equal,
                                    VarietalRowKey),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("PartitionKey",
                                    QueryComparisons.Equal,
                                    PartitionKey))));

            return result.Count == 0 ? string.Empty : result.First().Name;
        }

        private async Task<BottleDetailModel> GetBottleDetailsForWineBottle(BottleDetailModel bottle)
        {
            var aztWineBottle = await GetAzureTableWineBottle(bottle.EntityKeys.BottleId);

            if (aztWineBottle != null)
            {
                bottle.EntityKeys.WineBottleId = new AzureTableKey() { PartitionKey = aztWineBottle.PartitionKey, RowKey = aztWineBottle.RowKey, TimeStamp = aztWineBottle.Timestamp };
                bottle.Size = aztWineBottle.Size;
                bottle.RetailPrice = aztWineBottle.RetailPrice;
                bottle.UPC = aztWineBottle.UPC;
                bottle.QuantityProduced = aztWineBottle.QuantityProduced.ToString();
                bottle.BottleNumber = aztWineBottle.BottleNumber.ToString();
            }
            return bottle;
        }

        private async Task<BottleDetailModel> GetBottleDetailsForWine(BottleDetailModel bottle)
        {
            var aztWine = await GetAzureTableWine(bottle.EntityKeys.WineBottleId.PartitionKey, bottle.EntityKeys.WineBottleId.RowKey);
            if (aztWine != null)
            {
                bottle.EntityKeys.WineId = new AzureTableKey() { PartitionKey = aztWine.PartitionKey, RowKey = aztWine.RowKey, TimeStamp = aztWine.Timestamp };
                bottle.WineName = aztWine.Name; ;
                bottle.WineProductLine = aztWine.ProductLine;
                bottle.Vintage = aztWine.Vintage;
                bottle.Color = aztWine.Color;
            }
            return bottle;
        }
        private async Task<BottleDetailModel> GetBottleDetailsForWinery(BottleDetailModel bottle)
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
            return bottle;
        }

        private async Task<WineVarietalDetailModel> GetDetailsForWineVarietalVineyardVarietal(WineVarietalDetailModel WineVarietal)
        {
            var vvd = new VineyardVarietalDetail();
            var wineVarietalVineyardVarietal = (await TableClient.GetTableReference("WineVarietalVineyardVarietal").ExecuteQueryAsync(
                new TableQuery<AzureTableWineVarietalVineyardVarietalModel>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey",
                                QueryComparisons.GreaterThanOrEqual,
                                $"{WineVarietal.WineVarietalId.RowKey}-"),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey",
                                QueryComparisons.LessThan, $"{WineVarietal.WineVarietalId.RowKey}~")),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("PartitionKey",
                            QueryComparisons.Equal, WineVarietal.WineVarietalId.PartitionKey)
                        )
                    ))).FirstOrDefault();
            if (wineVarietalVineyardVarietal != null)
            {
                vvd.VineyardPercentage = (wineVarietalVineyardVarietal.VineyardPercentage/100).ToString("P");
                vvd.VineyardVarietalId = new AzureTableKey()
                {
                    RowKey = wineVarietalVineyardVarietal.RowKey.Replace(WineVarietal.WineVarietalId.RowKey, "").Substring(1),
                    PartitionKey = WineVarietal.WineVarietalId.PartitionKey,
                    TimeStamp = wineVarietalVineyardVarietal.Timestamp
                };
                vvd = await GetVinyardVarietalDetail(vvd);
                WineVarietal.VineyardDetail = vvd;


            }
            return WineVarietal;
        }

        private async Task<BottleDetailModel> GetBottleDetailForWineVarietals(BottleDetailModel bottle)
            {
                var wineVarietalDetailsList = new List<WineVarietalDetailModel>();
                var aztWineVarietals = await GetAzureTableWineVarietals(bottle.Color, bottle.EntityKeys.WineId.RowKey);
                foreach (var aztWineVarietal in aztWineVarietals)
                {
                    var aztVarietalKey = aztWineVarietal.RowKey.Substring(aztWineVarietal.RowKey.LastIndexOf("-") + 1);
                    var wineVarietalDetail = new WineVarietalDetailModel()
                    {
                        WineVarietalId = new AzureTableKey() { PartitionKey = aztWineVarietal.PartitionKey, RowKey = aztWineVarietal.RowKey, TimeStamp = aztWineVarietal.Timestamp },
                        Percentage = aztWineVarietal.Percentage,
                        VarietalName = await GetAzurTableVarietalName(aztWineVarietal.PartitionKey, aztVarietalKey)
                    };
                    wineVarietalDetailsList.Add(wineVarietalDetail);
                }
                bottle.VarietalDetails = wineVarietalDetailsList.AsEnumerable();
                return bottle;
            }

        private async Task<BottleDetailModel> GetBottleDetailsForWineSpec(BottleDetailModel bottle)
        {
            var aztWineSpec = await GetAzureTableWineSpec(bottle.EntityKeys.WineId.PartitionKey, bottle.EntityKeys.WineId.RowKey);
            if (aztWineSpec != null)
            {
                bottle.WineVarietalType = aztWineSpec.VarietalType;
                bottle.Color = aztWineSpec.isRose ? "Rose" : bottle.Color;
                bottle.isDessert = aztWineSpec.isDessert;
                bottle.isEstate = aztWineSpec.isEstate;
                bottle.isRose = aztWineSpec.isRose;
                bottle.ResSugar = (aztWineSpec.ResSugar / 100).ToString("P");
                bottle.AlcPercent = (aztWineSpec.AlcPercent / 100).ToString("P");
                bottle.WineAppellation = aztWineSpec.Appellation;
            }
            return bottle;
        }

        private async Task<VineyardVarietalDetail> GetVinyardVarietalDetail(VineyardVarietalDetail vineyardVarietalDetail)
        {
            var vineyardVarietalItem = (await TableClient.GetTableReference("VineyardVarietal").ExecuteQueryAsync(
                    new TableQuery<AzureTableVineyardVarietalModel>().Where(
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, vineyardVarietalDetail.VineyardVarietalId.RowKey),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vineyardVarietalDetail.VineyardVarietalId.PartitionKey)
                                )))).FirstOrDefault();

            if (vineyardVarietalItem != null)
            {
                vineyardVarietalDetail.CloneName = vineyardVarietalItem.CloneName;
            }
            else
            {
                return vineyardVarietalDetail;
            }

            var vineyardEqualCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, vineyardVarietalItem.RowKey.Substring(0, vineyardVarietalItem.RowKey.IndexOf("-")));
            var vineyardQuery = new TableQuery<AzureTableVineyardModel>().Where(vineyardEqualCond);
            var aztVineyard = await TableClient.GetTableReference("Vineyard").ExecuteQueryAsync(
                        new TableQuery<AzureTableVineyardModel>().Where(
                            vineyardEqualCond
                        ));

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

            return vineyardVarietalDetail;
        }
    }
}
