using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    interface ITermVisitor<TResult>
    {
        TResult Visit(Constant constant);
        TResult Visit(Log log);
        TResult Visit(Power power);
        TResult Visit(Product product);
        TResult Visit(Sum sum);
        TResult Visit(Variable variable);
        TResult Visit(Exp exp);
        TResult Visit(Piecewise piecewise);
    }
}
