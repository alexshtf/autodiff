using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Construct a piece-wise defined function term.
    /// </summary>
    [Serializable]
    public class PiecewiseTerm : Term
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="PiecewiseTerm"/> class
        /// </summary>
        /// <param name="pieces">The definition pieces. Each item is a pair of an inequality and a corresponding term.</param>
        public PiecewiseTerm(IEnumerable<Tuple<Inequality, Term>> pieces)
        {
            Contract.Requires(pieces != null);
            Contract.Requires(pieces.Any());
            Contract.Requires(Contract.ForAll(pieces, piece => piece != null && piece.Item1 != null && piece.Item2 != null));

            Pieces = Array.AsReadOnly(pieces.ToArray());
        }

        /// <summary>
        /// Gets the pieces of the piece-wise defined function.
        /// </summary>
        public ReadOnlyCollection<Tuple<Inequality, Term>> Pieces { get; private set; }

        /// <summary>
        /// Accepts a term visitor.
        /// </summary>
        /// <param name="visitor">The term visitor to accept.</param>
        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
