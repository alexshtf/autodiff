namespace AutoDiff.Compiled
{
    internal class Sum : TapeElement
    {
        public int[] Terms;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
