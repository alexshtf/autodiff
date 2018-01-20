using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiff
{
    internal static class ErrorMessages
    {
        public const string ArgLength = "arg length must equal the number of term variables";
        public const string GradLength = "grad length must equal the number of variables";
        public const string ParamLength = "parameters length must equal the number of term parameters";
    }
}
