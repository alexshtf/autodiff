using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class Compiler
    {
        public TapeElement[] Compile(AutoDiff.Term term, AutoDiff.Variable[] variables)
        {
            if (term is AutoDiff.Variable)
                term = new AutoDiff.ConstPower(term, 1);

            var tapeList = new List<Compiled.TapeElement>();
            var compilerVisitor = new CompilerVisitor(variables, tapeList);
            compilerVisitor.Compile(term);
            var tape = tapeList.ToArray();
            return tape;
        }
    }
}
