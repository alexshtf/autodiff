using System;
using static System.Diagnostics.Contracts.Contract;

namespace AutoDiff
{
    /// <summary>
    /// Represents a custom binary function term. The user provides custom delegates
    /// to evaluate and compute the gradient of the function.
    /// </summary>
    public class BinaryFunc : Term
    {
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
            Requires(eval != null);
            Requires(diff != null);
            Requires(left != null);
            Requires(right != null);

            Ensures(Eval == eval);
            Ensures(Diff == diff);
            Ensures(Left == left);
            Ensures(Right == right);

            Eval = eval;
            Diff = diff;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Constructs a factory delegate that creates similary binary functions for different terms.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <returns>The described factory delegate</returns>
        public static Func<Term, Term, BinaryFunc> Factory(
            Func<double, double, double> eval, 
            Func<double, double, Tuple<double, double>> diff)
        {
            Requires(eval != null);
            Requires(diff != null);
            Ensures(Result<Func<Term, Term, BinaryFunc>>() != null);

            Func<Term, Term, BinaryFunc> result = (left, right) => new BinaryFunc(eval, diff, left, right);
            return result;
        }

        /// <summary>
        /// Gets the evaluation delegate
        /// </summary>
        public Func<double, double, double> Eval { get; }

        /// <summary>
        /// Gets the differentiation delegate
        /// </summary>
        public Func<double, double, Tuple<double, double>> Diff { get; }

        /// <summary>
        /// Gets the function's left argument term
        /// </summary>
        public Term Left { get; }

        /// <summary>
        /// Gets the function's right argument term
        /// </summary>
        public Term Right { get; }

        /// <inheritdoc />
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
