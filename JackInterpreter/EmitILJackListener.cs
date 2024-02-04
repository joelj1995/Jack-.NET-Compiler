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
            this.writer = new JackILWriter(outputStream);
        }

        public override void EnterClassDeclaration(JackParser.ClassDeclarationContext context)
        {
            writer.WriteILHeader();
        }

        public override void ExitClassDeclaration([NotNull] ClassDeclarationContext context)
        {
            writer.WriteILFooter();
        }

        private readonly JackParser parser;
        private readonly CommonTokenStream tokens;
        private readonly JackILWriter writer;
    }
}
