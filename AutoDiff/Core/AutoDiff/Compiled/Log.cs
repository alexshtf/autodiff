namespace AutoDiff.Compiled
{
    internal sealed class Log : TapeElement
    {
        public int Arg;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
