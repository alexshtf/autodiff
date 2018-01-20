using System;

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
            Guard.NotNull(eval, nameof(eval));
            Guard.NotNull(diff, nameof(diff));
            Guard.NotNull(left, nameof(left));
            Guard.NotNull(right, nameof(right));

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
            Guard.NotNull(eval, nameof(eval));
            Guard.NotNull(diff, nameof(diff));

            return (left, right) => new BinaryFunc(eval, diff, left, right);
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
            Guard.NotNull(visitor, nameof(visitor));
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            Guard.NotNull(visitor, nameof(visitor));
            return visitor.Visit(this);
        }
    }
}
