using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class InventoryVintageModel
    {
        public int VinetageYear { get; set; }
        public double VinetageValue { get; set; }
        public IEnumerable<InventoryWineItemModel> WineItems {get; set;}
    }
}
