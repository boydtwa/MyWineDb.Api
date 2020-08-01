using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableVineyardVarietalModel : TableEntity
    {
        public string CloneName { get; set; }
    }
}
