using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.Filters;
using MyWineDb.Api.Models;
using System.Collections.Generic;
using MyWindDb.Api.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using System.Linq;
using System.Globalization;
using Microsoft.WindowsAzure.Storage;
using cosmosTable = Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Azure.Documents;
using MyWineDb.Api.Services;

namespace MywineDb.Api
{
    public static class GetCellarBottleDetails
    {
        [FunctionName("GetCellarBottleDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
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
