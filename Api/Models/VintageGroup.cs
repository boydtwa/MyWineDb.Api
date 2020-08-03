using System.Collections.Generic;

namespace MyWineDb.Api.Models
{
    public class VintageGroup
    {
        public int Count {get; set;}
        public int Year { get; set; }

        public string Color { get; set; }

        public IEnumerable<AzureTableKey> BottleIds { get; set; }

    }
}
