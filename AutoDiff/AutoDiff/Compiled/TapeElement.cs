using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    abstract class TapeElement
    {
        public double Value;
        public double Adjoint;
        public InputConnection[] InputOf;

        public abstract void Accept(ITapeVisitor visitor);
    }
}
