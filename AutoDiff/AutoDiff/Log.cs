using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public class Log : Term
    {
        public Log(Term arg)
        {
            Arg = arg;
        }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        public Term Arg { get; private set; }
    }
}
