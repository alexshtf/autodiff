using System;
using static System.Diagnostics.Contracts.Contract;

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
            Requires(eval != null);
            Requires(diff != null);
            Requires(argument != null);

            Ensures(Eval == eval);
            Ensures(Diff == diff);
            Ensures(Argument == argument);

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
            Requires(eval != null);
            Requires(diff != null);
            Ensures(Result<Func<Term, UnaryFunc>>() != null);

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
