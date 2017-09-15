using System;
using System.Collections.Generic;
using TapeElement = AutoDiff.Compiled.TapeElement;
using InputEdge = AutoDiff.Compiled.InputEdge;
using InputEdges = AutoDiff.Compiled.InputEdges;

namespace AutoDiff
{
    internal partial class CompiledDifferentiator
    {
        
        private class Compiler : ITermVisitor<TapeElement> 
        {
            
            private readonly List<TapeElement> tape;
            private readonly List<InputEdge> edges;
            private readonly Dictionary<Term, TapeElement> tapeElementOf;

            public Compiler(IEnumerable<Variable> variables, List<TapeElement> tape, List<InputEdge> edges)
            {
                this.tape = tape;
                this.edges = edges;
                tapeElementOf = new Dictionary<Term, TapeElement>();
                foreach (var variable in variables)
                {
                    var tapeVariable = new Compiled.Variable();
                    tape.Add(tapeVariable);
                    tapeElementOf[variable] = tapeVariable;
                }
            }

            public void Compile(Term term)
            {
                term.Accept(this);
            }

            public TapeElement Visit(Constant constant)
            {
                return Compile(constant, () => new Compiled.Constant(constant.Value) { Inputs = new InputEdges(0,0) });
            }

            public TapeElement Visit(Zero zero)
            {
                return Compile(zero, () => new Compiled.Constant(0) { Inputs = new InputEdges(0,0) });
            }

            public TapeElement Visit(ConstPower intPower)
            {
                return Compile(intPower, () =>
                {
                    var baseElement = intPower.Base.Accept(this);
                    var element = new Compiled.ConstPower
                    {
                        Exponent = intPower.Exponent,
                        Inputs = MakeInputEdges(() =>  
                        {
                            edges.Add(new InputEdge { Element = baseElement });
                        }),
                    };

                    return element;
                });
            }

            public TapeElement  Visit(TermPower power)
            {
                return Compile(power, () =>
                {
                    var baseElement = power.Base.Accept(this);
                    var expElement = power.Exponent.Accept(this);
                    var element = new Compiled.TermPower
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new InputEdge { Element = baseElement });
                            edges.Add(new InputEdge { Element = expElement });
                        }),
                    };

                    return element;
                });
            }

            public TapeElement Visit(Product product)
            {
                return Compile(product, () =>
                {
                    var leftElement = product.Left.Accept(this);
                    var rightElement = product.Right.Accept(this);
                    var element = new Compiled.Product
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new InputEdge { Element = leftElement });
                            edges.Add(new InputEdge { Element = rightElement });
                        })
                    };

                    return element;
                });
            }

            public TapeElement Visit(Sum sum)
            {
                return Compile(sum, () =>
                {
                    var terms = sum.Terms;
                    var tapeElements = new TapeElement[terms.Count];
                    for(var i = 0; i < terms.Count; ++i)
                        tapeElements[i] = terms[i].Accept(this);
                    var element = new Compiled.Sum 
                    { 
                        Inputs = MakeInputEdges(() => 
                        {
                            for(var i = 0; i < terms.Count; ++i)
                                edges.Add(new InputEdge { Element = tapeElements[i], Weight = 1});
                        })
                    };

                    return element;
                });
            }

            public TapeElement Visit(Variable variable)
            {
                return tapeElementOf[variable];
            }

            public TapeElement Visit(Log log)
            {
                return Compile(log, () =>
                {
                    var argElement = log.Arg.Accept(this);
                    var element = new Compiled.Log 
                    { 
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new InputEdge { Element = argElement });
                        }),
                    };

                    return element;
                });
            }

            public TapeElement Visit(Exp exp)
            {
                return Compile(exp, () =>
                {
                    var argElement = exp.Arg.Accept(this);
                    var element = new Compiled.Exp
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new InputEdge { Element = argElement });
                        }),
                    };

                    return element;
                });
            }

            public TapeElement Visit(UnaryFunc func)
            {
                return Compile(func, () =>
                {
                    var argElement = func.Argument.Accept(this);
                    var element = new Compiled.UnaryFunc(func.Eval, func.Diff)
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new InputEdge { Element = argElement });
                        }),
                    };

                    return element;
                });
            }

            public TapeElement Visit(BinaryFunc func)
            {
                return Compile(func, () =>
                {
                    var leftElement = func.Left.Accept(this);
                    var rightElement = func.Right.Accept(this);

                    var element = new Compiled.BinaryFunc(func.Eval, func.Diff)
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            edges.Add(new InputEdge { Element = leftElement });
                            edges.Add(new InputEdge { Element = rightElement });
                        })
                    };

                    return element;
                });
            }

            public TapeElement Visit(NaryFunc func)
            {
                return Compile(func, () =>
                {
                    var terms = func.Terms;
                    var indices = new TapeElement[terms.Count];
                    for(var i = 0; i < terms.Count; ++i)
                        indices[i] = terms[i].Accept(this);

                    var element = new Compiled.NaryFunc(func.Eval, func.Diff)
                    {
                        Inputs = MakeInputEdges(() => 
                        {
                            for(var i = 0; i < terms.Count; ++i)
                                edges.Add(new InputEdge { Element = indices[i] });
                        }),
                    };

                    return element;
                });
            }


            private TapeElement Compile(Term term, Func<TapeElement> compiler)
            {
                TapeElement element;
                if (tapeElementOf.TryGetValue(term, out element)) 
                    return element;
                
                tape.Add(element = compiler());
                tapeElementOf[term] = element;

                return element;
            }
            
            private InputEdges MakeInputEdges(Action action)
            {
                var offset = edges.Count;
                action();
                var length = edges.Count - offset;
                return new InputEdges(offset, length);
            }
        }
    }
}
