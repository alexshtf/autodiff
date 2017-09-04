using System;

namespace AutoDiff.Compiled
{
    internal sealed class BinaryFunc : TapeElement
    {
        public int Left => Inputs.Index(0);
        public int Right => Inputs.Index(1);
        public Func<double, double, double> Eval;
        public Func<double, double, Tuple<double, double>> Diff;

        public override void Accept(TapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
