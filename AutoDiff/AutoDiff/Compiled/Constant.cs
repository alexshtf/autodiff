using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
	class Constant : TapeElement
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
