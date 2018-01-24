using System;

namespace AutoDiff
{
    /// <summary>
    /// Represents the exponential function <c>e^x</c>
    /// </summary>
#if (!NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6)
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
