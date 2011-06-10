using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace UserDefinedFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            // create function factory for arctangent
            var arctan = UnaryFunc.Factory(
                x => Math.Atan(x),      // evaluate
                x => 1 / (1 + x * x));  // derivative of atan

            // create function factory for atan2
            var atan2 = BinaryFunc.Factory(
                (x, y) => Math.Atan2(y, x),
                (x, y) => Tuple.Create(
                    -y / (x*x + y*y),  // d/dx (from wikipedia)
                    x / (x*x + y*y))); // d/dy (from wikipedia)

            
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
                diff.Item1[0], 
                diff.Item1[1], 
                diff.Item1[2]);
        }
    }
}
