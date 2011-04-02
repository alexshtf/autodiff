using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public static class TermUtils
    {
        public static ICompiledTerm Compile(this Term term, params Variable[] variables)
        {
            return new CompiledDifferentiator(term, variables);
        }

        public static double Evaluate(this Term term, Variable[] variables, double[] point)
        {
            return term.Compile(variables).Evaluate(point);
        }

        public static double[] Differentiate(this Term term, Variable[] variables, double[] point)
        {
            return term.Compile(variables).Differentiate(point).Item1;
        }
    }
}
