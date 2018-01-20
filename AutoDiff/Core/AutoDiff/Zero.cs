using System;

namespace AutoDiff
{
    /// <summary>
    /// A constant zero term. Similar to <see cref="Constant"/> but represents only the value 0.
    /// </summary>
#if DOTNET
    [Serializable]
#endif
    public class Zero : Term
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
