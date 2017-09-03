using System;

namespace AutoDiff.Compiled
{
    internal sealed class NaryFunc : TapeElement
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
