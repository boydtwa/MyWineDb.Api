using System;
using System.Collections.Generic;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class BottleTreeLeafNode : IBottleTreeNode
    {
        public string Title { get; set; }
        public bool IsParent { get; set; }
        public int Count { get; set; }
        public AzureTableKey WineBottleId { get; set; }
        IEnumerable<IBottleTreeNode> IBottleTreeNode.ChildNodes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
