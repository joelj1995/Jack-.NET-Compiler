using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal record JackClassCompilationProduct
    {
        public required Type RootClassType { get; init; }
        public required string RootTypeName { get; init; }
    }
}
