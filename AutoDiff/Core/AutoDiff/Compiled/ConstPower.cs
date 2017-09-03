namespace AutoDiff.Compiled
{
    internal sealed class ConstPower : TapeElement
    {
        public int Base;
        public double Exponent;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
