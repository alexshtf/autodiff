using System;

namespace AutoDiff
{
    /// <summary>
    /// Represents a constant-power function x^n, where n is constant.
    /// </summary>
#if (!NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6)
    [Serializable]
#endif
    public class ConstPower : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="ConstPower"/> class.
        /// </summary>
        /// <param name="baseTerm">The base of the power function</param>
        /// <param name="exponent">The exponent of the power function</param>
        public ConstPower(Term baseTerm, double exponent)
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
        public double Exponent { get; }

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
