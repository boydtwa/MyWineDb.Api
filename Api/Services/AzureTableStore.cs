using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyWineDb.Api.Models;

namespace MyWineDb.Api.Services
{
    public class AzureTableStore : IAzureTableStore
    {
        private CloudTableClient TableClient { get; set; }
        public AzureTableStore(ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation($"Attempting AzureApi Connection");
            var constr = config["DbConnectViaKeyVault"];
            CloudStorageAccount storeAccount =
                CloudStorageAccount.Parse(constr);
            TableClient = storeAccount.CreateCloudTableClient();
            log.LogInformation($"AzureApi Connection Established");
        }

        public Task<AzureTableCellarModel> GetCellarRow(AzureTableKey Key)
        {
            throw new System.NotImplementedException();
        }

        public Task<AzureTableKey> UpsertCellarRow(AzureTableCellarModel Cellar)
        {
            throw new System.NotImplementedException();
        }
    }
}
