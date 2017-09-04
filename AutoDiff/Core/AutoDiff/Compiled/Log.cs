namespace AutoDiff.Compiled
{
    internal sealed class Log : TapeElement
    {
        public int Arg => Inputs.Index(0);

        public override void Accept(TapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
