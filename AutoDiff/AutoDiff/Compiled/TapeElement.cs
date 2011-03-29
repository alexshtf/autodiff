using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff.Compiled
{
    class TapeElement
    {
        public double Value;
        public double Derivative;
        public InputConnection[] InputOf;
    }
}
