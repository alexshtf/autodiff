using System;

namespace AutoDiff
{
    /// <summary>
    /// Represents a variable term. Variable terms are substituted for real values during evaluation and
    /// differentiation. 
    /// </summary>
#if DOTNET
    [Serializable]
#endif

    public class Variable : Term
    {
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
