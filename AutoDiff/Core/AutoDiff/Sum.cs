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
        
        /// <inheritdoc />
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override TResult Accept<TResult>(ITermVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
