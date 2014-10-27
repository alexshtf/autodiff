using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;
using System.Diagnostics;
using System.Threading;

namespace AutodiffBenchmark
{
    class Program
    {
        private const int TERM_SIZE = 10;
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            Console.SetWindowSize(120, 30);

            int[] sizes = { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
            for (int i = 0; i < sizes.Length; ++i)
            {
                var termsCount = sizes[i];
                var varsCount = sizes[i];

                Console.WriteLine("Benchmark for {0} terms and {1} variables", termsCount, varsCount);

                Console.Write("\tConstructing coefficients ...");
                var coefficients = GenerateCoefficients(termsCount, varsCount);
                Console.WriteLine(" done");

                // generate variables
                var vars = new Variable[varsCount];
                for(int j = 0; j < sizes[i]; ++j)
                    vars[j] = new Variable();


                Console.Write("\tGenerating input data ...");
                double[][] inputData = new double[1000][];
                for (int j = 0; j < inputData.Length; ++j)
                    inputData[j] = RandomDoubles(varsCount);
                Console.WriteLine(" done");

                GC.Collect();
                Thread.Sleep(4000);

                Console.Write("\tConstructing compiled term ...");
                var stopWatch = Stopwatch.StartNew();
                var compiledTerm = ConstructTerm(coefficients, vars);
                stopWatch.Stop();
                Console.WriteLine(" done in {0} milliseconds", stopWatch.ElapsedMilliseconds);

                GC.Collect();
                Thread.Sleep(4000);

                Console.Write("\tBenchmarking manual evaluation ...");
                stopWatch = Stopwatch.StartNew();
                double sum = 0;
                for (int j = 0; j < inputData.Length; ++j)
                    sum += NativeEvaluate(coefficients, inputData[j]);
                stopWatch.Stop();
                Console.WriteLine(" sum is {0}, speed is {1} msec/op", sum, stopWatch.ElapsedMilliseconds / (double)inputData.Length);

                GC.Collect();
                Thread.Sleep(4000);

                Console.Write("\tBenchmarking gradient approximation ...");
                stopWatch = Stopwatch.StartNew();
                sum = 0;
                for (int j = 0; j < inputData.Length / 10; ++j)
                {
                    var gradient = ApproxGradient(coefficients, inputData[j]);
                    sum += gradient.Sum();
                }
                stopWatch.Stop();
                Console.WriteLine(" sum is {0}, speed is {1} msec/op", sum, 10 * stopWatch.ElapsedMilliseconds / (double)inputData.Length);

                GC.Collect();
                Thread.Sleep(4000);

                Console.Write("\tBenchmarking AutoDiff compiled evaluation ...");
                stopWatch = Stopwatch.StartNew();
                sum = 0;
                for (int j = 0; j < inputData.Length; ++j)
                    sum += compiledTerm.Evaluate(inputData[j]);
                stopWatch.Stop();
                Console.WriteLine(" sum is {0}, speed is {1} msec/op", sum, stopWatch.ElapsedMilliseconds / (double)inputData.Length);

                GC.Collect();
                Thread.Sleep(4000);

                Console.Write("\tBenchmarking compiled differentiation ...");
                stopWatch = Stopwatch.StartNew();
                sum = 0; 
                for(int j = 0; j < inputData.Length; ++j)
                {
                    var diffResult = compiledTerm.Differentiate(inputData[j]);
                    sum += diffResult.Item2 + diffResult.Item1.Sum();
                }
                Console.WriteLine(" sum is {0}, speed is {1} msec/op", sum, stopWatch.ElapsedMilliseconds / (double)inputData.Length);

            }
        }

        private static ICompiledTerm ConstructTerm(Coefficient[][] coefficients, Variable[] variables)
        {
            var squareTerms =
                from i in Enumerable.Range(0, coefficients.Length)
                let termCoefficients = coefficients[i]
                let sumTerms = from j in Enumerable.Range(0, termCoefficients.Length)
                               select termCoefficients[j].value * variables[termCoefficients[j].index]
                let sum = TermBuilder.Sum(sumTerms)
                select TermBuilder.Power(sum, 2);

            var finalTerm = TermBuilder.Sum(squareTerms);
            var compiled = finalTerm.Compile(variables);
            return compiled;
        }

        private static double[] ApproxGradient(Coefficient[][] coefficients, double[] values, double epsilon = 1E-5)
        {
            var baseValue = NativeEvaluate(coefficients, values);
            var result = new double[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                // save backup of values[i]. we will change it
                var backup = values[i];
                
                // compute the shifte value and restore backup
                values[i] = values[i] + epsilon;
                var shiftedValue = NativeEvaluate(coefficients, values);
                values[i] = backup;

                // compute gradient at position i
                result[i] = (shiftedValue - baseValue) / epsilon;
            }
            return result;
        }

        private static double NativeEvaluate(Coefficient[][] coefficients, double[] values)
        {
            double result = 0.0;
            for (int i = 0; i < coefficients.Length; ++i)
            {
                var term = coefficients[i];
                var sum = 0.0;
                for (int j = 0; j < term.Length; ++j)
                    sum += term[j].value * values[term[j].index];
                result += (sum * sum);
            }
            return result;
        }

        private static Coefficient[][] GenerateCoefficients(int termsCount, int varsCount)
        {
            var result = new Coefficient[termsCount][];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new Coefficient[TERM_SIZE];
                var indices = RandomInts(TERM_SIZE, varsCount);
                var values = RandomDoubles(TERM_SIZE);
                for (int j = 0; j < TERM_SIZE; ++j)
                {
                    result[i][j].index = indices[j];
                    result[i][j].value = values[j];
                }
            }

            return result;
        }

        #region random generation helpers
        
        private static int[] RandomInts(int count, int max)
        {
            var result = new int[count];
            for (int i = 0; i < count; ++i)
                result[i] = random.Next(max);
            return result;
        }

        private static double[] RandomDoubles(int count)
        {
            var result = new double[count];
            for (int i = 0; i < count; ++i)
                result[i] = random.NextDouble();
            return result;
        }

        #endregion

        #region Coefficient structure
        
        private struct Coefficient
        {
            public int index;
            public double value;
        }

        #endregion
    }
}
