namespace AutoDiff.Compiled
{
	internal class Variable : TapeElement
	{
        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
