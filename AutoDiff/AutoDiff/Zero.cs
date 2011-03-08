using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// A constant zero term. Similar to <see cref="Constant"/> but represents only the value 0.
    /// </summary>
    [Serializable]
    public class Zero : Term
    {
        /// <summary>
        /// Accepts a term visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept.</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
