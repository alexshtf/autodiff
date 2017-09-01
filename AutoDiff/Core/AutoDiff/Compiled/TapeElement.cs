namespace AutoDiff.Compiled
{
    abstract class TapeElement
    {
        public double Value;
        public double Adjoint;
        public InputEdge[] Inputs;

        public abstract void Accept(ITapeVisitor visitor);
    }
}
