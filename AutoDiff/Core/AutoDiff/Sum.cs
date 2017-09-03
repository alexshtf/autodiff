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
        /// <summary>
        /// Constructs an instance of the <see cref="Sum"/> class.
        /// </summary>
        /// <param name="first">The first term in the sum</param>
        /// <param name="second">The second term in the sum</param>
        /// <param name="rest">The rest of the terms in the sum.</param>
        public Sum(Term first, Term second, params Term[] rest)
        {
            var allTerms = 
                new[] { first, second}.Concat(rest);

            Terms = allTerms.ToList().AsReadOnly();
        }

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
