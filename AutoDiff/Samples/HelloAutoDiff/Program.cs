using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace HelloAutoDiff
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

            // define the ordering of variables, and a point where the function will be evaluated/differentiated
            Variable[] vars = { x, y };
            double[] point = { 1, -2 };
            
            // calculate the value and the gradient at the point (x, y) = (1, -2)
            double eval = func.Evaluate(vars, point);
            double[] gradient = func.Differentiate(vars, point);

            // write the results
            Console.WriteLine("f(1, -2) = " + eval);
            Console.WriteLine("Gradient of f at (1, -2) = ({0}, {1})", gradient[0], gradient[1]);
        }
    }
}
