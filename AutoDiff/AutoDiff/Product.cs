using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public class Product : Term
    {
        public Product(Term left, Term right)
        {
            Left = left;
            Right = right;
        }

        public Term Left { get; private set; }
        public Term Right { get; private set; }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
