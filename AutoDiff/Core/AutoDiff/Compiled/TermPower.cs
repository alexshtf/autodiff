namespace AutoDiff.Compiled
{
    internal class TermPower : TapeElement
    {
        public int Base;
        public int Exponent;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
