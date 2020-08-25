using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class InventoryWineItemModel
    {
        public int Quantity { get; set; }
        public int Size { get; set; }
        public string WineryName { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string WineName { get; set; }
        public string Color { get; set; }
        public string VarietalType { get; set; }
        public double UnitValue { get; set; }
        public double TotalValue { get; set; }

        public IEnumerable<string>BarCodes { get; set; }

        public string WineBottleRowKey { get; set; }
    }
}
