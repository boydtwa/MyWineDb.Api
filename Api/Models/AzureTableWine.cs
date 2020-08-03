using System;
using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableWine : TableEntity
    {
        public string Name { get; set; }
        public string ProductLine { get; set; }
        public string Color { get; set; }
        public int Vintage { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
