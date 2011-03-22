using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoDiff
{
    /// <summary>
    /// A constant value term
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Constant = {Value}")]
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
        public double Value { get; private set; }

        /// <summary>
        /// Accepts a term visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
