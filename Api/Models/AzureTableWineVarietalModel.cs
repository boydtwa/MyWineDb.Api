using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.OData.Edm;
using MyWineDb.Api.Models;
using System.Linq;

namespace MyWineDb.Api.Models
{
    public class AzureTableWineVarietalModel : TableEntity
    {
        public double Percentage { get; set; }
    }
}
