using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.PerfTest
{
    interface ITest
    {
        string Name { get; }
        void Run();
    }
}
