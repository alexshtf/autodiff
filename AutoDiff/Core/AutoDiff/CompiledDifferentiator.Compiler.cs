using System;
using System.Collections.Generic;
using CompileResult = AutoDiff.Compiled.TapeElement;

namespace AutoDiff
{
    internal partial class CompiledDifferentiator<T>
    {
        
        private class Compiler : ITermVisitor<int> // int --> the index of the compiled element in the tape
        {
            
            private readonly List<Compiled.TapeElement> tape;
            private readonly List<Compiled.InputEdge> edges;
            private readonly Dictionary<Term, int> indexOf;

            public Compiler(T variables, List<Compiled.TapeElement> tape, List<Compiled.InputEdge> edges)
            {
                this.tape = tape;
                this.edges = edges;
                indexOf = new Dictionary<Term, int>();
                for(var i = 0; i < variables.Count; ++i)
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
                return Compile(constant, () => new Compiled.Constant(constant.Value) { Inputs = new Compiled.InputEdges(0,0) });
            }

            public int Visit(Zero zero)
            {
                return Compile(zero, () => new Compiled.Constant(0) { Inputs = new Compiled.InputEdges(0,0) });
            }

            public int Visit(ConstPower intPower)
            {
                return Compile(intPower, () =>
                {
                    var baseIndex = intPower.Base.Accept(this);
                    var element = new Compiled.ConstPower
                    {
                        Exponent = intPower.Exponent,
                        Inputs = MakeInputEdges(() =>  
                        {
                            edges.Add(new Compiled.InputEdge { Index = baseIndex });
                        }),
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
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new Compiled.InputEdge { Index = baseIndex });
                            edges.Add(new Compiled.InputEdge { Index = expIndex });
                        }),
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
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new Compiled.InputEdge { Index = leftIndex });
                            edges.Add(new Compiled.InputEdge { Index = rightIndex });
                        })
                    };

                    return element;
                });
            }

            public int Visit(Sum sum)
            {
                return Compile(sum, () =>
                {
                    var terms = sum.Terms;
                    var indices = new int[terms.Count];
                    for(var i = 0; i < terms.Count; ++i)
                        indices[i] = terms[i].Accept(this);
                    var element = new Compiled.Sum 
                    { 
                        Inputs = MakeInputEdges(() => 
                        {
                            for(var i = 0; i < terms.Count; ++i)
                                edges.Add(new Compiled.InputEdge {Index = indices[i], Weight = 1});
                        })
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
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new Compiled.InputEdge { Index = argIndex });
                        }),
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
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new Compiled.InputEdge { Index = argIndex });
                        }),
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
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new Compiled.InputEdge { Index = argIndex });
                        }),
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

                    var element = new Compiled.BinaryFunc(func.Eval, func.Diff)
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new Compiled.InputEdge { Index = leftIndex });
                            edges.Add(new Compiled.InputEdge { Index = rightIndex });
                        })
                    };

                    return element;
                });
            }

            public int Visit(NaryFunc func)
            {
                return Compile(func, () =>
                {
                    var terms = func.Terms;
                    var indices = new int[terms.Count];
                    for(var i = 0; i < terms.Count; ++i)
                        indices[i] = terms[i].Accept(this);

                    var element = new Compiled.NaryFunc(func.Eval, func.Diff)
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            for(var i = 0; i < terms.Count; ++i)
                                edges.Add(new Compiled.InputEdge {Index = indices[i] });
                        }),
                    };

                    return element;
                });
            }


            private int Compile(Term term, Func<CompileResult> compiler)
            {
                int index;
                if (indexOf.TryGetValue(term, out index)) 
                    return index;
                
                var compileResult = compiler();
                tape.Add(compileResult);

                index = tape.Count - 1;
                indexOf.Add(term, index);

                return index;
            }
            
            private Compiled.InputEdges MakeInputEdges(Action action)
            {
                var offset = edges.Count;
                action();
                var length = edges.Count - offset;
                return new Compiled.InputEdges(offset, length);
            }
        }
    }
}
