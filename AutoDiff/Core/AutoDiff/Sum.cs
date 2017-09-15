using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoDiff
{
    /// <summary>
    /// Represents a sum of at least two terms.
    /// </summary>
#if DOTNET
    [Serializable]
#endif
    [DebuggerDisplay("Sum: {Terms.Count}")]
    public class Sum : Term
    {
        internal Sum(IEnumerable<Term> terms)
        {
            Terms = terms.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the terms of this sum.
        /// </summary>
        public IReadOnlyList<Term> Terms { get; }
        
        /// <summary>
        /// Accepts a term visitor
        /// </summary>
        /// <param name="visitor">The term visitor to accept.</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a term visitor with a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the visitor's function</typeparam>
        /// <param name="visitor">The visitor to accept</param>
        /// <returns>
        /// The result from the visitor's visit function.
        /// </returns>
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
