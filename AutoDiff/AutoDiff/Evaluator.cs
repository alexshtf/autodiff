using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Contains static methods to evaluate the value of a function at a given point.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Evaluates a given function at a given point.
        /// </summary>
        /// <param name="term">The function to evaluate</param>
        /// <param name="variables">An array of variables used in <paramref name="term"/></param>
        /// <param name="values">A corresponding value for each variable in <paramref name="variables"/></param>
        /// <returns>The value of the function <paramref name="term"/> at the point given by <paramref name="variables"/>
        /// and <paramref name="values"/>. For all <c>i</c> we have <c>values[i]</c> is the value given
        /// to the variable <c>variables[i]</c>.</returns>
        /// <remarks>
        /// The array <paramref name="variables"/> must not contain duplicate variables. In addition, it must contain
        /// at least all the variables used in <paramref name="term."/>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown when a variable is used in <paramref name="term"/> but
        /// does not appear in <paramref name="variables"/>.</exception>
        public static double Evaluate(Term term, Variable[] variables, double[] values)
        {
            Contract.Requires(term != null);
            Contract.Requires(variables != null);
            Contract.Requires(values != null);
            Contract.Requires(variables.Length == values.Length);
            Contract.Requires(Contract.ForAll(variables, variable => variable != null));

            var compiled = new CompiledDifferentiator(term, variables);
            return compiled.Eval(values);
        }

        /// <summary>
        /// Evaluates a given function at a given point.
        /// </summary>
        /// <param name="term">The function to evaluate</param>
        /// <param name="values">A mapping of variables to their corresponding values. This mapping defines the point where
        /// the function will be evaluated.</param>
        /// <returns>The value of the function <paramref name="term"/> at the point given by <paramref name="values"/>
        /// <remarks>
        /// The dictionary <paramref name="values"/>  must contain at least all the variables used 
        /// in <paramref name="term."/>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown when a variable is used in <paramref name="term"/> but
        /// does not appear in <paramref name="variablesWithValues"/>.</exception>
        public static double Evaluate(Term term, IDictionary<Variable, double> variablesWithValues)
        {
            Contract.Requires(term != null);
            Contract.Requires(variablesWithValues != null);
            Contract.Requires(Contract.ForAll(variablesWithValues, kvPair => kvPair.Key != null));

            var variables = variablesWithValues.Select(x => x.Key).ToArray();
            var vals = variablesWithValues.Select(x => x.Value).ToArray();
            return Evaluate(term, variables, vals);
        }
    }
}
