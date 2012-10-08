using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class NaryFunc : TapeElement
    {
        public int[] Terms;
        public Func<double[], double> Eval;
        public Func<double[], double[]> Diff;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
