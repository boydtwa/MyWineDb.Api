using System.Collections.Generic;
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
        public static IDataStore DataStore { get; set; }
        [FunctionName("GetCellarSummaryBottles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("Processing GetCellarSummaryBottles request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var aztkString = new StreamReader(stream).ReadToEnd();
            var key = JsonConvert.DeserializeObject<AzureTableKey>(aztkString);
            if (key == null)
            {
                log.LogError($"Failed to Retrieve Bottles for Azure Table Key provided: {aztkString}");
                return new StatusCodeResult(400);
            }

            log.LogInformation("GetCellarSummaryBottles Api Request initiated");
            DataStore ??= new DataStore(log, context);
            var result = await DataStore.GetCellarSummaryBottles(key);

            if (result != null && result.GetType() == typeof(List<BottleBriefDataModel>))
                return new OkObjectResult(JsonConvert.SerializeObject(result));
            log.LogError($"Failed to get bottle detail from DataStore.");
            return new StatusCodeResult(500);
        }
    }
}
