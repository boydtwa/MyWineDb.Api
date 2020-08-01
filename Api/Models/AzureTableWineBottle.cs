using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableWineBottle : TableEntity
    {
        public int Size { get; set; }
        public double RetailPrice { get; set; }
        public string UPC { get; set; }
        public int QuantityProduced { get; set; }
        public int BottleNumber { get; set; }
    }
}
