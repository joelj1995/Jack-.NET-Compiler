using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static JackParser;

namespace JackInterpreter
{
    internal class EmitILJackListener : JackBaseListener
    {
        public EmitILJackListener(string fileName, JackParser jackParser, CommonTokenStream tokens, StreamWriter outputStream) : base()
        {
            this.parser = jackParser;
            this.tokens = tokens;
            this.outputStream = outputStream;
        }

        public override void EnterClassDeclaration(JackParser.ClassDeclarationContext context)
        {
            string className = context.className().ID().ToString() ?? throw new NullReferenceException("Class Name Null");
            outputStream.WriteLine(ILSnippets.RuntimeReference);
            outputStream.WriteLine($".assembly {jackAssemblyName} {{}}");
            outputStream.WriteLine($".class public auto ansi beforefieldinit {jackAssemblyName}.{className} extends [System.Runtime]System.Object");
            outputStream.WriteLine("{");
        }

        public override void ExitClassDeclaration([NotNull] ClassDeclarationContext context)
        {
            outputStream.WriteLine("}");
        }

        private readonly JackParser parser;
        private readonly CommonTokenStream tokens;
        private readonly StreamWriter outputStream;
        private const string jackAssemblyName = "JackExecutable";
    }
}
