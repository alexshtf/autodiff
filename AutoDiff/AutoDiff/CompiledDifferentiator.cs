using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace AutoDiff
{
    /// <summary>
    /// Compiles the terms tree to a more efficient form for differentiation.
    /// </summary>
    public partial class CompiledDifferentiator
    {
        private readonly Compiled.TapeElement[] tape;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledDifferentiator"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="variables">The variables.</param>
        public CompiledDifferentiator(Term function, Variable[] variables)
        {
            Contract.Requires(function != null);
            Contract.Requires(variables != null);
            Contract.Requires(Contract.ForAll(variables, variable => variable != null));
            Contract.Ensures(Dimension == variables.Length);

            var tapeList = new List<Compiled.TapeElement>();
            new Compiler(variables, tapeList).Compile(function);
            tape = tapeList.ToArray();

            Dimension = variables.Length;
        }

        public int Dimension { get; private set; }

        public Tuple<double[], double> Calculate(double[] arg)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg.Length == Dimension);

            EvaluateTape(arg);
            DifferetiateTape();

            var gradient = tape.Take(Dimension).Select(elem => elem.Derivative).ToArray();
            var value = tape.Last().Value;

            return Tuple.Create(gradient, value);
        }

        private void DifferetiateTape()
        {
            tape.Last().Derivative = 1; // derivative of the last variable with respect to itself is 1.
            for (int i = tape.Length - 2; i >= 0; --i)
            {
                tape[i].Derivative = 0;
                foreach (var connection in tape[i].InputOf)
                {
                    Debug.Assert(connection.IndexOnTape > i);

                    var inputElement = tape[connection.IndexOnTape];
                    double localDerivative = double.NaN;

                    var exp = inputElement as Compiled.Exp;
                    if (exp != null)
                        localDerivative = inputElement.Derivative * inputElement.Value;

                    var log = inputElement as Compiled.Log;
                    if (log != null)
                        localDerivative = inputElement.Derivative / ValueOf(log.Arg);

                    var power = inputElement as Compiled.Power;
                    if (power != null)
                        localDerivative = 
                            inputElement.Derivative * power.Exponent * Math.Pow(ValueOf(power.Base), power.Exponent - 1);

                    var product = inputElement as Compiled.Product;
                    if (product != null)
                    {
                        Debug.Assert(connection.ArgumentIndex == 0 || connection.ArgumentIndex == 1);
                        if (connection.ArgumentIndex == 0)
                            localDerivative = inputElement.Derivative * ValueOf(product.Right);
                        else
                            localDerivative = inputElement.Derivative * ValueOf(product.Left);
                    }

                    var sum = inputElement as Compiled.Sum;
                    if (sum != null)
                        localDerivative = inputElement.Derivative;

                    Debug.Assert(!double.IsNaN(localDerivative));
                    tape[i].Derivative += localDerivative;
                }
            }
        }

        private void EvaluateTape(double[] arg)
        {
            foreach (var i in Enumerable.Range(0, Dimension))
                tape[i].Value = arg[i];
            foreach (var i in Enumerable.Range(Dimension, tape.Length - Dimension))
            {
                var exp = tape[i] as Compiled.Exp;
                if (exp != null)
                {
                    exp.Value = Math.Exp(ValueOf(exp.Arg));
                    continue;
                }

                var log = tape[i] as Compiled.Log;
                if (log != null)
                {
                    log.Value = Math.Log(ValueOf(log.Arg));
                    continue;
                }

                var power = tape[i] as Compiled.Power;
                if (power != null)
                {
                    power.Value = Math.Pow(ValueOf(power.Base), power.Exponent);
                    continue;
                }

                var product = tape[i] as Compiled.Product;
                if (product != null)
                {
                    product.Value = ValueOf(product.Left) * ValueOf(product.Right);
                    continue;
                }

                var sum = tape[i] as Compiled.Sum;
                if (sum != null)
                {
                    sum.Value = 0;
                    foreach(var term in sum.Terms)
                        sum.Value += ValueOf(term);
                    continue;
                }

                if (!(tape[i] is Compiled.Constant))
                    throw new InvalidOperationException("There is a bug!!");
            }
        }

        private double ValueOf(int index)
        {
            return tape[index].Value;
        }
    }
}
