using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Represents a custom unary function term. The user provides custom delegates
    /// to evaluate and compute the derivative (differentiate) the function.
    /// </summary>
    public class UnaryFunc : Term
    {
        private readonly Func<double, double> eval;
        private readonly Func<double, double> diff;
        private readonly Term argument;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryFunc"/> class.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        public UnaryFunc(Func<double, double> eval, Func<double, double> diff, Term argument)
        {
            Contract.Requires(eval != null);
            Contract.Requires(diff != null);
            Contract.Requires(argument != null);

            Contract.Ensures(Eval == eval);
            Contract.Ensures(Diff == diff);
            Contract.Ensures(Argument == argument);

            this.eval = eval;
            this.diff = diff;
            this.argument = argument;
        }

        /// <summary>
        /// Gets the evaluation delegate.
        /// </summary>
        public Func<double, double> Eval { get { return eval; } }


        /// <summary>
        /// Gets the differentiation delegate.
        /// </summary>
        public Func<double, double> Diff { get { return diff; } }

        /// <summary>
        /// Gets the function's argument term
        /// </summary>
        public Term Argument { get { return argument; } }

        /// <summary>
        /// Accepts a term visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a term visitor with a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the visitor's function</typeparam>
        /// <param name="visitor">The visitor to accept</param>
        /// <returns>
        /// The result from the visitor's visit function.
        /// </returns>
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
