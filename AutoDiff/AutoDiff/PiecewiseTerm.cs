using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AutoDiff
{
    [Serializable]
    public class PiecewiseTerm : Term
    {
        public PiecewiseTerm(IEnumerable<Tuple<Inequality, Term>> pieces)
        {
            Pieces = Array.AsReadOnly(pieces.ToArray());
        }

        public ReadOnlyCollection<Tuple<Inequality, Term>> Pieces { get; private set; }

        public override void Accept(ITermVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
