using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace NewtonRaphsonSample
{
    static class RootFinder
    {
        /// <summary>
        /// Attempts to solve an equation f(x) = 0, givn f, using the newton-raphson method.
        /// </summary>
        /// <param name="func">The function</param>
        /// <param name="x">The variable used in the function.</param>
        /// <param name="initGuess">An initial guess where to start the iterations.</param>
        /// <param name="maxIterations">The number of iterations to perform</param>
        /// <returns>The approximated solution of f(x) = 0</returns>
        public static double NewtonRaphson(Term func, Variable x, double initGuess, int maxIterations = 10)
        {
            double guess = initGuess;
            var compiledFunc = func.Compile(x);
            for (int i = 0; i < maxIterations; ++i) // perform maxIterations iterations
            {
                // perform differentiation
                var diffResult = compiledFunc.Differentiate(guess);
                
                // extract function value + derivative (the first element of the gradient)
                var dfx = diffResult.Item1[0];
                var fx = diffResult.Item2;

                // newton-raphson iteration: x <- x - f(x) / f'(x)
                guess = guess - fx / dfx;
            }
            return guess;
        }
    }
}
