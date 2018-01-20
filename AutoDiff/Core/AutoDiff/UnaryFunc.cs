using System;

namespace AutoDiff
{
    /// <summary>
    /// Represents a custom unary function term. The user provides custom delegates
    /// to evaluate and compute the derivative (differentiate) the function.
    /// </summary>
    public class UnaryFunc : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryFunc"/> class.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <param name="argument">The argument term for the unary function</param>
        public UnaryFunc(Func<double, double> eval, Func<double, double> diff, Term argument)
        {
            Guard.NotNull(eval, nameof(eval));
            Guard.NotNull(diff, nameof(diff));
            Guard.NotNull(argument, nameof(argument));

            Eval = eval;
            Diff = diff;
            Argument = argument;
        }

        /// <summary>
        /// Constructs a factory delegate that creates similar unary functions for different terms.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <returns>The described factory delegate</returns>
        public static Func<Term, UnaryFunc> Factory(Func<double, double> eval, Func<double, double> diff)
        {
            Func<Term, UnaryFunc> result = term => new UnaryFunc(eval, diff, term);
            return result;
        }

        /// <summary>
        /// Gets the evaluation delegate.
        /// </summary>
        public Func<double, double> Eval { get; }


        /// <summary>
        /// Gets the differentiation delegate.
        /// </summary>
        public Func<double, double> Diff { get; }

        /// <summary>
        /// Gets the function's argument term
        /// </summary>
        public Term Argument { get; }

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
