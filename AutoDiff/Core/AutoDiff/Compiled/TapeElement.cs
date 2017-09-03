using System;

namespace AutoDiff.Compiled
{
    abstract class TapeElement
    {
        public double Value;
        public double Adjoint;
        public InputEdges Inputs;

        public abstract void Accept(ITapeVisitor visitor);
    }
}
