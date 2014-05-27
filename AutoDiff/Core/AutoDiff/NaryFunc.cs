using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.ObjectModel;

namespace AutoDiff
{
    /// <summary>
    /// Represents a custom n-ary function term. The user provides custom delegates
    /// to evaluate and compute the gradient of the function.
    /// </summary>
    public class NaryFunc : Term
    {
        private readonly Func<double[], double> eval;
        private readonly Func<double[], double[]> diff;

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
            this.eval = eval;
            this.diff = diff;
            this.Terms = new ReadOnlyCollection<Term>(terms.ToArray());
        }

        /// <summary>
        /// Constructs a factory delegate that creates similary n-ary functions for different terms.
        /// </summary>
        /// <param name="eval">The evaluation method for the custom function.</param>
        /// <param name="diff">The differentiation method for the custom function.</param>
        /// <returns>The described factory delegate</returns>
        public static Func<IEnumerable<Term>, NaryFunc> Factory(Func<double[], double> eval, Func<double[], double[]> diff)
        {
            Contract.Requires(eval != null);
            Contract.Requires(diff != null);
            Contract.Ensures(Contract.Result<Func<IEnumerable<Term>, NaryFunc>>() != null);

            Func<IEnumerable<Term>, NaryFunc> result = (terms) => new NaryFunc(eval, diff, terms);
            return result;
        }

        /// <summary>
        /// Gets the evaluation delegate
        /// </summary>
        public Func<double[], double> Eval { get { return eval; } }

        /// <summary>
        /// Gets the differentiation delegate
        /// </summary>
        public Func<double[], double[]> Diff { get { return diff; } }

        /// <summary>
        /// Gets the arguments of this function
        /// </summary>
        public ReadOnlyCollection<Term> Terms { get; private set; }

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
