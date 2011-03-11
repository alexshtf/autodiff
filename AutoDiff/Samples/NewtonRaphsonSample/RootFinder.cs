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
            for (int i = 0; i < maxIterations; ++i) // perform maxIterations iterations
            {
                // evaluate f(x)
                var fx = Evaluator.Evaluate(func, new Variable[] { x }, new double[] { guess });

                // evaluate f'(x)
                var gradient = Differentiator.Differentiate(func, new Variable[] { x }, new double[] { guess });
                var dfx = gradient[0]; // this is a single-variable function. First gradient element is the derivative

                // newton-raphson iteration: x <- x - f(x) / f'(x)
                guess = guess - fx / dfx;
            }
            return guess;
        }
    }
}
