using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoDiff.PerfTest
{
    class ExpQuadraticTest : ITest
    {
        private const int NUM_OF_VARS = 50;
        private const int NUM_OF_RUNS = 200;

        private readonly Term term;
        private readonly Variable[] variables;
        private double[][] values;

        public ExpQuadraticTest()
        {
            variables = new Variable[NUM_OF_VARS];
            foreach (var i in Enumerable.Range(0, NUM_OF_VARS))
                variables[i] = new Variable();

            values = new double[NUM_OF_RUNS][];
            foreach (var i in Enumerable.Range(0, NUM_OF_RUNS))
            {
                values[i] = new double[NUM_OF_VARS];
                foreach (var j in Enumerable.Range(0, NUM_OF_VARS))
                    values[i][j] = Math.Sin(i) + Math.Log(1 + j);
            }

            var terms =
                from i in Enumerable.Range(0, NUM_OF_VARS)
                from j in Enumerable.Range(0, NUM_OF_VARS)
                let a = Math.Log(1 + i) + Math.Log(2 + j)
                select a * variables[i] * variables[j];

            term = TermBuilder.Log(TermBuilder.Sum(terms));
        }

        public string Name
        {
            get { return "Exp quadratic"; }
        }

        public void Run()
        {
            var diff = new CompiledDifferentiator(term, variables);
            double sum = 0;
            for (int i = 0; i < NUM_OF_RUNS; ++i)
            {
                var diffResult = diff.Calculate(values[i]);
                var grad = diffResult.Item1;
                sum += grad.Sum();
            }
            Debug.WriteLine(sum);
        }
    }
}
