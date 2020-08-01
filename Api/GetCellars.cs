using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using System.Collections.Generic;

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
