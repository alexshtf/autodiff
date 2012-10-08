using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class UnaryFunc : TapeElement
    {
        public int Arg;
        public readonly Func<double, double> Eval;
        public readonly Func<double, double> Diff;

        public UnaryFunc(Func<double, double> eval, Func<double, double> diff)
        {
            this.Eval = eval;
            this.Diff = diff;
        }

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}