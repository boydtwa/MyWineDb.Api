using System.Collections.Generic;

namespace MyWineDb.Api.Models
{
    public class CellarsWithSummaryModel
    {
        public IEnumerable<CellarSummaryModel> Cellars { get; set; }
    }
}
