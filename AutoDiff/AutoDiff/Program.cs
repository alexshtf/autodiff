using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    class Program
    {
        public static void Main(string[] args)
        {
            // define variables
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();

            // define our function
            var func = (x + y) * TermBuilder.Exp(z + x * y);

            // prepare arrays needed for evaluation/differentiation
            Variable[] vars = { x, y, z };
            double[] values = {1, 2, -3 };

            // evaluate func at (1, 2, -3)
            double value = Evaluator.Evaluate(func, vars, values); 

            // calculate the gradient at (1, 2, -3)
            double[] gradient = Differentiator.Differentiate(func, vars, values);
        }
    }
}
