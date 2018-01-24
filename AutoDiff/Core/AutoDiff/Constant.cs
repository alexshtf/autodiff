using System;
using System.Diagnostics;

namespace AutoDiff
{
    /// <summary>
    /// A constant value term
    /// </summary>
#if (!NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6)
    [Serializable]
#endif
    [DebuggerDisplay("Constant = {" + nameof(Value) + "}")]
    public class Constant : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Constant"/> class
        /// </summary>
        /// <param name="value">The value of the constant</param>
        public Constant(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of this constant
        /// </summary>
        public double Value { get; }

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
