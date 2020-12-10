using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class BottleTreeNode : IBottleTreeNode
    {
        public string Title { get; set; }
        public bool IsParent { get; set; }
        public int Count { get; set; }
        public IEnumerable<IBottleTreeNode> ChildNodes { get; set; }
    }
}
