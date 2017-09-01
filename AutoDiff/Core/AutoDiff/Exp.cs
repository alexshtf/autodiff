namespace AutoDiff
{
    /// <summary>
    /// Represents the exponential function <c>e^x</c>
    /// </summary>
#if DOTNET
    [Serializable]
#endif
    public class Exp : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Exp"/> type.
        /// </summary>
        /// <param name="arg">The exponent of the function.</param>
        public Exp(Term arg)
        {
            Arg = arg;
        }

        /// <summary>
        /// Gets the exponent term.
        /// </summary>
        public Term Arg { get; }

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
