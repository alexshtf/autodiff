using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public class Zero : Term
    {
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
