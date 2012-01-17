using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    class InterpreterDifferentiatorFactory
    {
        public InterpreterDifferentiator Create(Term function, Variable[] variables)
        {
            var compiler = new Compiled.Compiler();
            var result = new InterpreterDifferentiator(function, variables, compiler);
            return result;
        }
    }
}
