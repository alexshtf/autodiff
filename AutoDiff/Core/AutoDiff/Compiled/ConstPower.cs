namespace AutoDiff.Compiled
{
    internal sealed class ConstPower : TapeElement
    {
        public int Base => Inputs.Index(0);
        public double Exponent;

        public override void Accept(TapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
