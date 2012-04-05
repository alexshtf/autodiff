using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CompileResult = AutoDiff.Compiled.TapeElement;

namespace AutoDiff
{
    partial class CompiledDifferentiator
    {
        private class Compiler : ITermVisitor<int> // int --> the index of the compiled element in the tape
        {
            private readonly List<Compiled.TapeElement> tape;
            private readonly Dictionary<Term, int> indexOf;

            public Compiler(Variable[] variables, List<Compiled.TapeElement> tape)
            {
                this.tape = tape;
                indexOf = new Dictionary<Term, int>();
                foreach (var i in Enumerable.Range(0, variables.Length))
                {
                    indexOf[variables[i]] = i;
                    tape.Add(new Compiled.Variable());
                }
            }

            public void Compile(Term term)
            {
                term.Accept(this);
            }

            public int Visit(Constant constant)
            {
                return Compile(constant, () => new Compiled.Constant(constant.Value) { Inputs = new Compiled.InputEdge[0] });
            }

            public int Visit(Zero zero)
            {
                return Compile(zero, () => new Compiled.Constant(0) { Inputs = new Compiled.InputEdge[0] });
            }

            public int Visit(ConstPower intPower)
            {
                return Compile(intPower, () =>
                    {
                        var baseIndex = intPower.Base.Accept(this);
                        var element = new Compiled.ConstPower
                        {
                            Base = baseIndex,
                            Exponent = intPower.Exponent,
                            Inputs = new Compiled.InputEdge[] 
                            {
                                new Compiled.InputEdge { Index = baseIndex },
                            },
                        };

                        return element;
                    });
            }

            public int Visit(TermPower power)
            {
                return Compile(power, () =>
                {
                    var baseIndex = power.Base.Accept(this);
                    var expIndex = power.Exponent.Accept(this);
                    var element = new Compiled.TermPower
                    {
                        Base = baseIndex,
                        Exponent = expIndex,
                        Inputs = new Compiled.InputEdge[]
                        {
                            new Compiled.InputEdge { Index = baseIndex },
                            new Compiled.InputEdge { Index = expIndex },
                        },
                    };

                    return element;
                });
            }

            public int Visit(Product product)
            {
                return Compile(product, () =>
                    {
                        var leftIndex = product.Left.Accept(this);
                        var rightIndex = product.Right.Accept(this);
                        var element = new Compiled.Product
                        {
                            Left = leftIndex,
                            Right = rightIndex,
                            Inputs = new Compiled.InputEdge[]
                            {
                                new Compiled.InputEdge { Index = leftIndex },
                                new Compiled.InputEdge { Index = rightIndex },
                            }
                        };

                        return element;
                    });
            }

            public int Visit(Sum sum)
            {
                return Compile(sum, () =>
                    {
                        var indicesQuery = from term in sum.Terms
                                           select term.Accept(this);
                        var indices = indicesQuery.ToArray();
                        var element = new Compiled.Sum 
                        { 
                            Terms = indices,
                            Inputs = indices.Select(x => new Compiled.InputEdge { Index = x }).ToArray(),
                        };

                        return element;
                    });
            }

            public int Visit(Variable variable)
            {
                return indexOf[variable];
            }

            public int Visit(Log log)
            {
                return Compile(log, () =>
                    {
                        var argIndex = log.Arg.Accept(this);
                        var element = new Compiled.Log 
                        { 
                            Arg = argIndex,
                            Inputs = new Compiled.InputEdge[]
                            {
                                new Compiled.InputEdge { Index = argIndex },
                            },
                        };

                        return element;
                    });
            }

            public int Visit(Exp exp)
            {
                return Compile(exp, () =>
                    {
                        var argIndex = exp.Arg.Accept(this);
                        var element = new Compiled.Exp
                        {
                            Arg = argIndex,
                            Inputs = new Compiled.InputEdge[]
                            {
                                new Compiled.InputEdge { Index = argIndex },
                            },
                        };

                        return element;
                    });
            }

            public int Visit(UnaryFunc func)
            {
                return Compile(func, () =>
                    {
                        var argIndex = func.Argument.Accept(this);
                        var element = new Compiled.UnaryFunc(func.Eval, func.Diff)
                        {
                            Arg = argIndex,
                            Inputs = new Compiled.InputEdge[]
                            {
                                new Compiled.InputEdge { Index = argIndex },
                            },
                        };

                        return element;
                    });
            }

            public int Visit(BinaryFunc func)
            {
                return Compile(func, () =>
                    {
                        var leftIndex = func.Left.Accept(this);
                        var rightIndex = func.Right.Accept(this);

                        var element = new Compiled.BinaryFunc
                        {
                            Eval = func.Eval,
                            Diff = func.Diff,
                            Left = leftIndex,
                            Right = rightIndex,
                            Inputs = new Compiled.InputEdge[]
                            {
                                new Compiled.InputEdge { Index = leftIndex },
                                new Compiled.InputEdge { Index = rightIndex },
                            }
                        };

                        return element;
                    });
            }

            public int Visit(NaryFunc func)
            {
                return Compile(func, () =>
                {
                    var indicesQuery = from term in func.Terms
                                       select term.Accept(this);
                    var indices = indicesQuery.ToArray();

                    var element = new Compiled.NaryFunc
                    {
                        Eval = func.Eval,
                        Diff = func.Diff,
                        Terms = indices,
                        Inputs = indices.Select(x => new Compiled.InputEdge { Index = x }).ToArray(),
                    };

                    return element;
                });
            }


            private int Compile(Term term, Func<CompileResult> compiler)
            {
                int index;
                if (!indexOf.TryGetValue(term, out index))
                {
                    var compileResult = compiler();
                    tape.Add(compileResult);

                    index = tape.Count - 1;
                    indexOf.Add(term, index);
                }

                return index;
            }
        }
    }
}
