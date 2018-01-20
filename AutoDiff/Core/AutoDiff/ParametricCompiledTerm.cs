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
            Guard.NotNull(arg, nameof(arg));
            Guard.MustHold(arg.Count == Variables.Count, ErrorMessages.ArgLength);
            Guard.NotNull(parameters, nameof(parameters));
            Guard.MustHold(parameters.Count == Parameters.Count, ErrorMessages.GradLength);

            var combinedArg = arg.Concat(parameters).ToArray();
            return compiledTerm.Evaluate(combinedArg);
        }

        public double Differentiate(IReadOnlyList<double> arg, IReadOnlyList<double> parameters, IList<double> grad)
        {
            Guard.NotNull(arg, nameof(arg));
            Guard.MustHold(arg.Count == Variables.Count, ErrorMessages.ArgLength);
            Guard.NotNull(grad, nameof(grad));
            Guard.MustHold(grad.Count == Variables.Count, ErrorMessages.GradLength);
            Guard.NotNull(parameters, nameof(parameters));
            Guard.MustHold(parameters.Count == Parameters.Count, ErrorMessages.ParamLength);

            var combinedArg = arg.Concat(parameters).ToArray();
            var combinedGrad = new double[combinedArg.Length];
            var val = compiledTerm.Differentiate(combinedArg, combinedGrad);

            for (var i = 0; i < arg.Count; ++i)
                grad[i] = combinedGrad[i];
            return val;
        }

        public IReadOnlyList<Variable> Variables { get; }

        public IReadOnlyList<Variable> Parameters { get; } 
    }
}
