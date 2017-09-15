using System;
using System.Linq;
using AutoDiff;
using System.Diagnostics;
using System.IO;

namespace AutodiffBenchmark
{
    class Program
    {
        private const int TERM_SIZE = 10;
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            using (var stdout = new StreamWriter(Console.OpenStandardOutput()))
            using (var stderr = new StreamWriter(Console.OpenStandardError()))
            {
                stderr.AutoFlush = true;
                RunBenchmark(stdout, stderr);
            }
            
        }
        class BenchmarkResult
        {
            public int NumberOfVars{get; set; }
            public int NumberOfTerms{get; set; }
            public long CompileMilliseconds{get; set; }
            public double MillisecondsPerManualEval{get; set; }
            public double MillisecondsPerGradApprox{get; set; }
            public double MillisecondsPerCompiledEval{get; set; }
            public double MillisecondsPerCompiledDiff{get; set; }
        }

        static void RunBenchmark(TextWriter resultWriter, TextWriter logWriter)
        {
            var fac = new CsvHelper.CsvFactory();
            using (var csvWriter = fac.CreateWriter(resultWriter))
            {
                csvWriter.WriteHeader<BenchmarkResult>();

                int[] sizes = { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
                for (int i = 0; i < sizes.Length; ++i)
                {
                    var row = new BenchmarkResult();

                    var termsCount = sizes[i]; row.NumberOfTerms = termsCount;
                    var varsCount = sizes[i]; row.NumberOfVars = varsCount;

                    logWriter.WriteLine(String.Format("Benchmark for {0} terms and {1} variables", termsCount, varsCount));

                    logWriter.Write("\tConstructing coefficients ...");
                    var coefficients = GenerateCoefficients(termsCount, varsCount);
                    logWriter.WriteLine(String.Format(" done"));

                    // generate variables
                    var vars = new Variable[varsCount];
                    for (int j = 0; j < sizes[i]; ++j)
                        vars[j] = new Variable();


                    logWriter.Write("\tGenerating input data ...");
                    double[][] inputData = new double[1000][];
                    for (int j = 0; j < inputData.Length; ++j)
                        inputData[j] = RandomDoubles(varsCount);
                    logWriter.WriteLine(String.Format(" done"));
					
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);

                    logWriter.Write("\tConstructing compiled term ...");
                    var stopWatch = Stopwatch.StartNew();
                    var compiledTerm = ConstructTerm(coefficients, vars);
                    stopWatch.Stop();
                    row.CompileMilliseconds = stopWatch.ElapsedMilliseconds;
                    logWriter.WriteLine(String.Format(" done in {0} milliseconds", row.CompileMilliseconds));

                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);

                    logWriter.Write("\tBenchmarking manual evaluation ...");
                    stopWatch = Stopwatch.StartNew();
                    double sum = 0;
                    for (int j = 0; j < inputData.Length; ++j)
                        sum += NativeEvaluate(coefficients, inputData[j]);
                    stopWatch.Stop();
                    row.MillisecondsPerManualEval = stopWatch.ElapsedMilliseconds / (double)inputData.Length;
                    logWriter.WriteLine(String.Format(" sum is {0}, speed is {1} msec/op", sum, row.MillisecondsPerManualEval));

                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);

                    logWriter.Write("\tBenchmarking AutoDiff compiled evaluation ...");
                    stopWatch = Stopwatch.StartNew();
                    sum = 0;
                    for (int j = 0; j < inputData.Length; ++j)
                        sum += compiledTerm.Evaluate(inputData[j]);
                    stopWatch.Stop();
                    row.MillisecondsPerCompiledEval = stopWatch.ElapsedMilliseconds / (double)inputData.Length;
                    logWriter.WriteLine(String.Format(" sum is {0}, speed is {1} msec/op", sum, row.MillisecondsPerCompiledEval));

                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);

                    logWriter.Write("\tBenchmarking compiled differentiation ...");
                    stopWatch = Stopwatch.StartNew();
                    sum = 0;
                    for (int j = 0; j < inputData.Length; ++j)
                    {
                        var diffResult = compiledTerm.Differentiate(inputData[j]);
                        sum += diffResult.Item2 + diffResult.Item1.Sum();
                    }
                    row.MillisecondsPerCompiledDiff = stopWatch.ElapsedMilliseconds / (double)inputData.Length;
                    logWriter.WriteLine(String.Format(" sum is {0}, speed is {1} msec/op", sum, row.MillisecondsPerCompiledDiff));

                    csvWriter.WriteRecord(row);
                }
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
