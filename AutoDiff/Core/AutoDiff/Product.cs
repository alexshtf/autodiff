namespace AutoDiff
{
    /// <summary>
    /// Represents a product between two terms.
    /// </summary>
#if DOTNET
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
