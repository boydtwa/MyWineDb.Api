using MyWineDb.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWineDb.Api.Services
{
    public interface IAzureTableStore
    {
        public Task<AzureTableCellarModel> GetCellarRow(AzureTableKey Key);
        public Task<AzureTableKey> UpsertCellarRow(AzureTableCellarModel Cellar);

    }
}
