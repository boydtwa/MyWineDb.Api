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

namespace MywineDb.Api
{
    public static class GetCellarBottleDetails
    {
        [FunctionName("GetCellarBottleDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation("Begin GetCellarBottleDetails request");
            var stream = req.Body;
            stream.Seek(0, SeekOrigin.Begin);
            var aztkString = new StreamReader(stream).ReadToEnd();
            var key = JsonConvert.DeserializeObject<AzureTableKey>(aztkString);
            if (key == null)
            {
                log.LogError($"Failed to Retireve Bottle Details for Azure Table Key provided: {aztkString}");
                return new StatusCodeResult(400);
            }

            log.LogInformation("GetCellarBottleDetails Api Request initiated");
            var dataStore = new DataStore(log, context);
            var result = await dataStore.GetCellarBottleDetails(key);
            if (result.GetType() == typeof(StatusCodeResult))
            {
                log.LogError($"Failed to get bottle detail from DataStore.");
                return (IActionResult)result;
            }
            return new OkObjectResult(JsonConvert.SerializeObject(await dataStore.GetCellarBottleDetails(key)));
        }
    }
}
