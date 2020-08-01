using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class AzureTableKey
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
