using System;

namespace AutoDiff.Compiled
{
    internal sealed class NaryFunc : TapeElement
    {
        public struct TermsAccessor
        {
            private NaryFunc obj;
            public TermsAccessor(NaryFunc obj) { this.obj = obj; }
            public int this[int i] => obj.Inputs.Index(i);
            public int Length => obj.Inputs.Length;
        }

        public TermsAccessor Terms => new TermsAccessor(this);
        public Func<double[], double> Eval;
        public Func<double[], double[]> Diff;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
