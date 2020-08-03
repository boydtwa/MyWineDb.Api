using MyWineDb.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWineDb.Api.Services
{
    public interface IDataStore
    {
        Task<IList<CellarSummaryModel>> GetCellarList();
        Task<IEnumerable<BottleBriefDataModel>> GetCellarSummaryBottles(AzureTableKey CellarId);
        Task<BottleDetailModel> GetCellarBottleDetails(AzureTableKey BottleId);
    }
}
