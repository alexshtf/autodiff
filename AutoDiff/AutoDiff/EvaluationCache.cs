using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public class EvaluationCache
    {
        private readonly Term root;
        private readonly Dictionary<Term, double> cache;

        public EvaluationCache(Term root, IDictionary<Variable, double> values)
        {
            this.root = root;
            this.cache = new Dictionary<Term, double>();

            var cacheConstructor = new CacheConstructorVisitor(cache, values);
            root.Accept(cacheConstructor);
        }

        public double this[Term term]
        {
            get { return cache[term]; }
        }

        #region CacheConstructorVisitor class

        private class CacheConstructorVisitor : ITermVisitor
        {
            private readonly Dictionary<Term, double> cache;
            private readonly IDictionary<Variable, double> values;

            public CacheConstructorVisitor(Dictionary<Term, double> cache, IDictionary<Variable, double> values)
            {
                this.cache = cache;
                this.values = values;
            }

            public void Visit(Constant constant)
            {
                cache[constant] = constant.Value;
            }

            public void Visit(Zero zero)
            {
                cache[zero] = 0;
            }

            public void Visit(IntPower intPower)
            {
                var powerBase = intPower.Base;
                powerBase.Accept(this);

                cache[intPower] = Math.Pow(cache[powerBase], intPower.Exponent);
            }

            public void Visit(Product product)
            {
                var left = product.Left;
                var right = product.Right;

                left.Accept(this);
                right.Accept(this);

                cache[product] = cache[left] * cache[right];
            }

            public void Visit(Sum sum)
            {
                foreach (var term in sum.Terms)
                    term.Accept(this);

                var sumValues = from term in sum.Terms
                                select cache[term];

                cache[sum] = sumValues.Sum();
            }

            public void Visit(Variable variable)
            {
                double value;
                if (values.TryGetValue(variable, out value))
                    cache[variable] = value;
                else
                    throw new InvalidOperationException("A variable is missing a value");
            }

            public void Visit(Log log)
            {
                var arg = log.Arg;
                arg.Accept(this);

                cache[log] = Math.Log(cache[arg]);
            }

            public void Visit(Exp exp)
            {
                var arg = exp.Arg;
                arg.Accept(this);

                cache[exp] = Math.Exp(cache[arg]);
            }


            public void Visit(PiecewiseTerm piecewiseTerm)
            {
                foreach (var pair in piecewiseTerm.Pieces)
                {
                    var inequalityTerm = pair.Item1.Term;
                    var valueTerm = pair.Item2;
                    inequalityTerm.Accept(this);
                    if (cache[inequalityTerm] <= 0)
                    {
                        valueTerm.Accept(this);
                        cache[piecewiseTerm] = cache[valueTerm];
                        return;
                    }
                }

                throw new InvalidOperationException("Piecewise evaluation failed. No piece satisfies the conditions.");
            }
        }

        #endregion
    }
}
