using System.Collections.Generic;

namespace MyWineDb.Api.Models
{
    public class CellarsWithDetailModel
    {
        public IEnumerable<CellarDetailModel> Cellars { get; set; }
    }
}
