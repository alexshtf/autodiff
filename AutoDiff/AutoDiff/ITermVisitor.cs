using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Visitor for terms that has no result from its computations.
    /// </summary>
    public interface ITermVisitor
    {
        /// <summary>
        /// Performs an action for a constant term.
        /// </summary>
        /// <param name="constant">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Constant constant);

        /// <summary>
        /// Performs an action for a zero term.
        /// </summary>
        /// <param name="zero">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Zero zero);

        /// <summary>
        /// Performs an action for a power term.
        /// </summary>
        /// <param name="intPower">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(IntPower intPower);

        /// <summary>
        /// Performs an action for a product term.
        /// </summary>
        /// <param name="product">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Product product);

        /// <summary>
        /// Performs an action for a sum term.
        /// </summary>
        /// <param name="sum">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Sum sum);

        /// <summary>
        /// Performs an action for a variable term.
        /// </summary>
        /// <param name="variable">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Variable variable);

        /// <summary>
        /// Performs an action for a logarithm term.
        /// </summary>
        /// <param name="log">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Log log);

        /// <summary>
        /// Performs an action for an exponential function term.
        /// </summary>
        /// <param name="exp">The input term.</param>
        /// <returns>The result of the computation.</returns>
        void Visit(Exp exp);
    }
}
