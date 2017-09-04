namespace AutoDiff.Compiled
{
    internal sealed class TermPower : TapeElement
    {
        public int Base => Inputs.Index(0);
        public int Exponent => Inputs.Index(1);

        public override void Accept(TapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
