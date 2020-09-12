using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MyWineDb.Api
{
    public static class GetBottleTree
    {
        [FunctionName("GetBottleTree")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing GetBottleTree request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var aztkString = new StreamReader(stream).ReadToEnd();
            var key = JsonConvert.DeserializeObject<AzureTableKey>(aztkString);
            if (key == null)
            {
                log.LogError($"Failed to Retireve Bottle Tree for Azure Table Key provided: {aztkString}");
                return new StatusCodeResult(400);
            }

            log.LogInformation("GetBottleTree Api Request initiated");
            var dataStore = new DataStore(log, context);
            var bottles = await dataStore.GetCellarSummaryBottles(key);
            if (bottles.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to get bottles from DataStore.");
                return (IActionResult)bottles;
            }

            var treeNodes = new List<IBottleTreeNode>();
            foreach(var bottle in bottles)
            {
                var vinetageNode = treeNodes.Where(v => v.Title == bottle.Vintage.ToString()).FirstOrDefault();
                if(vinetageNode == null)
                {
                    vinetageNode = new BottleTreeNode() { Title = bottle.Vintage.ToString(), IsParent = true, ChildNodes = new List<IBottleTreeNode>().AsEnumerable() };
                    treeNodes.Add(vinetageNode);
                }

                var vinetageNodeChildeNodes = vinetageNode.ChildNodes; //colors
                var colors = vinetageNodeChildeNodes.ToList();
                var color = colors.Where(c => c.Title == bottle.Color).FirstOrDefault();
                if (color == null)
                {
                    color = new BottleTreeNode() { Title = bottle.Color, IsParent = true, ChildNodes = new List<IBottleTreeNode>().AsEnumerable() };
                    colors.Add(color);
                }

                var colorChildNodes = color.ChildNodes; //wine Type
                var wineTypes = colorChildNodes.ToList();
                var wineType = wineTypes.Where(w => w.Title == bottle.VarietalType).FirstOrDefault();
                if(wineType == null)
                {
                    wineType = new BottleTreeNode() { Title = bottle.VarietalType, IsParent = true, ChildNodes = new List<IBottleTreeNode>().AsEnumerable() }; ;
                    wineTypes.Add(wineType);
                }


                var wineTypeChildNodes = wineType.ChildNodes; //countryRegion
                var countryRegions = wineTypeChildNodes.ToList();
                var countryRegion = countryRegions.Where(r => r.Title == $"{bottle.CountryOfOrigin}-{bottle.Region}").FirstOrDefault();
                if(countryRegion == null)
                {
                    countryRegion = new BottleTreeNode() { Title = $"{bottle.CountryOfOrigin}-{bottle.Region}", IsParent = true, ChildNodes = new List<IBottleTreeNode>().AsEnumerable() };
                    countryRegions.Add(countryRegion);
                }

                var countryRegionChildNodes = countryRegion.ChildNodes; //wineries
                var wineries = countryRegionChildNodes.ToList();
                var winery = wineries.Where(w => w.Title == bottle.WineryName).FirstOrDefault();
                if(winery == null || wineries.Count() == 0)
                {
                    winery = new BottleTreeNode() { Title = bottle.WineryName, IsParent = true, ChildNodes = new List<IBottleTreeNode>().AsEnumerable() }; 
                    wineries.Add(winery);
                }

                var wineryChildNodes = winery.ChildNodes; //wines
                var wines = wineryChildNodes.ToList();
                var wine = wines.Where(w => w.Title == bottle.WineName).FirstOrDefault();
                if(wine == null || wines.Count() == 0)
                {                
                    wine = new BottleTreeNode() { Title = bottle.WineName, IsParent = true, ChildNodes = new List<BottleTreeLeafNode>().AsEnumerable() };
                    wines.Add(wine);
                }

                var wineChildNodes = wine.ChildNodes; //wine Bottles
                var wineBottles = ((IEnumerable<BottleTreeLeafNode>)wineChildNodes).ToList();
                var wineBottle = wineBottles.Where(b => b.WineBottleId.RowKey == bottle.BottleId.RowKey).FirstOrDefault();
                if(wineBottle == null)
                {
                    wineBottle = new BottleTreeLeafNode() { Title = bottle.BarCode, IsParent = false, Count = 1,  WineBottleId = bottle.BottleId };
                    wineBottles.Add(wineBottle);
                }

                wineChildNodes = wineBottles.AsEnumerable();
                wine.ChildNodes = wineChildNodes;
                wine.Count = wineBottles.Count();
                wineryChildNodes = wines.AsEnumerable();
                winery.ChildNodes = wineryChildNodes;
                winery.Count = winery.ChildNodes.Sum(w => w.Count);
                countryRegionChildNodes = wineries.AsEnumerable();
                countryRegion.ChildNodes = countryRegionChildNodes;
                countryRegion.Count = countryRegion.ChildNodes.Sum(c =>c.Count);
                wineTypeChildNodes = countryRegions.AsEnumerable();
                wineType.ChildNodes = wineTypeChildNodes;
                wineType.Count = wineType.ChildNodes.Sum(c => c.Count);
                colorChildNodes = wineTypes.AsEnumerable();
                color.ChildNodes = colorChildNodes.AsEnumerable();
                color.Count = color.ChildNodes.Sum(c => c.Count);
                vinetageNodeChildeNodes = colors.AsEnumerable();
                vinetageNode.ChildNodes = vinetageNodeChildeNodes;
                vinetageNode.Count = vinetageNode.ChildNodes.Sum(v => v.Count);
            }

            var resultStr = JsonConvert.SerializeObject(treeNodes);
            log.LogInformation("GetCellarSummaryBottles Api Request completed");
            return new OkObjectResult(resultStr);
        }
    }
}
