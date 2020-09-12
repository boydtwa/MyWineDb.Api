using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public interface IBottleTreeNode
    {
        string Title { get; set; }
        bool IsParent { get; set; }
        int Count { get; set; }
        //AzureTableKey WineBottleId { get; set; }
        IEnumerable<IBottleTreeNode> ChildNodes { get; set; }

    }
}
