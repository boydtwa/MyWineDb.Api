using System.Collections.Generic;

namespace MyWineDb.Api.Models
{
    public class AzureTableEntityKeys
    {
        public AzureTableKey BottleId { get; set; }
        public AzureTableKey CellarId { get; set; }
        public AzureTableKey WineryId { get; set; }
        public AzureTableKey WineId { get; set; }
        public AzureTableKey WineBottleId { get; set; }
    }
}
