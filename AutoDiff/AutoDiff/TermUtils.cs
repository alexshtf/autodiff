using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    public enum CompilationType
    {
        /// <summary>
        /// Compiles faster, but runs slower
        /// </summary>
        Interpreter,

        /// <summary>
        /// Compiles slower but runs faster
        /// </summary>
        ExpressionTree,
    }

    /// <summary>
    /// Static methods that operate on terms.
    /// </summary>
    public static class TermUtils
    {
        private static readonly InterpreterDifferentiatorFactory interpreterDifferentiatorFactory = new InterpreterDifferentiatorFactory();
        private static readonly ExpressionTreeDifferentiatorFactory expressionTreeDifferentiatorFactory = new ExpressionTreeDifferentiatorFactory();

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
            Contract.Requires(variables != null);
            Contract.Requires(term != null);
            Contract.Ensures(Contract.Result<ICompiledTerm>() != null);
            Contract.Ensures(Contract.Result<ICompiledTerm>().Variables.Count == variables.Length);
            Contract.Ensures(Contract.ForAll(0, variables.Length, i => variables[i] == Contract.Result<ICompiledTerm>().Variables[i]));

            return term.Compile(CompilationType.ExpressionTree, variables);
        }

        /// <summary>
        /// Creates a compiled representation of a given term that allows efficient evaluation of the value/gradient.
        /// </summary>
        /// <param name="term">The term to compile.</param>
        /// <param name="variables">The variables contained in the term.</param>
        /// <param name="compilationType">The type of the compilation procedure to perform. See <see cref="CompilationType"/> for more info.</param>
        /// <returns>A compiled representation of <paramref name="term"/> that assigns values to variables in the same order
        /// as in <paramref name="variables"/></returns>
        /// <remarks>
        /// The order of the variables in <paramref name="variables"/> is important. Each call to <c>ICompiledTerm.Evaluate</c> or 
        /// <c>ICompiledTerm.Differentiate</c> receives an array of numbers representing the point of evaluation. The i'th number in this array corresponds
        /// to the i'th variable in <c>variables</c>.
        /// </remarks>
        public static ICompiledTerm Compile(this Term term, CompilationType compilationType, Variable[] variables)
        {
            if (compilationType == CompilationType.Interpreter)
                return interpreterDifferentiatorFactory.Create(term, variables);
            else if (compilationType == CompilationType.ExpressionTree)
                return expressionTreeDifferentiatorFactory.Create(term, variables);
            else
                throw new NotSupportedException();
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
        public static IParametricCompiledTerm Compile(this Term term, Variable[] variables, Variable[] parameters)
        {
            Contract.Requires(variables != null);
            Contract.Requires(parameters != null);
            Contract.Requires(term != null);
            Contract.Ensures(Contract.Result<IParametricCompiledTerm>() != null);
            Contract.Ensures(Contract.Result<IParametricCompiledTerm>().Variables.Count == variables.Length);
            Contract.Ensures(Contract.ForAll(0, variables.Length, i => variables[i] == Contract.Result<IParametricCompiledTerm>().Variables[i]));
            Contract.Ensures(Contract.Result<IParametricCompiledTerm>().Parameters.Count == parameters.Length);
            Contract.Ensures(Contract.ForAll(0, parameters.Length, i => parameters[i] == Contract.Result<IParametricCompiledTerm>().Parameters[i]));

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
            Contract.Requires(term != null);
            Contract.Requires(variables != null);
            Contract.Requires(point != null);
            Contract.Requires(variables.Length == point.Length);

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
        public static double[] Differentiate(this Term term, Variable[] variables, double[] point)
        {
            Contract.Requires(term != null);
            Contract.Requires(variables != null);
            Contract.Requires(point != null);
            Contract.Requires(variables.Length == point.Length);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.Result<double[]>().Length == variables.Length);

            var result =  term.Compile(variables).Differentiate(point).Item1;
            return result;
        }
    }
}
