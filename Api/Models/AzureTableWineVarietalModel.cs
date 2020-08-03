using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableWineVarietalModel : TableEntity
    {
        public double Percentage { get; set; }
    }
}
