using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    interface ITermVisitor
    {
        void Visit(Constant constant);
        void Visit(Log log);
        void Visit(Power power);
        void Visit(Product product);
        void Visit(Sum sum);
        void Visit(Variable variable);
        void Visit(Exp exp);
        void Visit(Piecewise piecewise);
    }
}
