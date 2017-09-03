namespace AutoDiff.Compiled
{
    internal sealed class Log : TapeElement
    {
        public int Arg => Inputs.Index(0);

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
