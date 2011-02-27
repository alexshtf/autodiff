using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public class Exp : Term
    {
        public Exp(Term arg)
        {
            Arg = arg;
        }

        public Term Arg { get; private set; }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
