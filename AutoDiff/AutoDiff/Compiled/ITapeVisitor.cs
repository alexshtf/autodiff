using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    interface ITapeVisitor
    {
        void Visit(Constant elem);
        void Visit(Exp elem);
        void Visit(Log elem);
        void Visit(ConstPower elem);
        void Visit(TermPower elem);
        void Visit(Product elem);
        void Visit(Sum elem);
        void Visit(Variable var);
        void Visit(UnaryFunc elem);
        void Visit(BinaryFunc elem);
        void Visit(NaryFunc elem);
    }
}
