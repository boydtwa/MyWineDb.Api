using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class InventoryCellarModel
    {
        public string CellarName { get; set; }

        public int BottleCount { get; set; }
        public int Capacity { get; set; }
        public string PctCapacity { get; set; }
        public double CellarTotalValue { get; set; }
        public IEnumerable<InventoryVintageModel> Vintages { get; set;}
    }
}
