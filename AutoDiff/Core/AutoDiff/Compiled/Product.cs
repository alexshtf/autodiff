namespace AutoDiff.Compiled
{
    internal class Product : TapeElement
    {
        public int Left;
        public int Right;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
