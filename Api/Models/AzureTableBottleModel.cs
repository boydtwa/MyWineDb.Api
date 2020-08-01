using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableBottleModel : TableEntity
    {
        public double PricePaid { get; set; }
        public DateTime CellarDate { get; set; }
        public string BarCode { get; set; }
    }
}
