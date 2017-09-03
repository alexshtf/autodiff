using System;
using AutoDiff;

namespace GradientDescentSample
{
    class Program
    {
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
    }
}
