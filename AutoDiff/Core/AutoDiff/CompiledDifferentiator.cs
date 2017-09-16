using System;
using System.Collections.Generic;
using static System.Diagnostics.Contracts.Contract;
using TapeElement = AutoDiff.Compiled.TapeElement;
using InputEdge = AutoDiff.Compiled.InputEdge;

namespace AutoDiff
{
    /// <inheritdoc />
    /// <summary>
    /// Compiles the terms tree to a more efficient form for differentiation.
    /// </summary>
    internal partial class CompiledDifferentiator : ICompiledTerm
    {
        private readonly TapeElement[] tape;
        private readonly int dimension;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledDifferentiator"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="variables">The variables.</param>
        public CompiledDifferentiator(Term function, IReadOnlyList<Variable> variables)
        {
            Variables = variables.AsReadOnly();
            dimension = variables.Count;

            if (function is Variable)
                function = new ConstPower(function, 1);

            var tapeList = new List<TapeElement>();
            var inputList = new List<InputEdge>();
            new Compiler(variables, tapeList, inputList).Compile(function);
            tape = tapeList.ToArray();
            
            var inputEdges = inputList.ToArray();
            foreach(var te in tape)
                te.Inputs = te.Inputs.Remap(inputEdges);
        }

        public IReadOnlyList<Variable> Variables { get; }

        public double Evaluate(double[] arg)
        {
            EvaluateTape(arg);
            return tape[tape.Length - 1].Value;
        }

        public double Differentiate(IReadOnlyList<double> arg, IList<double> grad) 
        {
            ForwardSweep(arg);
            ReverseSweep();

            for (var i = 0; i < dimension; ++i)
                grad[i] = tape[i].Adjoint;

            return tape[tape.Length - 1].Value;
        }

        private void ReverseSweep()
        {
            // initialize adjoints
            for (var i = 0; i < tape.Length - 1; ++i)
                tape[i].Adjoint = 0;
            tape[tape.Length - 1].Adjoint = 1;

            // accumulate adjoints
            for (var i = tape.Length - 1; i >= dimension; --i)
            {
                var inputs = tape[i].Inputs;
                var adjoint = tape[i].Adjoint;
                
                for(var j = 0; j < inputs.Length; ++j)
                    inputs.Element(j).Adjoint += adjoint * inputs.Weight(j);
            }
        }

        private void ForwardSweep(IReadOnlyList<double> arg)
        {
            for (var i = 0; i < dimension; ++i)
                tape[i].Value = arg[i];

            for (var i = dimension; i < tape.Length; ++i)
                tape[i].Diff();
        }

        private void EvaluateTape(IReadOnlyList<double> arg)
        {
            for(var i = 0; i < dimension; ++i)
                tape[i].Value = arg[i];
            
            for (var i = dimension; i < tape.Length; ++i )
                tape[i].Eval();
        }
    }
}
