using Microsoft.Azure.Cosmos.Table;
using System;

namespace MyWineDb.Api.Models
{
    public class AzureTableBottleModel : TableEntity
    {
        public double PricePaid { get; set; }
        public DateTime CellarDate { get; set; }
        public string BarCode { get; set; }
    }
}
