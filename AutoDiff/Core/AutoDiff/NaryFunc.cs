using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoDiff
{
    /// <summary>
    /// Represents a custom n-ary function term. The user provides custom delegates
    /// to evaluate and compute the gradient of the function.
    /// </summary>
    public class NaryFunc : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaryFunc"/> class.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <param name="terms">The argument terms for the n-ary function.</param>
        public NaryFunc(
            Func<double[], double> eval,
            Func<double[], double[]> diff,
            IEnumerable<Term> terms)
        {
            Guard.NotNull(eval, nameof(eval));
            Guard.NotNull(diff, nameof(diff));
            Guard.NotNullOrEmpty(terms, nameof(terms));

            Eval = eval;
            Diff = diff;
            Terms = terms.ToList().AsReadOnly();
        }

        /// <summary>
        /// Constructs a factory delegate that creates similary n-ary functions for different terms.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <returns>The described factory delegate</returns>
        public static Func<IEnumerable<Term>, NaryFunc> Factory(Func<double[], double> eval, Func<double[], double[]> diff)
        {
            Guard.NotNull(eval, nameof(eval));
            Guard.NotNull(diff, nameof(diff));

            Func<IEnumerable<Term>, NaryFunc> result = (terms) => new NaryFunc(eval, diff, terms);
            return result;
        }

        /// <summary>
        /// Gets the evaluation delegate
        /// </summary>
        public Func<double[], double> Eval { get; }

        /// <summary>
        /// Gets the differentiation delegate
        /// </summary>
        public Func<double[], double[]> Diff { get; }

        /// <summary>
        /// Gets the arguments of this function
        /// </summary>
        public IReadOnlyList<Term> Terms { get; }

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
