using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableWinery : TableEntity
    {
        public string Name { get; set; }

        // State, Provience etc.
        public string Region { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string GeoCoordinates { get; set; }
    }
}
