using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTool.Models
{
    public class Tree
    {
        public Tree? left { get; set; }
        public Tree? right { get; set; }
        public char? charVal { get; set; }
        public int data;
        public string? bitValue { get; set; }

        public Tree()
        {

        }
        public Tree(Tree? left, Tree? right, char? charval, int data, string? bitValue)
        {
            this.left = left;
            this.right = right;
            this.charVal = charval;
            this.data = data;
            this.bitValue = bitValue;
        }
    }
}
