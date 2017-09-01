using System;

namespace AutoDiff.Compiled
{
    internal class NaryFunc : TapeElement
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
