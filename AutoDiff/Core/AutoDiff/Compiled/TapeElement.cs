namespace AutoDiff.Compiled
{
    internal abstract class TapeElement
    {
        public double Value;
        public double Adjoint;
        public InputEdges Inputs;

        public abstract void Eval();
        public abstract void Diff();
    }
}
