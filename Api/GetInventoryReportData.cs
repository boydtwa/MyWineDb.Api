using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MyWineDb.Api
{
    public static class GetInventoryReportData
    {

        [FunctionName("GetInventoryReportData")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing GetInventoryReportData request");
            //var stream = req.Body;
            //stream.Seek(0, SeekOrigin.Begin);
            //var aztkString = new StreamReader(stream).ReadToEnd();
            //var key = JsonConvert.DeserializeObject<AzureTableKey>(aztkString);
            //if (key == null)
            //{
            //    log.LogError($"Failed to Retireve Bottles for Azure Table Key provided: {aztkString}");
            //    return new StatusCodeResult(400);
            //}

            log.LogInformation("GetInventoryReportData Api Request initiated");
            var dataStore = new DataStore(log, context);
            var enumeratedCellars = await dataStore.GetCellarList();
            if (enumeratedCellars.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to get list of Cellars.");
                return (IActionResult)enumeratedCellars;
            }
            var report = new InventoryReportModel();
            var inventoryCellars = new List<InventoryCellarModel>();

            report.DateOfReport = DateTime.Now;
            var cellarBottleCount = 0;
            foreach (var cellar in enumeratedCellars)
            {
                var GetInventoryReportData = await dataStore.GetCellarSummaryBottles(cellar.CellarId);

                if (GetInventoryReportData.GetType() == typeof(List<BottleBriefDataModel>))
                {
                    cellar.Bottles = GetInventoryReportData;
                }
                var reportCellar = new InventoryCellarModel()
                {
                    CellarName = cellar.Name,
                    Capacity = cellar.Capacity,
                };
                var vintageList = new List<InventoryVintageModel>();

                var WineItemList = new List<InventoryWineItemModel>();
                var VintageListItem = new InventoryVintageModel();
                foreach(var bottle in cellar.Bottles)
                {
                    cellarBottleCount++;
                    var vintageListItem = (from yearItem in vintageList
                                           where yearItem.VinetageYear == bottle.Vintage
                                           select yearItem).FirstOrDefault();
                    if (vintageListItem != null && vintageListItem.WineItems != null)
                    {
                        VintageListItem = vintageListItem;
                        WineItemList = VintageListItem.WineItems.ToList();
                    }
                    else
                    {
                        var newVintageItem = new InventoryVintageModel()
                        {
                            VinetageYear = bottle.Vintage,
                            WineItems = new List<InventoryWineItemModel>().AsEnumerable()
                        };

                        VintageListItem = newVintageItem;
                        vintageList.Add(VintageListItem);
                        WineItemList = VintageListItem.WineItems.ToList();
                    }
                    var wineBottleRowKey = bottle.BottleId.RowKey.Substring(bottle.BottleId.RowKey.IndexOf("-") + 1);
                    wineBottleRowKey = wineBottleRowKey.Substring(0, wineBottleRowKey.LastIndexOf("-"));
                    var itemInList = (from item in WineItemList
                                      where item.WineBottleRowKey == wineBottleRowKey
                                      select item).FirstOrDefault();
                    if(itemInList != null)
                    {
                        itemInList.Quantity++;
                        itemInList.UnitValue = itemInList.UnitValue < bottle.PricePaid ? bottle.PricePaid :
                            itemInList.UnitValue < bottle.RetailPrice ? bottle.RetailPrice : itemInList.UnitValue;
                        itemInList.TotalValue = itemInList.UnitValue * itemInList.Quantity;
                        if (!string.IsNullOrEmpty(bottle.BarCode))
                        {
                            var barCodeList = itemInList.BarCodes.ToList();
                            barCodeList.Add(bottle.BarCode);
                            itemInList.BarCodes = barCodeList.AsEnumerable();
                        }
                    }
                    else
                    {
                        var newItem = new InventoryWineItemModel()
                        {
                            Quantity = 1,
                            Size = bottle.Size,
                            WineryName = bottle.WineryName,
                            Color = bottle.Color,
                            VarietalType = bottle.VarietalType,
                            UnitValue = bottle.RetailPrice > 0 ? bottle.PricePaid > bottle.RetailPrice ? bottle.PricePaid : bottle.RetailPrice : 30d,
                            TotalValue = bottle.RetailPrice > 0 ? bottle.PricePaid > bottle.RetailPrice ? bottle.PricePaid : bottle.RetailPrice : 30d,
                            WineBottleRowKey = wineBottleRowKey
                        };
                        if (!string.IsNullOrEmpty(bottle.BarCode))
                        {
                            var btlBarCodeList = new List<string>() { bottle.BarCode };
                            newItem.BarCodes = btlBarCodeList.AsEnumerable();
                        }
                        WineItemList.Add(newItem);
                    }
                    VintageListItem.WineItems = WineItemList.AsEnumerable();
                    VintageListItem.VinetageValue = WineItemList.Sum(tv => tv.TotalValue);
                }
                reportCellar.Vintages = vintageList.AsEnumerable();
                reportCellar.CellarTotalValue = reportCellar.Vintages.Sum(cv => cv.VinetageValue);
                inventoryCellars.Add(reportCellar);
            }
            report.CellarsTotalValue = inventoryCellars.Sum(cv => cv.CellarTotalValue);
            report.AveBottleValue = cellarBottleCount > 0 ? report.CellarsTotalValue / cellarBottleCount : 0;
            report.Cellars = inventoryCellars.AsEnumerable();
            var resultStr = JsonConvert.SerializeObject(report);
            log.LogInformation("GetInventoryReportData Api Request completed");
            return new OkObjectResult(resultStr);
        }

    }
}
