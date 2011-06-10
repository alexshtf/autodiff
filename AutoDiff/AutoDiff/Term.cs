using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Base class for all automatically-differentiable terms.
    /// </summary>
    [Serializable]
    [ContractClass(typeof(TermContacts))]
    public abstract class Term
    {
        /// <summary>
        /// Accepts a term visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept</param>
        public abstract void Accept(ITermVisitor visitor);

        /// <summary>
        /// Accepts a term visitor with a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the visitor's function</typeparam>
        /// <param name="visitor">The visitor to accept</param>
        /// <returns>The result from the visitor's visit function.</returns>
        public abstract TResult Accept<TResult>(ITermVisitor<TResult> visitor);

        /// <summary>
        /// Converts a floating point constant to a constant term.
        /// </summary>
        /// <param name="value">The floating point constnat</param>
        /// <returns>The resulting term.</returns>
        public static implicit operator Term(double value)
        {
            return TermBuilder.Constant(value);
        }

        /// <summary>
        /// Constructs a sum of the two given terms.
        /// </summary>
        /// <param name="left">First term in the sum</param>
        /// <param name="right">Second term in the sum</param>
        /// <returns>A term representing the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Term operator+(Term left, Term right)
        {
            if (left is Zero && right is Zero)
                return new Zero();
            else if (left is Zero)
                return right;
            else if (right is Zero)
                return left;
            else
                return TermBuilder.Sum(left, right);
        }

        /// <summary>
        /// Constructs a product term of the two given terms.
        /// </summary>
        /// <param name="left">The first term in the product</param>
        /// <param name="right">The second term in the product</param>
        /// <returns>A term representing the product of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Term operator*(Term left, Term right)
        {
            return TermBuilder.Product(left, right);
        }

        /// <summary>
        /// Constructs a difference of the two given terms.
        /// </summary>
        /// <param name="left">The first term in the difference</param>
        /// <param name="right">The second term in the difference.</param>
        /// <returns>A term representing <paramref name="left"/> - <paramref name="right"/>.</returns>
        public static Term operator-(Term left, Term right)
        {
            return left + (-1) * right;
        }

        /// <summary>
        /// Constructs a negated term
        /// </summary>
        /// <param name="term">The term to negate</param>
        /// <returns>A term representing <c>-term</c>.</returns>
        public static Term operator-(Term term)
        {
            return (-1) * term;
        }
    }

    [ContractClassFor(typeof(Term))]
    abstract class TermContacts : Term
    {
        public override void Accept(ITermVisitor visitor)
        {
            Contract.Requires(visitor != null);
        }

        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            Contract.Requires(visitor != null);
            return default(TResult);
        }
    }

}
