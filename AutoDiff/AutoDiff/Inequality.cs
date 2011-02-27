using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Represents an inequality of the form f(x) <= 0;
    /// </summary>
    [Serializable]    
    public class Inequality
    {
        public Inequality(Term term)
        {
            Contract.Requires(term != null);
            Term = term;
        }

        public Term Term { get; private set; }

        /// <summary>
        /// Constructs two inequalitites that together represent a predicate f(x) in [a, b]
        /// </summary>
        /// <param name="term">The term</param>
        /// <param name="a">Lower interval bound</param>
        /// <param name="b">Upper interval bound</param>
        /// <returns>The two inequalities as described in the summary.</returns>
        public static IEnumerable<Inequality> Interval(Term term, double a, double b)
        {
            Contract.Requires(term != null);
            Contract.Requires(b >= a);
            yield return GreaterThanEquals(term, a); // a <= term
            yield return LessThanEquals(term, b);    // term <= b
        }

        /// <summary>
        /// Constructs an inequality of the form left &lt;= right.
        /// </summary>
        /// <param name="left">The left part</param>
        /// <param name="right">The right part</param>
        /// <returns>The constructed inequality.</returns>
        public static Inequality GreaterThanEquals(Term left, Term right)
        {
            Contract.Requires(left != null);
            Contract.Requires(right != null);
            return new Inequality(right - left); // left >= right means right - left <= 0
        }
        /// <summary>
        /// Constructs an inequality of the form left &gt;= right.
        /// </summary>
        /// <param name="left">The left part</param>
        /// <param name="right">The right part</param>
        /// <returns>The constructed inequality.</returns>
        public static Inequality LessThanEquals(Term left, Term right)
        {
            Contract.Requires(left != null);
            Contract.Requires(right != null);
            return new Inequality(left - right); // left <= right means left - right <= 0
        }

        /// <summary>
        /// Returns an instance of an inequality that is always true.
        /// </summary>
        public static readonly Inequality AlwaysTrue = new Inequality(new Zero());
    }
}
