using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class CellarsWithDetailModel
    {
        public IEnumerable<CellarDetailModel> Cellars { get; set; }
    }
}
