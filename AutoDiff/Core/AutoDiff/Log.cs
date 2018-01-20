namespace AutoDiff
{
    /// <summary>
    /// Represents a natural logarithm function
    /// </summary>
#if DOTNET
    [Serializable]
#endif
    public class Log : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="arg">The argument of the natural logarithm</param>
        public Log(Term arg)
        {
            Guard.NotNull(arg, nameof(arg));
            Arg = arg;
        }

        /// <summary>
        /// Gets the natural logarithm argument.
        /// </summary>
        public Term Arg { get; }

        /// <inheritdoc />
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            Guard.NotNull(visitor, nameof(visitor));
            return visitor.Visit(this);
        }
        
        /// <inheritdoc />
        public override void Accept(ITermVisitor visitor)
        {
            Guard.NotNull(visitor, nameof(visitor));
            visitor.Visit(this);
        }
    }
}
