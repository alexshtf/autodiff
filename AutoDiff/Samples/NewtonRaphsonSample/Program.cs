using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace NewtonRaphsonSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Variable x = new Variable();

            // f(x) = e^(-x) + x - 2
            // it has two roots. See plot.png image attached to this project.    
            var func = TermBuilder.Exp(-x) + x - 2;

            // find the root near 2
            var root1 = RootFinder.NewtonRaphson(func, x, 2);
            
            // find the root near -1
            var root2 = RootFinder.NewtonRaphson(func, x, -1);

            Console.WriteLine("The equation e^(-x) + x - 2 = 0 has two solutions:\nx = {0}\nx = {1}", root1, root2);
        }
    }
}
