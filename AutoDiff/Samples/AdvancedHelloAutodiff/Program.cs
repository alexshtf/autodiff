using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace AdvancedHelloAutodiff
{
    class Program
    {
        static void Main(string[] args)
        {
            // we will use a function of two variables
            Variable x = new Variable();
            Variable y = new Variable();

            // func(x, y) = (x + y) * exp(x - y)
            var func = (x + y) * TermBuilder.Exp(x - y);

            // create compiled term and use it for many gradient/value evaluations
            var compiledFunc = func.Compile(x, y);

            // we can now efficiently compute the gradient of "func" 64 times with different inputs.
            // compilation helps when we need many evaluations of the same function.
            for (int i = 0; i < 64; ++i)
            {
                var xVal = i / 64.0;
                var yVal = 1 + i / 128.0;

                var diffResult = compiledFunc.Differentiate(xVal, yVal);
                var gradient = diffResult.Item1;
                var value = diffResult.Item2;

                Console.WriteLine("The value of func at x = {0}, y = {1} is {2} and the gradient is ({3}, {4})", 
                    xVal, yVal, value, gradient[0], gradient[1]);
            }
        }
    }
}
