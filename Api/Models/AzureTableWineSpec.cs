using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableWineSpec : TableEntity
    {
        public string VarietalType { get; set; }
        public string WineMaker { get; set; }
        public string Appellation { get; set; }
        public double ResSugar { get; set; }
        public double AlcPercent { get; set; }
        public bool isDessert { get; set; }
        public bool isRose { get; set; }
        public bool isEstate { get; set; }
    }
}
