using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AutoDiff
{
    /// <summary>
    /// Represents a sum of at least two terms.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Sum: {Terms.Count}")]
    public class Sum : Term
    {
        /// <summary>
        /// Constructs an instance of the <see cref="Sum"/> class.
        /// </summary>
        /// <param name="first">The first term in the sum</param>
        /// <param name="second">The second term in the sum</param>
        /// <param name="rest">The rest of the terms in the sum.</param>
        public Sum(Term first, Term second, params Term[] rest)
        {
            var allTerms = 
                (new Term[] { first, second}).Concat(rest);

            Terms = allTerms.ToList().AsReadOnly();
        }

        internal Sum(IEnumerable<Term> terms)
        {
            Terms = Array.AsReadOnly(terms.ToArray());
        }

        /// <summary>
        /// Gets the terms of this sum.
        /// </summary>
        public ReadOnlyCollection<Term> Terms { get; private set; }
        
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
