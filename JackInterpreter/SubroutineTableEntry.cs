using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal class SubroutineTableEntry
    {
        public SubroutineKind Kind { get; private set; }
        public string Name { get; private set; }

        public SubroutineTableEntry(SubroutineKind kind, string name)
        {
            this.Kind = kind;
            this.Name = name;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj is SubroutineTableEntry te)
            {
                return te.Name.Equals(this.Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
