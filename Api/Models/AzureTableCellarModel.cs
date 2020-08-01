using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace MyWineDb.Api.Models
{
    public class AzureTableCellarModel : TableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }

        public AzureTableCellarModel()
        {

        }
    }
}
