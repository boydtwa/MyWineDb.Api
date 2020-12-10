using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class InventoryReportModel
    {
        public DateTime DateOfReport { get; set; }
        public int TotalNumberOfBottles { get; set; }
        public double AveBottleValue { get; set; }
        public double CellarsTotalValue { get; set; }
        public IEnumerable<InventoryCellarModel> Cellars { get; set; }
    }
}
