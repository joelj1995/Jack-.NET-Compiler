using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal class SubroutineTableEntry
    {
        public SubroutineKind Kind { get; private set; }
        public string Name { get; private set; }
        public ImmutableArray<string> ArgumentTypes { get; private set; }
        public string ReturnType { get; private set; }
        public bool IsVirtual { get; private set; }

        public SubroutineTableEntry(SubroutineKind kind, string name, string[] args, bool isVirtual, string returnType = "void")
        {
            this.Kind = kind;
            this.Name = name;
            this.ArgumentTypes = args.ToImmutableArray();
            this.IsVirtual = isVirtual;
            ReturnType = returnType;
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

        public string GenerateInstanceInvocationIL(string parentClass)
        {
            var callOp = "call";
            if (IsVirtual)
            {
                callOp = "callvirt";
            }
            var name = Name;
            var returnType = ReturnType;
            if (Name.Equals("new"))
            {
                callOp = "newobj";
                name = ".ctor";
                returnType = "void";
            }
            return $"{callOp} instance {returnType} {parentClass}::{name}({String.Join(',', ArgumentTypes)})";
        }
    }
}
