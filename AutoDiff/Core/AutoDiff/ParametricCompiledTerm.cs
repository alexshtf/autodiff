using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutoDiff
{
    class ParametricCompiledTerm : IParametricCompiledTerm
    {
        private readonly ICompiledTerm compiledTerm;

        public ParametricCompiledTerm(Term term, IReadOnlyList<Variable> variables, IReadOnlyList<Variable> parameters)
        {
            compiledTerm = term.Compile(variables.Concat(parameters).ToArray());
            Variables = variables.AsReadOnly();
            Parameters = parameters.AsReadOnly();
        }

        public double Evaluate(IReadOnlyList<double> arg, IReadOnlyList<double> parameters)
        {
            var combinedArg = arg.Concat(parameters).ToArray();
            return compiledTerm.Evaluate(combinedArg);
        }

        public Tuple<double[], double> Differentiate(IReadOnlyList<double> arg, IReadOnlyList<double> parameters)
        {
            var combinedArg = arg.Concat(parameters).ToArray();
            var diffResult = compiledTerm.Differentiate(combinedArg);

            var partialGradient = new double[arg.Count];
            Array.Copy(diffResult.Item1, partialGradient, partialGradient.Length);

            return Tuple.Create(partialGradient, diffResult.Item2);
        }

        public IReadOnlyList<Variable> Variables { get; }

        public IReadOnlyList<Variable> Parameters { get; } 
    }
}
