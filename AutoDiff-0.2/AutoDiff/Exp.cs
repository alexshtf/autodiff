using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Represents the exponential function <c>e^x</c>
    /// </summary>
    [Serializable]
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
        public Term Arg { get; private set; }

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
