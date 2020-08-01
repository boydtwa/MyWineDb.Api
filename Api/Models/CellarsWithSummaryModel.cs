using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class CellarsWithSummaryModel
    {
        public IEnumerable<CellarSummaryModel> Cellars { get; set; }
    }
}
