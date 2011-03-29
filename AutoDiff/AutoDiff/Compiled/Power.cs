using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class Power : TapeElement
    {
        public int Base;
        public double Exponent;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
