namespace AutoDiff
{
#if DOTNET
    [Serializable]
#endif
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
        public Term Base { get; }

        /// <summary>
        /// Gets the exponent term of the power function.
        /// </summary>
        public Term Exponent { get; }

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
