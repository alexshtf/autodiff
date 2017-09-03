namespace AutoDiff.Compiled
{
    internal sealed class Product : TapeElement
    {
        public int Left => Inputs.Index(0);
        public int Right => Inputs.Index(1);

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
