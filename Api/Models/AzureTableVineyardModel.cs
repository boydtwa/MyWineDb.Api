using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableVineyardModel : TableEntity
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Appellation { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string GeoCoordinates { get; set; }
    }
}
