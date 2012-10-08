using CenterSpace.NMath.Core;


namespace AutoDiff.NMath
{
    class CompiledTermDoubleFunctional : DoubleFunctional
    {
        private readonly ICompiledTerm _compiledTerm;

        public CompiledTermDoubleFunctional(ICompiledTerm compiledTerm)
            : base(compiledTerm.Variables.Count)
        {
            _compiledTerm = compiledTerm;
        }

        public override double Evaluate(DoubleVector x)
        {
            return _compiledTerm.Evaluate(x.ToArray());
        }

        public override void Gradient(DoubleVector x, DoubleVector grad)
        {
            var diff = _compiledTerm.Differentiate(x.ToArray());
            for (int i = 0; i < grad.Length; ++i)
                grad[i] = diff.Item1[i];
        }
    }
}
