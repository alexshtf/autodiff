using System.Collections.Generic;
using static System.Diagnostics.Contracts.Contract;

namespace AutoDiff
{
    /// <summary>
    /// Static methods that operate on terms.
    /// </summary>
    public static class TermUtils
    {
        /// <summary>
        /// Creates a compiled representation of a given term that allows efficient evaluation of the value/gradient.
        /// </summary>
        /// <param name="term">The term to compile.</param>
        /// <param name="variables">The variables contained in the term.</param>
        /// <returns>A compiled representation of <paramref name="term"/> that assigns values to variables in the same order
        /// as in <paramref name="variables"/></returns>
        /// <remarks>
        /// The order of the variables in <paramref name="variables"/> is important. Each call to <c>ICompiledTerm.Evaluate</c> or 
        /// <c>ICompiledTerm.Differentiate</c> receives an array of numbers representing the point of evaluation. The i'th number in this array corresponds
        /// to the i'th variable in <c>variables</c>.
        /// </remarks>
        public static ICompiledTerm Compile(this Term term, params Variable[] variables)
        {
            return Compile(term, (IReadOnlyList<Variable>)variables);
        }

        /// <summary>
        /// Creates a compiled representation of a given term that allows efficient evaluation of the value/gradient.
        /// </summary>
        /// <param name="term">The term to compile.</param>
        /// <param name="variables">The variables contained in the term.</param>
        /// <returns>A compiled representation of <paramref name="term"/> that assigns values to variables in the same order
        /// as in <paramref name="variables"/></returns>
        /// <remarks>
        /// The order of the variables in <paramref name="variables"/> is important. Each call to <c>ICompiledTerm.Evaluate</c> or 
        /// <c>ICompiledTerm.Differentiate</c> receives an array of numbers representing the point of evaluation. The i'th number in this array corresponds
        /// to the i'th variable in <c>variables</c>.
        /// </remarks>
        public static ICompiledTerm Compile(this Term term, IReadOnlyList<Variable> variables)
        {
            Requires(variables != null);
            Requires(term != null);
            Ensures(Result<ICompiledTerm>() != null);
            Ensures(Result<ICompiledTerm>().Variables.Count == variables.Count);
            Ensures(ForAll(0, variables.Count, i => variables[i] == Result<ICompiledTerm>().Variables[i]));

            return new CompiledDifferentiator(term, variables);
        }

        /// <summary>
        /// Creates a compiled representation of a given term that allows efficient evaluation of the value/gradient where part of the variables serve as function
        /// inputs and other variables serve as constant parameters.
        /// </summary>
        /// <param name="term">The term to compile.</param>
        /// <param name="variables">The variables contained in the term.</param>
        /// <param name="parameters">The constant parameters in the term.</param>
        /// <returns>A compiled representation of <paramref name="term"/> that assigns values to variables in the same order
        /// as in <paramref name="variables"/> and <paramref name="parameters"/></returns>
        /// <remarks>
        /// The order of the variables in <paramref name="variables"/> is important. Each call to <c>ICompiledTerm.Evaluate</c> or 
        /// <c>ICompiledTerm.Differentiate</c> receives an array of numbers representing the point of evaluation. The i'th number in this array corresponds
        /// to the i'th variable in <c>variables</c>.
        /// </remarks>
        public static IParametricCompiledTerm Compile(this Term term, IReadOnlyList<Variable> variables, IReadOnlyList<Variable> parameters)
        {
            Requires(variables != null);
            Requires(parameters != null);
            Requires(term != null);
            Ensures(Result<IParametricCompiledTerm>() != null);
            Ensures(Result<IParametricCompiledTerm>().Variables.Count == variables.Count);
            Ensures(ForAll(0, variables.Count, i => variables[i] == Result<IParametricCompiledTerm>().Variables[i]));
            Ensures(Result<IParametricCompiledTerm>().Parameters.Count == parameters.Count);
            Ensures(ForAll(0, parameters.Count, i => parameters[i] == Result<IParametricCompiledTerm>().Parameters[i]));

            return new ParametricCompiledTerm(term, variables, parameters);
        }

        /// <summary>
        /// Evaluates the function represented by a given term at a given point.
        /// </summary>
        /// <param name="term">The term representing the function to evaluate.</param>
        /// <param name="variables">The variables used in <paramref name="term"/>.</param>
        /// <param name="point">The values assigned to the variables in <paramref name="variables"/></param>
        /// <returns>The value of the function represented by <paramref name="term"/> at the point represented by <paramref name="variables"/>
        /// and <paramref name="point"/>.</returns>
        /// <remarks>The i'th value in <c>point</c> corresponds to the i'th variable in <c>variables</c>.</remarks>
        public static double Evaluate(this Term term, Variable[] variables, double[] point)
        {
            Requires(term != null);
            Requires(variables != null);
            Requires(point != null);
            Requires(variables.Length == point.Length);

            return term.Compile(variables).Evaluate(point);
        }

        /// <summary>
        /// Computes the gradient of the function represented by a given term at a given point.
        /// </summary>
        /// <param name="term">The term representing the function to differentiate.</param>
        /// <param name="variables">The variables used in <paramref name="term"/>.</param>
        /// <param name="point">The values assigned to the variables in <paramref name="variables"/></param>
        /// <returns>The gradient of the function represented by <paramref name="term"/> at the point represented by <paramref name="variables"/>
        /// and <paramref name="point"/>.</returns>
        /// <remarks>The i'th value in <c>point</c> corresponds to the i'th variable in <c>variables</c>. In addition, the i'th value
        /// in the resulting array is the partial derivative with respect to the i'th variable in <c>variables</c>.</remarks>
        public static double[] Differentiate(this Term term, IReadOnlyList<Variable> variables, IReadOnlyList<double> point)
        {
            Requires(term != null);
            Requires(variables != null);
            Requires(point != null);
            Requires(variables.Count == point.Count);
            Ensures(Result<double[]>() != null);
            Ensures(Result<double[]>().Length == variables.Count);

            var result =  term.Compile(variables).Differentiate(point).Item1;
            return result;
        }
    }
}
