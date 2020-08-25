namespace MyWineDb.Api.Models
{
    public class BottleBriefDataModel
    {
        public AzureTableKey BottleId;
        public string CountryOfOrigin { get; set; }
        public string Region { get; set; }
        public string WineryName { get; set; }
        public string WineName { get; set; }
        public string ProductLine { get; set; }
       public string Color { get; set; }
        public int Vintage { get; set; }
        public string VarietalType { get; set; }
 
        public int Size { get; set; }
        public double RetailPrice { get; set; } 
        public double PricePaid { get; set; }
        public string BarCode { get; set; }
        public bool IsDesert { get; set; }
    }
}
