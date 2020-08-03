using System.Collections.Generic;

namespace MyWineDb.Api.Models
{
    public class CellarDetailModel
    {
        public AzureTableKey CellarId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Capacity { get; set; }

        public ColorGroup  ColorDetail {get; set;}

        public IEnumerable<BottleDetailModel> Bottles {get; set;}
    }
}
