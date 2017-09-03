namespace AutoDiff.Compiled
{
    internal sealed class TermPower : TapeElement
    {
        public int Base;
        public int Exponent;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
