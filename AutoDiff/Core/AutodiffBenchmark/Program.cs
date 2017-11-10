using System;
using System.Linq;
using AutoDiff;
using System.Diagnostics;
using System.IO;
using static System.Math;

namespace AutodiffBenchmark
{    
    class Program
    {
        private const int TERM_SIZE = 10;
        private static readonly Random random = new Random();

        static void Main()
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
                foreach (int sz in sizes)
                {
                    var row = new BenchmarkResult();

                    var termsCount = sz; row.NumberOfTerms = termsCount;
                    var varsCount = sz; row.NumberOfVars = varsCount;
                    var grad = new double[sz];


                    logWriter.WriteLine("Benchmark for {0} terms and {1} variables", termsCount, varsCount);

                    logWriter.Write("\tConstructing coefficients ...");
                    var coefficients = GenerateCoefficients(termsCount, varsCount);
                    logWriter.WriteLine(" done");

                    // generate variables
                    var vars = new Variable[varsCount];
                    for (var j = 0; j < sz; ++j)
                        vars[j] = new Variable();


                    logWriter.Write("\tGenerating input data ...");
                    var inputData = new double[1000][];
                    for (var j = 0; j < inputData.Length; ++j)
                        inputData[j] = RandomDoubles(varsCount);
                    logWriter.WriteLine(" done");
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);


                    ICompiledTerm compiledTerm = null;
                    row.CompileMilliseconds = MeasureMsec("Constructing compiled term", logWriter, 
                        () => compiledTerm = ConstructTerm(coefficients, vars));

                    row.MillisecondsPerManualEval = MeasureMsecPerOp("manual evaluation", 
                        logWriter, inputData.Length, () => inputData.Sum(array => NativeEvaluate(coefficients, array)));

                    row.MillisecondsPerCompiledEval = MeasureMsecPerOp("AutoDiff compiled evaluation",
                        logWriter, inputData.Length, () => inputData.Sum(array => compiledTerm.Evaluate(array)));

                    row.MillisecondsPerCompiledDiff = MeasureMsecPerOp("compiled differentiation",
                        logWriter, inputData.Length, () =>
                        {
                            var sum = 0.0;
                            foreach (var array in inputData)
                            {
                                var val = compiledTerm.Differentiate(array, grad);
                                sum += val + grad.Sum();
                            }
                            return sum;
                        });

                    csvWriter.WriteRecord(row);
                }
            }
        }

        private static long MeasureMsec(string message, TextWriter logWriter, Action op)
        {
            logWriter.Write('\t');
            logWriter.Write(message);
            logWriter.Write(" ...");

            var stopWatch = Stopwatch.StartNew();
            op();
            var time = stopWatch.ElapsedMilliseconds;
            logWriter.WriteLine(" done in {0} msec", time);
            
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);
            return time;
        }

        private static double MeasureMsecPerOp(string message, TextWriter logWriter, int numOps, Func<double> ops)
        {
            logWriter.Write("\tBenchmarking ");
            logWriter.Write(message);
            logWriter.Write(" ...");

            var stopWatch = Stopwatch.StartNew();
            var result = ops();
            var speed = stopWatch.ElapsedMilliseconds / (double)numOps;
            logWriter.WriteLine(" sum is {0}, speed is {1} msec/op", result, speed);
            
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, true);
            return speed;
        }

        private static ICompiledTerm ConstructTerm(Coefficient[][] coefficients, Variable[] variables)
        {
            var squareTerms =
                from i in Enumerable.Range(0, coefficients.Length)
                let termCoefficients = coefficients[i]
                let sumTerms = from j in Enumerable.Range(0, termCoefficients.Length)
                               select termCoefficients[j].Value * variables[termCoefficients[j].Index]
                let sum = TermBuilder.Sum(sumTerms)
                select TermBuilder.Power(sum, 2);

            var finalTerm = TermBuilder.Sum(squareTerms);
            var compiled = finalTerm.Compile(variables);
            return compiled;
        }

        private static double NativeEvaluate(Coefficient[][] coefficients, double[] values)
        {
            var result = 0.0;
            foreach (var termCoef in coefficients)
            {
                var innerProduct = 0.0;
                for (var j = 0; j < termCoef.Length; ++j)
                    innerProduct += termCoef[j].Value * values[termCoef[j].Index];
                result += innerProduct * innerProduct;
            }
            return result;
        }

        private static Coefficient[][] GenerateCoefficients(int termsCount, int varsCount)
        {
            var result = new Coefficient[termsCount][];
            for (var i = 0; i < result.Length; ++i)
            {
                result[i] = new Coefficient[TERM_SIZE];
                var indices = RandomInts(TERM_SIZE, varsCount);
                var values = RandomDoubles(TERM_SIZE);
                for (var j = 0; j < TERM_SIZE; ++j)
                {
                    result[i][j].Index = indices[j];
                    result[i][j].Value = values[j];
                }
            }

            return result;
        }

        #region random generation helpers
        
        private static int[] RandomInts(int count, int max)
        {
            var result = new int[count];
            for (var i = 0; i < count; ++i)
                result[i] = random.Next(max);
            return result;
        }

        private static double[] RandomDoubles(int count)
        {
            var result = new double[count];
            for (var i = 0; i < count; ++i)
                result[i] = random.NextDouble();
            return result;
        }

        #endregion

        #region Coefficient structure
        
        private struct Coefficient
        {
            public int Index;
            public double Value;
        }

        #endregion
    }
}
