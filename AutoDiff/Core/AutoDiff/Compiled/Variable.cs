namespace AutoDiff.Compiled
{
	internal sealed class Variable : TapeElement
	{
        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
