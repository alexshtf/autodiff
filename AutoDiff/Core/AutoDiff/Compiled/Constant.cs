namespace AutoDiff.Compiled
{
    internal sealed class Constant : TapeElement
	{
        public Constant(double value)
        {
            Value = value;
            Adjoint = 0;
        }

	    public override void Eval(TapeElement[] tape)
	    {}

	    public override void Diff(TapeElement[] tape)
	    {}
	}
}
