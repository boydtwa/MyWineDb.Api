using MyWindDb.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class CellarDetailModel
    {
        public AzureTableKey CellarId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Capacity { get; set; }

        public ColorGroup  ColorDetail {get; set;}

        public IEnumerable<BottleDetailModel> Bottles {get; set;}


        //public double PercentFull()
        //{
        //    return (Convert.ToDouble(BottleCount()) / Convert.ToDouble(Capacity)) * 100;
        //}

        //public int BottleCount()
        //{
        //    return Bottles == null ? 0 : Bottles.Count();
        //}


    }
}
