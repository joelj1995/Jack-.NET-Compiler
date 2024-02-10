using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal class SubroutineSymbolTable
    {
        public IReadOnlyDictionary<string, ISet<SubroutineTableEntry>> Symbols => classSubroutines.ToFrozenDictionary();

        public void Define(string className, string subroutineName, SubroutineKind subroutineKind, string[] args, bool isVirtual, string returnType)
        {
            if (!classSubroutines.ContainsKey(className))
            {
                classSubroutines[className] = new HashSet<SubroutineTableEntry>();
            }
            classSubroutines[className].Add(new SubroutineTableEntry(subroutineKind, subroutineName, args, isVirtual, returnType));
        }

        public SubroutineTableEntry Get(string className, string subroutineName)
        {
            return classSubroutines[className].Single(r => r.Name.Equals(subroutineName));
        }

        private IDictionary<string, ISet<SubroutineTableEntry>> classSubroutines = new Dictionary<string, ISet<SubroutineTableEntry>>();
    }
}
