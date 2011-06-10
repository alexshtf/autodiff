using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Represents a custom binary function term. The user provides custom delegates
    /// to evaluate and compute the gradient of the function.
    /// </summary>
    public class BinaryFunc : Term
    {
        private readonly Func<double, double, double> eval;
        private readonly Func<double, double, Tuple<double, double>> diff;
        private readonly Term left;
        private readonly Term right;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFunc"/> class.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <param name="left">The left argument term for the binary function.</param>
        /// <param name="right">The right argument term for the binary function.</param>
        public BinaryFunc(
            Func<double, double, double> eval, 
            Func<double, double, Tuple<double, double>> diff,
            Term left, Term right)
        {
            Contract.Requires(eval != null);
            Contract.Requires(diff != null);
            Contract.Requires(left != null);
            Contract.Requires(right != null);

            Contract.Ensures(Eval == eval);
            Contract.Ensures(Diff == diff);
            Contract.Ensures(Left == left);
            Contract.Ensures(Right == right);

            this.eval = eval;
            this.diff = diff;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Constructs a factory delegate that creates similary binary functions for different terms.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <returns>The described factory delegate</returns>
        public static Func<Term, Term, BinaryFunc> Factory(Func<double, double, double> eval, Func<double, double, Tuple<double, double>> diff)
        {
            Contract.Requires(eval != null);
            Contract.Requires(diff != null);
            Contract.Ensures(Contract.Result < Func<Term, Term, BinaryFunc>>() != null);

            Func<Term, Term, BinaryFunc> result = (left, right) => new BinaryFunc(eval, diff, left, right);
            return result;
        }

        /// <summary>
        /// Gets the evaluation delegate
        /// </summary>
        public Func<double, double, double> Eval { get { return eval; } }

        /// <summary>
        /// Gets the differentiation delegate
        /// </summary>
        public Func<double, double, Tuple<double, double>> Diff { get { return diff; } }

        /// <summary>
        /// Gets the function's left argument term
        /// </summary>
        public Term Left { get { return left; } }

        /// <summary>
        /// Gets the function's right argument term
        /// </summary>
        public Term Right { get { return right; } }

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
        /// <returns>The result from the visitor's visit function.</returns>
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
