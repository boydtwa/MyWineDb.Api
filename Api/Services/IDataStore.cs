using Microsoft.Azure.Cosmos.Table;
using MyWindDb.Api.Models;
using MyWineDb.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
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
