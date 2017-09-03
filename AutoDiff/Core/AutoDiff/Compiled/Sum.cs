namespace AutoDiff.Compiled
{
    internal sealed class Sum : TapeElement
    {
        public struct TermsAccessor
        {
            private Sum obj;
            public TermsAccessor(Sum obj) { this.obj = obj; }
            public int this[int i] => obj.Inputs.Index(i);
            public int Length => obj.Inputs.Length;
        }

        public TermsAccessor Terms => new TermsAccessor(this);

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
