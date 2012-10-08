using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AutoDiff
{
    class ParametricCompiledTerm : IParametricCompiledTerm
    {
        private readonly ICompiledTerm compiledTerm;

        public ParametricCompiledTerm(Term term, Variable[] variables, Variable[] parameters)
        {
            compiledTerm = term.Compile(variables.Concat(parameters).ToArray());
            Variables = Array.AsReadOnly(variables.ToArray());
            Parameters = Array.AsReadOnly(parameters.ToArray());
        }

        public double Evaluate(double[] arg, double[] parameters)
        {
            var combinedArg = arg.Concat(parameters).ToArray();
            return compiledTerm.Evaluate(combinedArg);
        }

        public Tuple<double[], double> Differentiate(double[] arg, double[] parameters)
        {
            var combinedArg = arg.Concat(parameters).ToArray();
            var diffResult = compiledTerm.Differentiate(combinedArg);

            var partialGradient = new double[arg.Length];
            Array.Copy(diffResult.Item1, partialGradient, partialGradient.Length);

            return Tuple.Create(partialGradient, diffResult.Item2);
        }

        public ReadOnlyCollection<Variable> Variables { get; private set;}

        public ReadOnlyCollection<Variable> Parameters { get; private set;} 
    }
}
