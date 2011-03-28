using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class Power : Term
    {
        public Term Base;
        public double Exponent;

        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

    }
}
