using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class BinaryFunc : TapeElement
    {
        public int Left;
        public int Right;
        public Func<double, double, double> Eval;
        public Func<double, double, Tuple<double, double>> Diff;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
