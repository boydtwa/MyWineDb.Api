using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableVarietalModel : TableEntity
    {
        public string Name { get; set; }
    }
}
