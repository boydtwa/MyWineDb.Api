using Microsoft.Azure.KeyVault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MyWineDb.Api.Models
{
    public class AzureTableEntityKeys
    {
        public string EntityName { get; set; }
        public AzureTableKey BottleId { get; set; }
        public AzureTableKey CellarId { get; set; }
        public AzureTableKey WineryId { get; set; }
        public AzureTableKey WineId { get; set; }
        public AzureTableKey WineBottleId { get; set; }
        public IEnumerable<AzureTableKey> WineVarietalIds { get; set; }
        public IEnumerable<AzureTableKey> VarietalIds { get; set; }
        public IEnumerable<AzureTableKey> VineyardIds { get; set; }
        public IEnumerable<AzureTableKey> VineyardVarietalIds { get; set; }
        public IEnumerable<AzureTableKey> WineVarietalVineyardVarietalIds { get; set; }
    }
}
