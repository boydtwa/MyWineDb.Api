using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyWineDb.Api
{
    public static class GetInventoryReportHtml
    {
        private static DataStore CloudData {get; set;}
        [FunctionName("GetInventoryReportHtml")]
        public static async Task<HttpResponseMessage> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    ILogger log, ExecutionContext context)
        {
            log.LogInformation("GetInventoryReportHtml Api Request initiated");
            CloudData = new DataStore(log, context);
            var enumeratedCellars = await CloudData.GetCellarList();
            if (enumeratedCellars.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to get list of Cellars.");
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Failed to get list of Cellars") };
            }
            var report = new InventoryReportModel();
            report.DateOfReport = DateTime.Now;

            var inventoryCellars = new List<InventoryCellarModel>();
            var cellarBottleCount = 0;
            foreach (var cellar in enumeratedCellars)
            {
                var inventoryCellar = await ProcessCellar(cellar);
                inventoryCellars.Add(inventoryCellar);
            }
            report.TotalNumberOfBottles = cellarBottleCount;
            report.CellarsTotalValue = inventoryCellars.Sum(cv => cv.CellarTotalValue);
            report.AveBottleValue = cellarBottleCount > 0 ? report.CellarsTotalValue / cellarBottleCount : 0;
            report.Cellars = inventoryCellars.AsEnumerable();

            // make Html
            var strB = new StringBuilder();

            strB.Append(ReportPieces.Part1);
            strB.Append(ReportPieces.SummaryData(
                report.DateOfReport.ToString("d"), string.Format("{0:0,0.00}",report.CellarsTotalValue), 
                report.TotalNumberOfBottles.ToString(), string.Format("{0:0,0.00}", report.AveBottleValue)));
            foreach(var c in report.Cellars)
            {
                strB.Append(ReportPieces.CellarHeader(c.CellarName, c.Capacity.ToString(), (c.PctCapacity * 100).ToString("P")));
                foreach(var v in c.Vintages)
                {
                    strB.Append(ReportPieces.VintageHeader(v.VinetageYear.ToString()));
                    strB.Append(ReportPieces.CellarLineHeader());
                    foreach(var i in v.WineItems)
                    {
                        strB.Append(ReportPieces.CellarLineItem(i.Quantity.ToString(), i.Size.ToString(),
                            i.WineryName, i.Region, i.Country, i.WineName, i.Color, i.VarietalType, string.Format("{0:0,0.00}", i.UnitValue),
                            string.Format("{0:0,0.00}", i.TotalValue)));
                    }
                    strB.Append("</div></div>"); //close TableBody for each vintage
                }
                strB.Append("</div>"); //close  divTable blueTable
                strB.Append(ReportPieces.CellarFooter(string.Format("{0:0,0.00}",c.CellarTotalValue)));
            }
            strB.Append(ReportPieces.EndOfPage);
            log.LogInformation("GetInventoryReportData Api Request completed");
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(strB.ToString()) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html") { CharSet = "UTF-8" };
               
            return response;
        }

        private static async Task<InventoryCellarModel> ProcessCellar(CellarSummaryModel cellar)
        {
            var inventoryCellar = new InventoryCellarModel()
            {
                CellarName = cellar.Name,
                Capacity = cellar.Capacity,
                BottleCount = 0
            };

            inventoryCellar = await ProcessBottles(inventoryCellar, cellar);
            return inventoryCellar;
        }

        private static async Task<InventoryCellarModel> ProcessBottles(InventoryCellarModel InventoryCellar, CellarSummaryModel Cellar)
        {
            var vintageList = new List<InventoryVintageModel>();
            var WineItemList = new List<InventoryWineItemModel>();
            var VintageListItem = new InventoryVintageModel();
            var GetInventoryReportData = await CloudData.GetCellarSummaryBottles(Cellar.CellarId);

            if (GetInventoryReportData.GetType() == typeof(List<BottleBriefDataModel>))
            {
                Cellar.Bottles = GetInventoryReportData;
            }
            foreach(var bottle in Cellar.Bottles)
            {
                InventoryCellar.BottleCount++;
                var vintageListItem = (from yearItem in vintageList
                                       where yearItem.VinetageYear == bottle.Vintage
                                       select yearItem).FirstOrDefault();
                if (vintageListItem != null && vintageListItem.WineItems != null)
                {
                    //already have a collection established for the vintage with at least
                    //one WineItem in it.
                    VintageListItem = vintageListItem;
                    WineItemList = VintageListItem.WineItems.ToList();
                }
                else
                {
                    //need to create the Vintage List Item object
                    var newVintageItem = new InventoryVintageModel()
                    {
                        VinetageYear = bottle.Vintage,
                        WineItems = new List<InventoryWineItemModel>().AsEnumerable()
                    };
                    VintageListItem = newVintageItem; //change scope in case of another bottle of same type
                    vintageList.Add(VintageListItem);
                    WineItemList = VintageListItem.WineItems.ToList();
                }
                var wineBottleRowKey = bottle.BottleId.RowKey.Substring(bottle.BottleId.RowKey.IndexOf("-") + 1);
                wineBottleRowKey = wineBottleRowKey.Substring(0, wineBottleRowKey.LastIndexOf("-"));
                var itemInList = (from item in WineItemList
                                  where item.WineBottleRowKey == wineBottleRowKey
                                  select item).FirstOrDefault();
                if (itemInList != null)
                {
                    //at least one other bottle exists in List
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
                    //first bottle of this type
                    var newItem = new InventoryWineItemModel()
                    {
                        Quantity = 1,
                        Size = bottle.Size,
                        WineryName = bottle.WineryName,
                        WineName = bottle.WineName,
                        Country = bottle.CountryOfOrigin,
                        Region = bottle.Region,
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
                InventoryCellar.Vintages = vintageList.AsEnumerable();
                InventoryCellar.CellarTotalValue = InventoryCellar.Vintages.Sum(cv => cv.VinetageValue);
            }
            return InventoryCellar;
        }
    }
}
