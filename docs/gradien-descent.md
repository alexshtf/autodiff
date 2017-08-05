# Intro
Like equation-solving, finding a function's minimum is in many cases impossible to solve analytically, or is computationally infeasible (will take too long to compute). To tackle these problems mathematicians invented a family of minimization methods called _iterative solvers_. The gradient descent method is one of them. The idea is very simple - the opposite direction to the gradient is the steepest descent direction, so in order to find a minimum we should just "follow the gradient".

There is an unsolved problem here - the gradient tells us the direction we should go, but does not tell us _how much_ to go. This parameter is called _step size_ and there are many methods to compute/estimate the optimal step size. In this tutorial we will use a constant step size. Another problem is when should we stop? Clearly stopping when we reach the minimum is impractical as it might take forever. There are many stopping criteria, and in this tutorial we will use the simplest one - the number of iterations.

# The code
We will create a method that given a compiled term, an initial guess, the step size and the number of iterations attempts to find the minimum of the function represented by the compiled term.
```c#
static double[] GradientDescent(ICompiledTerm func, double[] init, double stepSize, int iterations)
{
    // clone the initial argument
    var x = (double[])init.Clone();

    // perform the iterations
    for (int i = 0; i < iterations; ++i)
    {
        // compute the gradient
        var gradient = func.Differentiate(x).Item1;

        // perform a descent step
        for (int j = 0; j < x.Length; ++j)
            x[j] -= stepSize * gradient[j];
    }

    return x;
}
```

Now we can use our small function to find the minimum of a function.
```c#
static void Main(string[] args)
{
    var x = new Variable();
    var y = new Variable();
    var z = new Variable();

    // f(x, y, z) = (x-2)² + (y+4)² + (z-1)²
    // the min should be (x, y, z) = (2, -4, 1)
    var func = TermBuilder.Power(x - 2, 2) + TermBuilder.Power(y + 4, 2) + TermBuilder.Power(z - 1, 2);
    var compiled = func.Compile(x, y, z);

    // perform optimization
    var vec = new double[3];
    vec = GradientDescent(compiled, vec, stepSize: 0.01, iterations: 1000);

    Console.WriteLine("The approx. minimizer is: {0}, {1}, {2}", vec[0], vec[1], vec[2]);
}
```

The output is:

    The approx. minimizer is: 1.99999999663407, -3.99999999326813, 0.999999998317033

Which is pretty close to the real minimizer (2, -4, 1)
