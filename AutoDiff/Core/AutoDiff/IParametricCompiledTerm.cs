using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using static System.Diagnostics.Contracts.Contract;

namespace AutoDiff
{
    /// <summary>
    /// Represents a parametric term after it has been compiled for efficient evaluation/differentiation. A parametric
    /// term has some variables that function as "constant parameters" and others that function as actual variables.
    /// </summary>
    [ContractClass(typeof(ParametricCompiledTermContract))]
    public interface IParametricCompiledTerm
    {
        /// <summary>
        /// Evaluates the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to evaluate..</param>
        /// <param name="parameters">The parameter values</param>
        /// <returns>The value of the function represented by the term at the given point.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        double Evaluate(double[] arg, double[] parameters);

        /// <summary>
        /// Computes the gradient of the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to differentiate.</param>
        /// <param name="parameters">The parameter values</param>
        /// <returns>A tuple, where the first item is the gradient at <paramref name="arg"/> and the second item is 
        /// the value at <paramref name="arg"/>. That is, the second value is the same as running <see cref="Evaluate"/> on 
        /// <paramref name="arg"/> and <paramref name="parameters"/>.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        Tuple<double[], double> Differentiate(double[] arg, double[] parameters);

        /// <summary>
        /// The collection of variables contained in this compiled term.
        /// </summary>
        /// <remarks>
        /// The order of variables in this collection specifies the meaning of each argument in <see cref="Differentiate"/> or
        /// <see cref="Evaluate"/>. That is, the variable at <c>Variables[i]</c> corresponds to the i-th element in the <c>arg</c> parameter of <see cref="Differentiate"/>
        /// and <see cref="Evaluate"/>.
        /// </remarks>
        IReadOnlyList<Variable> Variables { get; }

        /// <summary>
        /// The collection of parameter variables contained in this compiled term.
        /// </summary>
        /// <remarks>
        /// The order of variables in this collection specifies the meaning of each argument in <see cref="Differentiate"/> or
        /// <see cref="Evaluate"/>. That is, the variable at <c>Variables[i]</c> corresponds to the i-th element in the <c>parameters</c> parameter of <see cref="Differentiate"/>
        /// and <see cref="Evaluate"/>.
        /// </remarks>
        IReadOnlyList<Variable> Parameters { get; }
    }

    [ContractClassFor(typeof(IParametricCompiledTerm))]
    abstract class ParametricCompiledTermContract : IParametricCompiledTerm
    {

        public double Evaluate(double[] arg, double[] parameters)
        {
            Requires(arg != null);
            Requires(arg.Length == Variables.Count);
            Requires(parameters != null);
            Requires(parameters.Length == Parameters.Count);

            return default(double);
        }

        public Tuple<double[], double> Differentiate(double[] arg, double[] parameters)
        {
            Requires(arg != null);
            Requires(arg.Length == Variables.Count);
            Requires(parameters != null);
            Requires(parameters.Length == Parameters.Count);

            Ensures(Result<Tuple<double[], double>>() != null);
            Ensures(Result<Tuple<double[], double>>().Item1.Length == arg.Length);

            return null;
        }

        public IReadOnlyList<Variable> Variables
        {
            get
            {
                Ensures(Result<ReadOnlyCollection<Variable>>() != null);
                return null;
            }
        }

        public IReadOnlyList<Variable> Parameters
        {
            get
            {
                Ensures(Result<ReadOnlyCollection<Variable>>() != null);
                return null;
            }
        }
    }
}
