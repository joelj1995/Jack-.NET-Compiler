using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal static class JackDefinitions
    {
        internal enum SymbolKind
        {
            STATIC, FIELD, ARG, VAR, NONE
        }

        public const string JackAssemblyName = "JackExecutable";
    }
}
