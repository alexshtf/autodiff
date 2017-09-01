namespace AutoDiff.Compiled
{
    internal class Constant : TapeElement
	{
        public Constant(double value)
        {
            Value = value;
            Adjoint = 0;
        }

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
