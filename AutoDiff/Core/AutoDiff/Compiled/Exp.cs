namespace AutoDiff.Compiled
{
    internal sealed class Exp : TapeElement
    {
        public int Arg;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
