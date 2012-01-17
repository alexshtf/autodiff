using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoDiff
{
    partial class ExpressionTreeDifferentiator
    {
        private class MathMethods
        {
            private static readonly MethodInfo EXP = new Func<double, double>(Math.Exp).Method;
            private static readonly MethodInfo LOG = new Func<double, double>(Math.Log).Method;
            private static readonly MethodInfo POW = new Func<double, double, double>(Math.Pow).Method;

            public Expression Exp(Expression arg)
            {
                var result = Expression.Call(EXP, arg);
                return result;
            }

            public Expression Log(Expression arg)
            {
                var result = Expression.Call(LOG, arg);
                return result;
            }

            public Expression Pow(Expression powBase, Expression exponent)
            {
                var result = Expression.Call(POW, powBase, exponent);
                return result;
            }
        }
    }
}
