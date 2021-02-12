using MyWineDb.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWineDb.Api.Services
{
    public interface IDataStore
    {
        public Task<IList<CellarSummaryModel>> GetCellarList();
        public Task<IEnumerable<BottleBriefDataModel>> GetCellarSummaryBottles(AzureTableKey CellarId);
        public Task<BottleDetailModel> GetCellarBottleDetails(AzureTableKey BottleId);
    }
}
