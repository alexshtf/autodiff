using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public abstract class Term
    {
        public abstract void Accept(ITermVisitor visitor);

        public static implicit operator Term(double value)
        {
            return TermBuilder.Constant(value);
        }

        public static Term operator+(Term left, Term right)
        {
            return TermBuilder.Sum(left, right);
        }

        public static Term operator*(Term left, Term right)
        {
            return TermBuilder.Product(left, right);
        }

        public static Term operator-(Term left, Term right)
        {
            return left + (-1) * right;
        }
    }
}
