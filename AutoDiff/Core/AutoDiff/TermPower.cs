using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    [Serializable]
    public class TermPower : Term
    {
        public TermPower(Term baseTerm, Term exponent)
        {
            Base = baseTerm;
            Exponent = exponent;
        }

        /// <summary>
        /// Gets the base term of the power function
        /// </summary>
        public Term Base { get; private set; }

        /// <summary>
        /// Gets the exponent term of the power function.
        /// </summary>
        public Term Exponent { get; private set; }

        /// <summary>
        /// Accepts a term visitor.
        /// </summary>
        /// <param name="visitor">The term visitor to accept.</param>
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
