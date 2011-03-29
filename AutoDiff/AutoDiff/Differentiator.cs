using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Contains static methods to calculate function gradients.
    /// </summary>
    public static class Differentiator
    {
        /// <summary>
        /// Computes the gradient of a given function at a given point.
        /// </summary>
        /// <param name="term">The function to differentiate</param>
        /// <param name="variables">A vector containing the function's variables.</param>
        /// <param name="values">An array of values, one for each variable in <paramref name="variables"/>.</param>
        /// <returns>The computed gradient of the function represented by <paramref name="term"/>. Each element
        /// in the resulting array is the partial derivative of the corresponding variable from <paramref name="variables"/>
        /// at the point given by <paramref name="values"/>.</returns>
        /// <remarks>The <paramref name="variables"/> array must not contain duplicates. In addition, it must contain
        /// at least all the variables that are used in <paramref name="term"/>.</remarks>
        /// <exception cref="InvalidOperationException">A variable is used in <paramref name="term"/> but is not given
        /// in <paramref name="variables"/>.</exception>
        public static double[] Differentiate(Term term, Variable[] variables, double[] values)
        {
            Contract.Requires(term != null);
            Contract.Requires(variables != null);
            Contract.Requires(values != null);
            Contract.Requires(Contract.ForAll(variables, v => v != null));
            Contract.Requires(variables.Length == values.Length);
            Contract.Ensures(Contract.Result<double[]>().Length == variables.Length);

            var compiled = new CompiledDifferentiator(term, variables);
            return compiled.Calculate(values).Item1;
        }

        /// <summary>
        /// Computes the gradient of a given function at a given point.
        /// </summary>
        /// <param name="term">The function to differentiate</param>
        /// <param name="pairs">A list of (variable, value) pairs. Each such pair represents a single variable
        /// and the value assigned to this variable.</param>
        /// <returns>The computed gradient of the function represented by <paramref name="term"/>. Each element
        /// in the resulting array is the partial derivative of the corresponding variable from <paramref name="pairs"/>
        /// at the point given by the values in <paramref name="pairs"/>.</returns>
        /// <remarks>The <paramref name="pairs"/> list must not contain duplicate variables. In addition, it must contain
        /// at least all the variables that are used in <paramref name="term"/>.</remarks>
        /// <exception cref="InvalidOperationException">A variable is used in <paramref name="term"/> but is not given
        /// in <paramref name="pairs"/>.</exception>
        public static double[] Differentiate(Term term, IList<Tuple<Variable, double>> pairs)
        {
            Contract.Requires(term != null);
            Contract.Requires(pairs != null);
            Contract.Requires(Contract.ForAll(pairs, p => p.Item1 != null));
            Contract.Ensures(Contract.Result<double[]>().Length == pairs.Count);

            var variables = pairs.Select(x => x.Item1).ToArray();
            var values = pairs.Select(x => x.Item2).ToArray();
            return Differentiate(term, variables, values);
        }
    }
}
