using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public class IntPower : Term
    {
        public IntPower(Term baseTerm, double exponent)
        {
            Base = baseTerm;
            Exponent = exponent;
        }

        public Term Base { get; private set; }
        public double Exponent { get; private set; }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
