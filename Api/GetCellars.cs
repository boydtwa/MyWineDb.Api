using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;
using MyWineDb.Api.Services;
using Newtonsoft.Json;

namespace MyWineDb.Api
{
    public static class GetCellars
    {
        public static IDataStore DataStore { get; set; }

        [FunctionName("GetCellars")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log, ExecutionContext context)
        {
            log.LogInformation("Begin GetCellars request  ");

            DataStore ??= new DataStore(log, context);
            log.LogInformation("GetCellars Api Request initiated");
            try
            {
                var resultStr = JsonConvert.SerializeObject(await DataStore.GetCellarList());
                if (resultStr != null)
                    return new OkObjectResult(resultStr);

                throw new Exception("Result returned could not be serialized");
            }
            catch (Exception e)
            {
                log.LogError($"Failed to get Cellar Summaries from DataStore. \nMessage: {e.Message}");

            }
            return new StatusCodeResult(500);
        }

    }
}
