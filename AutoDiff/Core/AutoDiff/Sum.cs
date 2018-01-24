using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoDiff
{
    /// <summary>
    /// Represents a sum of at least two terms.
    /// </summary>
#if (!NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6)
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
