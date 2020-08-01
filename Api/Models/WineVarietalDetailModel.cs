using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class WineVarietalDetailModel
    {
        public AzureTableKey WineVarietalId { get; set; }
        
        public string VarietalName { get; set; }
        public double Percentage { get; set; }

        public VineyardVarietalDetail VineyardDetail { get; set; }

        public bool HasVinyardDetail()
        {
            return VineyardDetail == null || string.IsNullOrEmpty(VineyardDetail.VineyardName) ? false : true;
        }
    }
}
