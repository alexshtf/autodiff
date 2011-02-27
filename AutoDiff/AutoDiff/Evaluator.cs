using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    public static class Evaluator
    {
        public static double Evaluate(Term term, Variable[] variables, double[] values)
        {
            Contract.Requires(variables.Length == values.Length);
            Contract.Requires(Contract.ForAll(variables, variable => variable != null));

            var dictionary = new Dictionary<Variable, double>();
            for (int i = 0; i < variables.Length; ++i)
                dictionary.Add(variables[i], values[i]);

            return Evaluate(term, dictionary);
        }

        public static double Evaluate(Term term, IDictionary<Variable, double> values)
        {
            var evaluator = new EvalVisitor(values);
            term.Accept(evaluator);
            return evaluator.Result;
        }

        private class EvalVisitor : ITermVisitor
        {
            private readonly IDictionary<Variable, double> values;

            public EvalVisitor(IDictionary<Variable, double> values)
            {
                this.values = values;
            }

            public double Result { get; private set; }

            public void Visit(Constant constant)
            {
                Result = constant.Value;
            }

            public void Visit(Zero zero)
            {
                Result = 0;
            }

            public void Visit(IntPower intPower)
            {
                intPower.Base.Accept(this);
                Result = Math.Pow(Result, intPower.Exponent);
            }

            public void Visit(Product product)
            {
                product.Left.Accept(this);
                var left = Result;
                
                product.Right.Accept(this);
                var right = Result;

                Result = left * right;
            }

            public void Visit(Sum sum)
            {
                double temp = 0;
                foreach (var term in sum.Terms)
                {
                    term.Accept(this);
                    temp += Result;
                }

                Result = temp;
            }

            public void Visit(Variable variable)
            {
                double value;
                if (values.TryGetValue(variable, out value))
                    Result = value;
                else
                    throw new InvalidOperationException("A variable has no value");
            }


            public void Visit(Log log)
            {
                log.Arg.Accept(this);
                Result = Math.Log(Result);
            }

            public void Visit(Exp exp)
            {
                exp.Arg.Accept(this);
                Result = Math.Exp(Result);
            }


            public void Visit(PiecewiseTerm piecewiseTerm)
            {
                foreach (var pair in piecewiseTerm.Pieces)
                {
                    var inequalityTerm = pair.Item1.Term;
                    inequalityTerm.Accept(this);
                    if (Result <= 0) // we found a piece that satisfies its inequality.
                    {
                        var valueTerm = pair.Item2;
                        valueTerm.Accept(this);
                        return;
                    }
                }

                // if we reached here it means we didn't return in the above loop --> no piece satisfies the conditions.
                throw new InvalidOperationException("Piecewise evaluation failed. No piece satisfies the conditions");
            }
        }

    }
}
