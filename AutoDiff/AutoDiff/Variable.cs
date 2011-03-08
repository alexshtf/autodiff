using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    /// <summary>
    /// Represents a variable term. Variable terms are substituted for real values during evaluation and
    /// differentiation. For more info see <see cref="Evaluator"/> and <see cref="Differentiator"/> classes.
    /// </summary>
    [Serializable]
    public class Variable : Term
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
