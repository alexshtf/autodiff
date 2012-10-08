using CenterSpace.NMath.Core;

namespace AutoDiff.NMath
{
    public static class NMathTermExtensions
    {
        public static DoubleFunctional AsDoubleFunctional(this Term term, params Variable[] variables)
        {
            var compiledTerm = term.Compile(variables);
            var result = new CompiledTermDoubleFunctional(compiledTerm);
            return result;
        }
    }
}
