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
        /// <returns>A term representing the sum of <see cref="left"/> and <see cref="right"/>.</returns>
        public static Term operator+(Term left, Term right)
        {
            return TermBuilder.Sum(left, right);
        }

        /// <summary>
        /// Constructs a product term of the two given terms.
        /// </summary>
        /// <param name="left">The first term in the product</param>
        /// <param name="right">The second term in the product</param>
        /// <returns>A term representing the product of <see cref="left"/> and <see cref="right"/>.</returns>
        public static Term operator*(Term left, Term right)
        {
            return TermBuilder.Product(left, right);
        }

        public static Term operator-(Term left, Term right)
        {
            return left + (-1) * right;
        }
    }

    [ContractClassFor(typeof(Term))]
    class TermContacts : Term
    {
        public override void Accept(ITermVisitor visitor)
        {
            Contract.Requires(visitor != null);
        }
    }

}
