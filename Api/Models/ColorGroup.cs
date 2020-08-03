using System.Collections.Generic;

namespace MyWineDb.Api.Models
{
    public class ColorGroup
    {
        public string Color { get; set; }
        public int TotalBottles { get; set; }
        public IEnumerable<VintageGroup> VintageGroups {get; set;}
    }
}
