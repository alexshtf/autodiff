using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoDiff.PerfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest(new ExpQuadraticTest());
        }

        static void RunTest(ITest test)
        {
            Console.Write("Running test: " + test.Name + " ...");
            var stopWatch = Stopwatch.StartNew();
            test.Run();
            stopWatch.Stop();
            Console.WriteLine(" done in " + stopWatch.Elapsed);
        }
    }
}
