using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal class EmitILJackListener : JackBaseListener
    {
        public Type? RootClassType { get; private set; }
        public string? RootTypeName { get; private set; }

        public EmitILJackListener(string fileName, JackParser jackParser, AssemblyBuilder assemblyBuilder) : base()
        {
            this.parser = jackParser;
            this.assemblyBuilder = assemblyBuilder;
            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(fileName); // Might be better to share this across files
        }

        public override void EnterClassName(JackParser.ClassNameContext context)
        {
            var className = context.ID().ToString() ?? throw new NullReferenceException("Empty name found for class");
            this.RootTypeName = className;
            var typeBuilder = moduleBuilder.DefineType(className, System.Reflection.TypeAttributes.Public);
        }

        public override void EnterSubroutineDec([NotNull] JackParser.SubroutineDecContext context)
        {
        }


        private readonly JackParser parser;
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;
    }
}
