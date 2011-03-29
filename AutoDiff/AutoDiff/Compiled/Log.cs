using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
	class Log : TapeElement
    {
        public int Arg;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
