Here is a code sample to demonstrate integration with ExtremeOptimization (documentation [here](http://www.extremeoptimization.com/Documentation.aspx)).

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
        var compiledFunc = func.Compile(x, y);

        var minimum = Minimize(compiledFunc);
        Console.WriteLine(minimum);
    }

    private static DenseVector Minimize(ICompiledTerm compiledFunc)
    {
        var optimizer = new QuasiNewtonOptimizer(QuasiNewtonMethod.Bfgs);
        optimizer.ObjectiveFunction =
            vec => compiledFunc.Evaluate(vec.ToArray());
        optimizer.GradientFunction =
            vec =>
            {
                var diffResult = compiledFunc.Differentiate(vec.ToArray());
                return Vector.Create(diffResult.Item1);
            };
        optimizer.ExtremumType = ExtremumType.Minimum;
        optimizer.Dimensions = 2;
        optimizer.InitialGuess = DenseVector.CreateConstant(2, 0);

        var minimum = optimizer.FindExtremum();
        return minimum;
    }
}
{code:c#}