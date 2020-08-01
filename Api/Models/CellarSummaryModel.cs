using MyWindDb.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class CellarSummaryModel
    {
        public AzureTableKey CellarId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int Capacity { get; set; }

        public int BottleCount { get; set; }

        // value of Wine in Cellar
        public double Value { get; set; }

        public IEnumerable<BottleBriefDataModel>Bottles { get; set; }

        public IEnumerable<BottleDetailModel>BottlesWithDetails { get; set; }


    }
}
