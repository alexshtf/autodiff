using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public interface ITermVisitor
    {
        void Visit(Constant constant);

        void Visit(Zero zero);

        void Visit(IntPower intPower);

        void Visit(Product product);

        void Visit(Sum sum);

        void Visit(Variable variable);

        void Visit(Log log);

        void Visit(Exp exp);

        void Visit(PiecewiseTerm piecewiseTerm);
    }
}
