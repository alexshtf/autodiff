using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    public interface ICompiledTerm
    {
        double Evaluate(params double[] arg);
        Tuple<double[], double> Differentiate(params double[] arg);
    }
}
