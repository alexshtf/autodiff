using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Compiles the terms tree to a more efficient form for differentiation.
    /// </summary>
    public partial class CompiledDifferentiator
    {
        private readonly Compiled.Term function;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledDifferentiator"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="variables">The variables.</param>
        public CompiledDifferentiator(Term function, Variable[] variables)
        {
            var compiler = new CompilerVisitor(function, variables);
            this.function = function.Accept(compiler);
        }

        public Tuple<double[], double> Calculate(double[] arg)
        {
            Cleanup();

            var result = function.Accept(new DiffVisitor(arg));
            return Tuple.Create(result.Item1.ToArray(arg.Length), result.Item2);
        }

        private void Cleanup()
        {
            function.Accept(CleanupVisitor.Instance);
        }
    }
}
