using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Represents a natural logarithm function
    /// </summary>
    [Serializable]
    public class Log : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="arg">The argument of the natural logarithm</param>
        public Log(Term arg)
        {
            Arg = arg;
        }

        /// <summary>
        /// Accepts a terms visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Gets the natural logarithm argument.
        /// </summary>
        public Term Arg { get; private set; }
    }
}
