using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Services;
using Newtonsoft.Json;

namespace MyWineDb.Api
{
    public static class GetCellars
    {
        
        [FunctionName("GetCellars")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log, ExecutionContext context)
        {
            log.LogInformation("Begin GetCellars request  ");

            var dataStore = new DataStore(log, context);
            var resultStr = JsonConvert.SerializeObject(await dataStore.GetCellarList());
            log.LogInformation($"GetCellars request processed: {resultStr}");

            return new OkObjectResult(resultStr);
        }

    }
}
