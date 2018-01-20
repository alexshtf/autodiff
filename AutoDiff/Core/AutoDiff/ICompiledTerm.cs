using System;
using System.Collections.Generic;

namespace AutoDiff
{
    public static class CompiledTerm
    {
        /// <summary>
        /// Computes the gradient of the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to differentiate.</param>
        /// <returns>A tuple, where the first item is the gradient at <paramref name="arg"/> and the second item is 
        /// the value at <paramref name="arg"/>. That is, the second value is the same as running <see cref="Evaluate"/> on 
        /// <paramref name="arg"/>.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        public static Tuple<double[], double> Differentiate(this ICompiledTerm term, IReadOnlyList<double> arg)
        {
            var grad = new double[term.Variables.Count];
            var val = term.Differentiate(arg, grad);
            return Tuple.Create(grad, val);
        }

        /// <summary>
        /// Computes the gradient of the compiled term at the given point.
        /// </summary>
        /// <param name="arg">The point at which to differentiate.</param>
        /// <returns>A tuple, where the first item is the gradient at <paramref name="arg"/> and the second item is 
        /// the value at <paramref name="arg"/>. That is, the second value is the same as running <see cref="Evaluate"/> on 
        /// <paramref name="arg"/>.</returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        public static Tuple<double[], double> Differentiate(this ICompiledTerm term, params double[] arg)
        {
            return Differentiate(term, (IReadOnlyList<double>) arg);
        }
    }
    
    /// <summary>
    /// Represents a term after it has been compiled for efficient evaluation/differentiation.
    /// </summary>
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
        /// Computes gradient of the compiled term at the given point
        /// </summary>
        /// <param name="arg">The point at which to differentiate</param>
        /// <param name="grad">The list to be filled with the gradient at the point specified by <paramref name="arg"/></param>
        /// <returns>The value at the point specified by <paramref name="arg"/></returns>
        /// <remarks>The number at <c>arg[i]</c> is the value assigned to the variable <c>Variables[i]</c>.</remarks>
        double Differentiate(IReadOnlyList<double> arg, IList<double> grad);

        /// <summary>
        /// The list of variables contained in this compiled term.
        /// </summary>
        /// <remarks>
        /// The order of variables in this collection specifies the meaning of each argument in <see cref="Differentiate"/> or
        /// <see cref="Evaluate"/>. That is, the variable at <c>Variables[i]</c> corresponds to the i-th parameter of <see cref="Differentiate"/>
        /// and <see cref="Evaluate"/>.
        /// </remarks>
        IReadOnlyList<Variable> Variables { get; }
    }
}
