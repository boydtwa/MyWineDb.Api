using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MyWineDb.Api
{
    public static class GetCellarBottleDetails
    {
        public static IDataStore DataStore { get; set; }
        [FunctionName("GetCellarBottleDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("Begin GetCellarBottleDetails request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var azureTaskString = await new StreamReader(stream).ReadToEndAsync();
            var key = JsonConvert.DeserializeObject<AzureTableKey>(azureTaskString);
            if (key == null)
            {
                log.LogError($"Failed to Retrieve Bottle Details for Azure Table Key provided: {azureTaskString}");
                return new StatusCodeResult(400);
            }

            log.LogInformation("GetCellarBottleDetails Api Request initiated");

            DataStore ??= new DataStore(log, context);
            var result = await DataStore.GetCellarBottleDetails(key);

            if (result != null && result.GetType() == typeof(BottleDetailModel))
                return new OkObjectResult(JsonConvert.SerializeObject(result));
            log.LogError($"Failed to get bottle detail from DataStore.");
            return new StatusCodeResult(500);
        }
    }
}
