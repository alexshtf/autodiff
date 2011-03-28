using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    abstract class Term
    {
        public Term()
        {
            Multiplier = 1;
        }

        public abstract void Accept(ITermVisitor visitor);
        public abstract TResult Accept<TResult>(ITermVisitor<TResult> visitor);

        public double CachedValue;
        public bool HasCachedValue;

        public double Multiplier;

        public bool IsGradientCached;
        public SparseVector CachedGradient;
    }
}
