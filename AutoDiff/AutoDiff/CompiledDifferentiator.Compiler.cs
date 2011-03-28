using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public partial class CompiledDifferentiator
    {
        private class CompilerVisitor : ITermVisitor<Compiled.Term>
        {
            private Dictionary<Variable, int> indexOf;
            private Dictionary<Term, Compiled.Term> compiledTerms;

            public CompilerVisitor(Term function, Variable[] variables)
            {
                indexOf = new Dictionary<Variable, int>();
                foreach (var i in Enumerable.Range(0, variables.Length))
                    indexOf[variables[i]] = i;
                compiledTerms = new Dictionary<Term, Compiled.Term>();
            }

            public Compiled.Term Visit(Constant constant)
            {
                return new Compiled.Constant { Multiplier = constant.Value };
            }

            public Compiled.Term Visit(Zero zero)
            {
                return new Compiled.Constant { Multiplier = 0 };
            }

            public Compiled.Term Visit(IntPower intPower)
            {
                return CachedCompile(intPower, () =>
                    {
                        var compiledBase = intPower.Base.Accept(this);
                        if (compiledBase.Multiplier == 0)
                            return new Compiled.Constant { Multiplier = 0 };
                        else if (compiledBase is Compiled.Constant)
                        {
                            var value = ((Compiled.Constant)compiledBase).Multiplier;
                            return new Compiled.Constant { Multiplier = Math.Pow(value, intPower.Exponent) };
                        }
                        else
                            return new Compiled.Power { Base = compiledBase, Exponent = intPower.Exponent };
                    });
            }

            public Compiled.Term Visit(Product product)
            {
                return CachedCompile(product, () =>
                    {
                        double constant = 1;
                        var children = new Compiled.Term[2];
                        int childrenCount = 0;

                        // compile left and right parts of the product
                        var compiledLeft = product.Left.Accept(this);
                        var compiledRight = product.Right.Accept(this);

                        // check if the left part compiles to a constant and update as necessary
                        if (compiledLeft is Compiled.Constant)
                            constant = constant * ((Compiled.Constant)compiledLeft).Multiplier;
                        else
                            children[childrenCount++] = compiledLeft;

                        // check if the right part compiles to a constant and update as necessary
                        if (compiledRight is Compiled.Constant)
                            constant = constant * ((Compiled.Constant)compiledRight).Multiplier;
                        else
                            children[childrenCount++] = compiledRight;

                        if (childrenCount == 2) // everything is not a constant
                            return new Compiled.Product { Left = compiledLeft, Right = compiledRight };
                        else if (childrenCount == 1) // we have a constant and a non-constant child
                        {
                            var child = children[0];
                            child.Multiplier *= constant;
                            return child;
                        }
                        else // childrenCount == 0 --> everything is a constant
                            return new Compiled.Constant { Multiplier = constant };
                    });
            }

            public Compiled.Term Visit(Sum sum)
            {
                return CachedCompile(sum, () =>
                    {
                        var compiledChildren = new List<Compiled.Term>();
                        double bias = 0;
                        foreach (var child in sum.Terms)
                        {
                            var compiledChild = child.Accept(this);
                            if (compiledChild is Compiled.Constant)
                                bias += ((Compiled.Constant)compiledChild).Multiplier;
                            else
                                compiledChildren.Add(compiledChild);
                        }

                        if (compiledChildren.Count == 0)
                            return new Compiled.Constant { Multiplier = bias };
                        else
                            return new Compiled.Sum { Terms = compiledChildren.ToArray(), Bias = bias };
                    });
            }

            public Compiled.Term Visit(Log log)
            {
                return CachedCompile(log, () =>
                    {
                        var compiledArg = log.Arg.Accept(this);
                        if (compiledArg is Compiled.Constant)
                        {
                            var value = ((Compiled.Constant)compiledArg).Multiplier;
                            value = Math.Log(value);
                            return new Compiled.Constant { Multiplier = value };
                        }
                        else
                            return new Compiled.Log { Arg = compiledArg };
                    });
            }

            public Compiled.Term Visit(Variable variable)
            {
                return new Compiled.Variable { Index = indexOf[variable] };
            }

            public Compiled.Term Visit(Exp exp)
            {
                return CachedCompile(exp, () =>
                    {
                        var compiledArg = exp.Arg.Accept(this);
                        if (compiledArg is Compiled.Constant)
                        {
                            var value = ((Compiled.Constant)compiledArg).Multiplier;
                            value = Math.Exp(value);
                            return new Compiled.Constant { Multiplier = value };
                        }
                        else
                            return new Compiled.Exp { Arg = compiledArg };
                    });
            }

            public Compiled.Term Visit(PiecewiseTerm piecewiseTerm)
            {
                return CachedCompile(piecewiseTerm, () =>
                    {
                        var inequalities = new List<Compiled.Term>();
                        var terms = new List<Compiled.Term>();

                        foreach (var item in piecewiseTerm.Pieces)
                        {
                            inequalities.Add(item.Item1.Term.Accept(this));
                            terms.Add(item.Item2.Accept(this));
                        }

                        return new Compiled.Piecewise
                        {
                            Terms = terms.ToArray(),
                            Inequalities = inequalities.ToArray(),
                        };
                    });
            }

            private Compiled.Term CachedCompile(Term term, Func<Compiled.Term> compiler)
            {
                Compiled.Term result;
                if (compiledTerms.TryGetValue(term, out result))
                {
                    if (!(result is Compiled.Constant) && !(result is Compiled.Variable))
                        result.IsGradientCached = true;
                    return result;
                }

                result = compiler();
                compiledTerms[term] = result;
                return result;
            }
        }

    }
}
