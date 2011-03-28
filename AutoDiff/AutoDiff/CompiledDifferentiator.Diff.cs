using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public partial class CompiledDifferentiator
    {
        private class DiffVisitor : Compiled.ITermVisitor<Tuple<SparseVector, double>>
        {
            private static readonly SparseVector ZeroVector = new SparseVector();
            private readonly double[] values;

            public DiffVisitor(double[] values)
            {
                this.values = values;
            }

            public Tuple<SparseVector, double> Visit(Compiled.Constant term)
            {
                var gradient = ZeroVector;
                var value = term.Multiplier;

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Log term)
            {
                var argDiff = term.Arg.Accept(this);
                var gradient = SparseVector.Scale(argDiff.Item1, term.Multiplier / argDiff.Item2);
                var value = term.Multiplier * Math.Log(argDiff.Item2);

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Exp term)
            {
                var argDiff = term.Arg.Accept(this);
                var gradient = SparseVector.Scale(argDiff.Item1, term.Multiplier * Math.Exp(argDiff.Item2));
                var value = term.Multiplier * Math.Exp(argDiff.Item2);

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Piecewise term)
            {
                SparseVector gradient = null;
                double value = 0.0;

                foreach (var index in Enumerable.Range(0, term.Inequalities.Length))
                {
                    var inequalityDiff = term.Inequalities[index].Accept(this);
                    if (inequalityDiff.Item2 <= 0)
                    {
                        var termDiff = term.Terms[index].Accept(this);
                        gradient = SparseVector.Scale(termDiff.Item1, term.Multiplier);
                        value = termDiff.Item2 * term.Multiplier;
                    }
                }

                if (gradient == null)
                    throw new InvalidOperationException("Piecewise differentiation failed. No inequality was satisfied by the argument");

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Power term)
            {
                var baseDiff = term.Base.Accept(this);
                var gradient = SparseVector.Scale(
                    baseDiff.Item1, 
                    term.Exponent * term.Multiplier * Math.Pow(baseDiff.Item2, term.Exponent - 1));
                var value = term.Multiplier * Math.Pow(baseDiff.Item2, term.Exponent);

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Product term)
            {
                var leftDiff = term.Left.Accept(this);
                var rightDiff = term.Right.Accept(this);
                var value = term.Multiplier * leftDiff.Item2 * rightDiff.Item2;
                var gradient = SparseVector.Sum(
                    SparseVector.Scale(leftDiff.Item1, rightDiff.Item2 * term.Multiplier),
                    SparseVector.Scale(rightDiff.Item1, leftDiff.Item2 * term.Multiplier));

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Sum term)
            {
                var gradient = ZeroVector;
                var value = term.Bias;
                foreach (var child in term.Terms)
                {
                    var childDiff = child.Accept(this);
                    value += childDiff.Item2;
                    gradient = SparseVector.Sum(gradient, childDiff.Item1);
                }
                value = value * term.Multiplier;
                gradient = SparseVector.Scale(gradient, term.Multiplier);

                return GetResult(term, gradient, value);
            }

            public Tuple<SparseVector, double> Visit(Compiled.Variable term)
            {
                var gradient = new SparseVector(term.Index, term.Multiplier);
                var value = values[term.Index] * term.Multiplier;

                return GetResult(term, gradient, value);
            }

            private Tuple<SparseVector, double> GetResult(Compiled.Term term, SparseVector gradient, double value)
            {
                return Tuple.Create(gradient, value);
            }
        }

    }
}
