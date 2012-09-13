using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Represents a term after it has been compiled for efficient evaluation/differentiation.
    /// </summary>
    [ContractClass(typeof(CompiledTermContract))]
    public interface ICompiledTerm
    {
        /// <summary>
        /// Evaluates the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to evaluate.</param>
        /// <returns>The value of the function represented by the term at the given point.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        double Evaluate(params double[] arg);

        /// <summary>
        /// Computes the gradient of the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to differentiate.</param>
        /// <returns>A tuple, where the first item is the gradient at <paramref name="arg"/> and the second item is 
        /// the value at <paramref name="arg"/>. That is, the second value is the same as running <see cref="Evaluate"/> on 
        /// <paramref name="arg"/>.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        Tuple<double[], double> Differentiate<T>(T arg) where T : IList<double>;

        /// <summary>
        /// Computes the gradient of the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to differentiate.</param>
        /// <returns>A tuple, where the first item is the gradient at <paramref name="arg"/> and the second item is 
        /// the value at <paramref name="arg"/>. That is, the second value is the same as running <see cref="Evaluate"/> on 
        /// <paramref name="arg"/>.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        Tuple<double[], double> Differentiate(params double[] arg);

        /// <summary>
        /// The collection of variables contained in this compiled term.
        /// </summary>
        /// <remarks>
        /// The order of variables in this collection specifies the meaning of each argument in <see cref="Differentiate"/> or
        /// <see cref="Evaluate"/>. That is, the variable at <c>Variables[i]</c> corresponds to the i-th parameter of <see cref="Differentiate"/>
        /// and <see cref="Evaluate"/>.
        /// </remarks>
        ReadOnlyCollection<Variable> Variables { get; }
    }

    [ContractClassFor(typeof(ICompiledTerm))]
    abstract class CompiledTermContract : ICompiledTerm
    {
        public double Evaluate(params double[] arg)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg.Length == Variables.Count);
            return default(double);
        }

        public Tuple<double[], double> Differentiate<T>(T arg)
            where T : IList<double>
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg.Count == Variables.Count);
            Contract.Ensures(Contract.Result<Tuple<double[], double>>() != null);
            Contract.Ensures(Contract.Result<Tuple<double[], double>>().Item1.Length == arg.Count);
            return null;
        }

        public Tuple<double[], double> Differentiate(params double[] arg)
        {
            Contract.Requires(arg != null);
            Contract.Requires(arg.Length == Variables.Count);
            Contract.Ensures(Contract.Result<Tuple<double[], double>>() != null);
            Contract.Ensures(Contract.Result<Tuple<double[], double>>().Item1.Length == arg.Length);
            return null;
        }

        public ReadOnlyCollection<Variable> Variables
        {
            get 
            { 
                Contract.Ensures(Contract.Result<ReadOnlyCollection<Variable>>() != null);
                return null;
            }
        }
    }

}
