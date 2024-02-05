using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JackInterpreter.JackDefinitions;

namespace JackInterpreter
{
    internal class SymbolTable
    {
        public void Define(string name, string type, SymbolKind kind)
        {
            if (subroutineScope && kind.Equals(SymbolKind.STATIC)) throw new ArgumentException();
            if (subroutineScope && kind.Equals(SymbolKind.FIELD)) throw new ArgumentException();
            if (!subroutineScope && kind.Equals(SymbolKind.ARG)) throw new ArgumentException();
            if (!subroutineScope && kind.Equals(SymbolKind.VAR)) throw new ArgumentException();
            if (DefaultTable.ContainsKey(name)) throw new InvalidOperationException($"{name} is already defined.");
            DefaultTable[name] = new TableEntry(name, type, kind, idx[(int)kind]++);
        }

        public int IndexOf(string name)
        {
            return Lookup(name).index;
        }

        public SymbolKind KindOf(string name)
        {
            try
            {
                return Lookup(name).kind;
            }
            catch (KeyNotFoundException)
            {
                return SymbolKind.NONE;
            }
        }

        public void StartSubroutine()
        {
            subroutineScope = true;
            subroutineTable = new Dictionary<string, TableEntry>();
            idx[(int)SymbolKind.ARG] = 0;
            idx[(int)SymbolKind.VAR] = 0;
        }

        public string TypeOf(string name)
        {
            return Lookup(name).type;
        }

        public int VarCount(SymbolKind kind)
        {
            return idx[(int)kind];
        }

        private TableEntry Lookup(string name)
        {
            if (subroutineScope)
            {
                if (subroutineTable.ContainsKey(name))
                {
                    return subroutineTable[name];
                }
            }
            return classTable[name];
        }

        private IDictionary<string, TableEntry> DefaultTable => subroutineScope ? subroutineTable : classTable;

        private readonly record struct TableEntry(string name, string type, SymbolKind kind, int index);
        private IDictionary<string, TableEntry> classTable = new Dictionary<string, TableEntry>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private IDictionary<string, TableEntry> subroutineTable = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        private bool subroutineScope = false;
        private readonly int[] idx = new int[(int)SymbolKind.NONE] { 0, 0, 0, 0 };
    }
}
