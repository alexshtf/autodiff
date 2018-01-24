using System;

namespace AutoDiff
{
    /// <summary>
    /// Represents a product between two terms.
    /// </summary>
#if (!NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6)
    [Serializable]
#endif
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
        public Term Left { get; }

        /// <summary>
        /// Gets the second product term.
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
