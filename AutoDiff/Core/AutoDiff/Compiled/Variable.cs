namespace AutoDiff.Compiled
{
	internal sealed class Variable : TapeElement
	{
        public override void Accept(TapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
