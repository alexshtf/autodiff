using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Represents a product between two terms.
    /// </summary>
    [Serializable]
    public class Product : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Product"/> type.
        /// </summary>
        /// <param name="left">The first product term</param>
        /// <param name="right">The second product term</param>
        public Product(Term left, Term right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Gets the first product term.
        /// </summary>
        public Term Left { get; private set; }

        /// <summary>
        /// Gets the second product term.
        /// </summary>
        public Term Right { get; private set; }

        /// <summary>
        /// Accepts a term visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
