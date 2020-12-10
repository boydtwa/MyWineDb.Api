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

namespace MyWineDb.Api
{
    public static class Cellar
    {
        [FunctionName("Cellar")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing Cellar request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var cellarString = new StreamReader(stream).ReadToEnd();
            var key = JsonConvert.DeserializeObject<AzureTableKey>(cellarString);
            if (key == null)
            {
                log.LogError($"Failed to Retireve a valid Key from Request: {cellarString}");
                return new StatusCodeResult(400);
            }
            log.LogInformation("GetCellarRow Request initiated");
            var dataStore = new AzureTableStore(log, context);
            var result = await dataStore.GetCellarRow(key);
            if (result.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to get Cellar record from DataStore.");
                return (IActionResult)result;
            }
            var resultStr = JsonConvert.SerializeObject(result);
            log.LogInformation("GetCellarRow Request completed");
            return new OkObjectResult(resultStr);
        }
    }

    public static class UpsertCellar
    {
        [FunctionName("UpsertCellar")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing UpsertCellar request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var cellarString = new StreamReader(stream).ReadToEnd();
            var cellarRow = JsonConvert.DeserializeObject<AzureTableCellarModel>(cellarString);
            if (cellarRow == null)
            {
                log.LogError($"Failed to Retireve a valid AzureTableCellarModel from Request: {cellarString}");
                return new StatusCodeResult(400);
            }
            log.LogInformation("UpsertCellar Request initiated");
            var dataStore = new AzureTableStore(log, context);
            var result = await dataStore.UpsertCellarRow(cellarRow);
            if (result.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to upsert the Cellar Row.");
                return (IActionResult)result;
            }
            var resultStr = JsonConvert.SerializeObject(result);
            log.LogInformation("UpsertCellar Request completed");
            return new OkObjectResult(resultStr);
        }
    }

}
