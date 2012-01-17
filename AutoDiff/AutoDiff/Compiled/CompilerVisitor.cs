using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class CompilerVisitor : AutoDiff.ITermVisitor<int> // int --> the index of the compiled element in the tape
    {
        private readonly List<Compiled.TapeElement> tape;
        private readonly List<List<AutoDiff.Compiled.InputConnection>> inputConnections;
        private readonly Dictionary<AutoDiff.Term, int> indexOf;

        public CompilerVisitor(AutoDiff.Variable[] variables, List<Compiled.TapeElement> tape)
        {
            this.tape = tape;
            indexOf = new Dictionary<Term, int>();
            inputConnections = new List<List<Compiled.InputConnection>>();
            foreach (var i in Enumerable.Range(0, variables.Length))
            {
                indexOf[variables[i]] = i;
                tape.Add(new Compiled.Variable());
                inputConnections.Add(new List<Compiled.InputConnection>());
            }
        }

        public void Compile(AutoDiff.Term term)
        {
            term.Accept(this);
            for (int i = 0; i < tape.Count; ++i)
                tape[i].InputOf = inputConnections[i].ToArray();
        }

        public int Visit(AutoDiff.Constant constant)
        {
            return Compile(constant, () =>
                new CompileResult { Element = new Compiled.Constant(constant.Value), InputTapeIndices = new int[0] });
        }

        public int Visit(AutoDiff.Zero zero)
        {
            return Compile(zero, () => new CompileResult { Element = new Compiled.Constant(0), InputTapeIndices = new int[0] });
        }

        public int Visit(AutoDiff.ConstPower intPower)
        {
            return Compile(intPower, () =>
            {
                var baseIndex = intPower.Base.Accept(this);
                var element = new Compiled.ConstPower { Base = baseIndex, Exponent = intPower.Exponent };
                return new CompileResult { Element = element, InputTapeIndices = new int[] { baseIndex } };
            });
        }

        public int Visit(AutoDiff.TermPower power)
        {
            return Compile(power, () =>
            {
                var baseIndex = power.Base.Accept(this);
                var expIndex = power.Exponent.Accept(this);
                var element = new Compiled.TermPower
                {
                    Base = baseIndex,
                    Exponent = expIndex,
                };

                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = new int[] { baseIndex, expIndex },
                };
            });
        }

        public int Visit(AutoDiff.Product product)
        {
            return Compile(product, () =>
            {
                var leftIndex = product.Left.Accept(this);
                var rightIndex = product.Right.Accept(this);
                var element = new Compiled.Product
                {
                    Left = leftIndex,
                    Right = rightIndex,
                };

                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = new int[] { leftIndex, rightIndex },
                };
            });
        }

        public int Visit(AutoDiff.Sum sum)
        {
            return Compile(sum, () =>
            {
                var indicesQuery = from term in sum.Terms
                                   select term.Accept(this);
                var indices = indicesQuery.ToArray();
                var element = new Compiled.Sum { Terms = indices };
                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = indices,
                };
            });
        }

        public int Visit(AutoDiff.Variable variable)
        {
            return indexOf[variable];
        }

        public int Visit(AutoDiff.Log log)
        {
            return Compile(log, () =>
            {
                var argIndex = log.Arg.Accept(this);
                var element = new Compiled.Log { Arg = argIndex };
                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = new int[] { argIndex },
                };
            });
        }

        public int Visit(AutoDiff.Exp exp)
        {
            return Compile(exp, () =>
            {
                var argIndex = exp.Arg.Accept(this);
                var element = new Compiled.Exp { Arg = argIndex };
                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = new int[] { argIndex },
                };
            });
        }

        public int Visit(AutoDiff.UnaryFunc func)
        {
            return Compile(func, () =>
            {
                var argIndex = func.Argument.Accept(this);
                var element = new Compiled.UnaryFunc(func.Eval, func.Diff) { Arg = argIndex };
                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = new int[] { argIndex },
                };
            });
        }

        public int Visit(AutoDiff.BinaryFunc func)
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
                };

                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = new int[] { leftIndex, rightIndex },
                };
            });
        }

        public int Visit(AutoDiff.NaryFunc func)
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
                    Terms = indices
                };

                return new CompileResult
                {
                    Element = element,
                    InputTapeIndices = indices,
                };
            });
        }


        private int Compile(AutoDiff.Term term, Func<CompileResult> compiler)
        {
            int index;
            if (!indexOf.TryGetValue(term, out index))
            {
                var compileResult = compiler();
                tape.Add(compileResult.Element);

                index = tape.Count - 1;
                indexOf.Add(term, index);

                inputConnections.Add(new List<Compiled.InputConnection>());
                for (int i = 0; i < compileResult.InputTapeIndices.Length; ++i)
                {
                    var inputTapeIndex = compileResult.InputTapeIndices[i];
                    inputConnections[inputTapeIndex].Add(new Compiled.InputConnection
                    {
                        IndexOnTape = index,
                        ArgumentIndex = i,
                    });
                }
            }

            return index;
        }

        private class CompileResult
        {
            public Compiled.TapeElement Element;
            public int[] InputTapeIndices;
        }
    }
}
