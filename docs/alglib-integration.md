The following code sample demonstrates integration with the [AlgLib](http://www.alglib.net) library.

{code:c#}
class Program
{
    static void Main(string[]() args)
    {
        var x = new Variable();
        var y = new Variable();

        // f(x, y) = log(exp(x²-2y²-xy+17) + exp(x²-3x+y))
        var func = TermBuilder.Log(
            TermBuilder.Exp(x * x + 2 * y * y - x + y + 17) +
            TermBuilder.Exp(x * x + y * y + 3 * x + y));

        double[]() minimum = Minimize(func.Compile(x, y));

        Console.WriteLine("The minimum is at ({0}, {1})", minimum[0](0), minimum[1](1));
    }

    private static double[]() Minimize(ICompiledTerm func)
    {
        // we will use an array of zeros as our initial guess.
        var x = new double[func.Variables.Count](func.Variables.Count);
            
        // we will optimize using AlgLib's conjuate gradient algorithm
        alglib.mincgstate state;
        alglib.mincgcreate(x, out state);
        alglib.mincgoptimize(state, (double[]()() arg, ref double val, double[]()() grad, object obj) =>
            {
                // perform differentiation with AutoDiff
                var diff = func.Differentiate(arg);

                // copy the results to AlgLib
                val = diff.Item2;
                Array.Copy(diff.Item1, grad, grad.Length);
            }, null, null);
            
        // extract the resulting minimum point
        alglib.mincgreport report;
        alglib.mincgresults(state, out x, out report);

        // return minimum
        return x;
    }
}
{code:c#}