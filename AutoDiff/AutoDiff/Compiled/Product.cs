using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
	class Product : TapeElement
    {
        public int Left;
        public int Right;

        public override void Accept(ITapeVisitor visitor)
        {
            visitor.Visit(this);
        }
	}
}
