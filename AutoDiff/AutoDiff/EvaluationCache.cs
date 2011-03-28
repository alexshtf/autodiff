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

        private class CacheConstructorVisitor : ITermVisitor<double>
        {
            private readonly Dictionary<Term, double> cache;
            private readonly IDictionary<Variable, double> values;

            public CacheConstructorVisitor(Dictionary<Term, double> cache, IDictionary<Variable, double> values)
            {
                this.cache = cache;
                this.values = values;
            }

            public double Visit(Constant constant)
            {
                cache[constant] = constant.Value;
                return constant.Value;
            }

            public double Visit(Zero zero)
            {
                cache[zero] = 0;
                return 0;
            }

            public double Visit(IntPower intPower)
            {
                var result = Math.Pow(intPower.Base.Accept(this), intPower.Exponent);
                cache[intPower] = result;
                return result;
            }

            public double Visit(Product product)
            {
                var result = product.Left.Accept(this) * product.Right.Accept(this);
                cache[product] = result;
                return result;
            }

            public double Visit(Sum sum)
            {
                double result = 0;
                foreach (var term in sum.Terms)
                    result += term.Accept(this);
                cache[sum] = result;
                return result;
            }

            public double Visit(Variable variable)
            {
                double value;
                if (values.TryGetValue(variable, out value))
                {
                    cache[variable] = value;
                    return value;
                }
                else
                    throw new InvalidOperationException("A variable is missing a value");
            }

            public double Visit(Log log)
            {
                var result = Math.Log(log.Arg.Accept(this));
                cache[log] = result;
                return result;
            }

            public double Visit(Exp exp)
            {
                var result = Math.Exp(exp.Arg.Accept(this));
                cache[exp] = result;
                return result;
            }

            public double Visit(PiecewiseTerm piecewiseTerm)
            {
                foreach (var pair in piecewiseTerm.Pieces)
                {
                    var inequalityTerm = pair.Item1.Term;
                    var inequalityResult = inequalityTerm.Accept(this);
                    if (inequalityResult <= 0)
                    {
                        var valueTerm = pair.Item2;
                        var result = valueTerm.Accept(this);
                        cache[piecewiseTerm] = result;
                        return result;
                    }
                }

                throw new InvalidOperationException("Piecewise evaluation failed. No piece satisfies the conditions.");
            }
        }

        #endregion
    }
}
