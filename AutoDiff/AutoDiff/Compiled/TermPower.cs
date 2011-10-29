using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class TermPower : TapeElement
    {
        public int Base;
        public int Exponent;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
