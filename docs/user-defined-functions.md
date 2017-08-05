**Short intro**
Sometimes the built-in functions provided by the AutoDiff library are not enough and you wish to use other functions that don't exist in the AutoDiff library. For this purpose the library allows you to define your own custom functions. All you need to do is to provide delegates to compute the function's value and its derivative. The following example will show you how to define {{ arctan }} and {{ atan2 }} functions and create new terms that use them. There are usually two steps:
* Create a terms factory given evaluation / derivative delegates
* Use the factories to create terms and build functions using those terms

**The code**
This time we will let the code speak for itself. It's quite self-explainatory
{code:c#}
class Program
{
    static void Main(string[]() args)
    {
        // create function factory for arctangent
        var arctan = UnaryFunc.Factory(
            x => Math.Atan(x),      // evaluate
            x => 1 / (1 + x * x));  // derivative of atan

        // create function factory for atan2
        var atan2 = BinaryFunc.Factory(
            (x, y) => Math.Atan2(y, x),
            (x, y) => Tuple.Create(
                -y / (x**x + y**y),  // d/dx (from wikipedia)
                x / (x**x + y**y))); // d/dy (from wikipedia)

        
        // define variables
        var u = new Variable();
        var v = new Variable();
        var w = new Variable();

        // create and compile a term
        var term = atan2(u, v) - arctan(w) * atan2(v, w);
        var compiled = term.Compile(u, v, w);

        // compute value + gradient
        var diff = compiled.Differentiate(1, 2, -2);

        Console.WriteLine("The value at (1, 2, -2) is {0}", diff.Item2);
        Console.WriteLine("The gradient at (1, 2, -2) is ({0}, {1}, {2})",
            diff.Item1[0](0), 
            diff.Item1[1](1), 
            diff.Item1[2](2));
    }
}
{code:c#}