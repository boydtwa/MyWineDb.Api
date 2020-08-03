using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWineDb.Api.Models
{
    public class BottleDetailModel
    {
        public AzureTableEntityKeys EntityKeys { get; set; }

        public string BarCode { get; set; }
        public DateTime CellarDate { get; set; }
        public DateTime? DrinkDate { get; set; }
        public double PricePaid { get; set; }
        public double RetailPrice { get; set; }
        public string UPC { get; set; }
        public int Size { get; set; }
        public string QuantityProduced { get; set; }
        public string BottleNumber { get; set; }
        public string WineName { get; set; }
        public string WineProductLine { get; set; }
        public string Color { get; set; }
        public string WineReleaseDate { get; set; }
        public string WineVarietalType { get; set; }
        public string WineMaker { get; set; }
        public string WineAppellation { get; set; }
        public string ResSugar { get; set; }
        public string AlcPercent { get; set; }
        public bool isDessert { get; set; }
        public bool isRose { get; set; }
        public bool isEstate { get; set; }
        public string WineryName { get; set; }
        public string WineryRegion { get; set; }

        public string WineryAddress { get; set; }
        public string WineryGeoCoordinates { get; set; }

        public int Vintage { get; set; }
        public IEnumerable<WineVarietalDetailModel> VarietalDetails { get; set; }

        public IEnumerable<NoteDetailModel> Notes { get; set; }

        public bool HasNotes()
        {
            return (Notes == null || Notes.Count() == 0) ? false : true;
        }
        public double Price()
        {
            return PricePaid > 0 ? RetailPrice > PricePaid ? RetailPrice : PricePaid : 0;
        }

    }
}
