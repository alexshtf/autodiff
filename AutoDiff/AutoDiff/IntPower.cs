using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Represents a constant-power function x^n, where n is constant.
    /// </summary>
    [Serializable]
    public class IntPower : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="IntPower"/> class.
        /// </summary>
        /// <param name="baseTerm">The base of the power function</param>
        /// <param name="exponent">The exponent of the power function</param>
        public IntPower(Term baseTerm, double exponent)
        {
            Base = baseTerm;
            Exponent = exponent;
        }

        /// <summary>
        /// Gets the base term of the power function
        /// </summary>
        public Term Base { get; private set; }

        /// <summary>
        /// Gets the exponent term of the power function.
        /// </summary>
        public double Exponent { get; private set; }

        /// <summary>
        /// Accepts a term visitor.
        /// </summary>
        /// <param name="visitor">The term visitor to accept.</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
