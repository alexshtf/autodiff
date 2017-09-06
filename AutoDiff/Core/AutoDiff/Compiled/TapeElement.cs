using System;

namespace AutoDiff.Compiled
{
    internal abstract class TapeElement
    {
        public double Value;
        public double Adjoint;
        public InputEdges Inputs;

        public abstract void Eval(TapeElement[] tape);
        public abstract void Diff(TapeElement[] tape);
    }
}
