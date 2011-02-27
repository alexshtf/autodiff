using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public static class InequalityExtensions
    {
        public static Inequality LessThanEquals(this Term x, Term y)
        {
            return Inequality.LessThanEquals(x, y);
        }

        public static Inequality GreaterThanEquals(this Term x, Term y)
        {
            return Inequality.GreaterThanEquals(x, y);
        }
    }
}
