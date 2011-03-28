using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class Piecewise : Term
    {
        public Term[] Inequalities;
        public Term[] Terms;

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
