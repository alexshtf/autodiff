using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    class ExpressionTreeDifferentiatorFactory
    {
        public ExpressionTreeDifferentiator Create(Term term, Variable[] variables)
        {
            return new ExpressionTreeDifferentiator(term, variables, new Compiled.Compiler());
        }
    }
}
