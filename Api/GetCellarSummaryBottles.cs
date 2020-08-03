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

namespace MywineDb.Api
{
    public static class GetCellarSummaryBottles
    {
        [FunctionName("GetCellarSummaryBottles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing GetCellarSummaryBottles request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var aztkString = new StreamReader(stream).ReadToEnd();
            var key = JsonConvert.DeserializeObject<AzureTableKey>(aztkString);
            if (key == null)
            {
                log.LogError($"Failed to Retireve Bottles for Azure Table Key provided: {aztkString}");
                return new StatusCodeResult(400);
            }

            log.LogInformation("GetCellarSummaryBottles Api Request initiated");
            var dataStore = new DataStore(log, context);
            var result = await dataStore.GetCellarSummaryBottles(key);
            if (result.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to get bottles from DataStore.");
                return (IActionResult)result;
            }
            var resultStr = JsonConvert.SerializeObject(await dataStore.GetCellarSummaryBottles(key));
            log.LogInformation("GetCellarSummaryBottles Api Request completed");
            return new OkObjectResult(resultStr);
        }
    }
}
