using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    public enum SymbolKind
    {
        STATIC, FIELD, ARG, VAR, NONE
    }

    public enum SubroutineKind
    {
        CONSTRUCTOR,
        FUNCTION,
        METHOD
    }

    internal static class JackDefinitions
    {
        public const string JackAssemblyName = "JackExecutable";
        public static string[] PrimitiveCLRTypes => new string[] { "int", "void", "char", "bool" };
    }
}
