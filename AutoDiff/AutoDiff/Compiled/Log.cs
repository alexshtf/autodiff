using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class Log : Term
    {
        public Term Arg;

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
