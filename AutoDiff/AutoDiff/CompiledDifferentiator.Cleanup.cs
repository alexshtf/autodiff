using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public partial class CompiledDifferentiator
    {
        private class CleanupVisitor : Compiled.ITermVisitor
        {
            public static readonly CleanupVisitor Instance = new CleanupVisitor();

            public void Visit(Compiled.Constant constant)
            {
                constant.HasCachedValue = true;
                constant.CachedValue = constant.Multiplier;
                constant.CachedGradient = null;
            }

            public void Visit(Compiled.Log log)
            {
                log.Arg.Accept(this);

                log.HasCachedValue = false;
                log.CachedGradient = null;
            }

            public void Visit(Compiled.Power power)
            {
                power.Base.Accept(this);

                power.HasCachedValue = false;
                power.CachedGradient = null;
            }

            public void Visit(Compiled.Product product)
            {
                product.Left.Accept(this);
                product.Right.Accept(this);

                product.HasCachedValue = false;
                product.CachedGradient = null;
            }

            public void Visit(Compiled.Sum sum)
            {
                foreach (var term in sum.Terms)
                    term.Accept(this);

                sum.HasCachedValue = false;
                sum.CachedGradient = null;
            }

            public void Visit(Compiled.Variable variable)
            {
                variable.HasCachedValue = false;
                variable.CachedGradient = null;
            }


            public void Visit(Compiled.Exp exp)
            {
                exp.Arg.Accept(this);

                exp.HasCachedValue = false;
                exp.CachedGradient = null;
            }

            public void Visit(Compiled.Piecewise piecewise)
            {
                foreach (var term in piecewise.Terms)
                    term.Accept(this);
                foreach (var inequality in piecewise.Inequalities)
                    inequality.Accept(this);

                piecewise.HasCachedValue = false;
                piecewise.CachedGradient = null;
            }
        }
    }
}
