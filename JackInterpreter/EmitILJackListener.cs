﻿using Antlr4.Runtime;
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
        public EmitILJackListener(string fileName, JackParser jackParser, CommonTokenStream tokens, StreamWriter outputStream, SymbolTable symbolTable) : base()
        {
            this.parser = jackParser;
            this.tokens = tokens;
            this.outputStream = outputStream;
            this.symbolTable = symbolTable;
        }

        public override void EnterClassDeclaration(JackParser.ClassDeclarationContext context)
        {
            // TODO: Really this header just be written somewhere else for cases where we're handling multiple Jack source filesf
            string className = context.className().ID().ToString() ?? 
                throw new NullReferenceException("Class Name Null");
            outputStream.WriteLine(".assembly extern mscorlib {}");
            outputStream.WriteLine($".assembly {jackAssemblyName} {{}}");
            outputStream.WriteLine($".class public auto ansi beforefieldinit {jackAssemblyName}.{className} extends [mscorlib]System.Object");
            outputStream.WriteLine("{");
        }

        public override void ExitClassDeclaration([NotNull] ClassDeclarationContext context)
        {
            outputStream.WriteLine("}");
        }

        public override void EnterSubroutineDec([NotNull] SubroutineDecContext context)
        {
            symbolTable.StartSubroutine();
            string subroutineName = context.subroutineName().ID().ToString() ?? 
                throw new NullReferenceException("Subroutine Name Null");
            string modifier = context.subroutineDecModifier() is FunctionContext ctx ? "static" : "";
            outputStream.WriteLine($".method {modifier} public void {subroutineName}() cil managed");
            outputStream.WriteLine("{");
            if (subroutineName.Equals("main"))
                outputStream.WriteLine($".entrypoint");
            outputStream.WriteLine($".maxstack 256");
        }

        public override void ExitSubroutineDec([NotNull] SubroutineDecContext context)
        {
            outputStream.WriteLine("}");
        }

        public override void EnterSubroutineBody([NotNull] SubroutineBodyContext context)
        {
        }

        public override void ExitSubroutineBody([NotNull] SubroutineBodyContext context)
        {
            outputStream.WriteLine("ret");
        }

        public override void EnterIfStatement([NotNull] IfStatementContext context)
        {
            
        }

        public override void EnterIfBody([NotNull] IfBodyContext context)
        {
            var nextCookie = ifCookie++;
            intCookieStack.Push(nextCookie);
            outputStream.WriteLine($"brfalse IF_EXIT_{nextCookie}");
        }

        public override void ExitIfStatement([NotNull] IfStatementContext context)
        {
            var lastCookie = intCookieStack.Pop();
            outputStream.WriteLine($"IF_EXIT_{lastCookie}:");
        }



        public override void EnterThatMethod([NotNull] ThatMethodContext context)
        {

        }

        public override void ExitThatMethod([NotNull] ThatMethodContext context)
        {
            var lhs = context.className()?.ID()?.ToString() ?? 
                context.varName()?.ID()?.ToString() ?? 
                throw new NullReferenceException("lhs");
            var rhs = context.subroutineName()?.ID().ToString() ?? throw new NullReferenceException("rhs");
            if (lhs.Equals("Output"))
            {
                switch (rhs)
                {
                    case "printInt":
                        outputStream.WriteLine("call void [mscorlib]System.Console::Write(int32)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
        }

        public override void EnterConstInt([NotNull] ConstIntContext context)
        {
            var value = context.INTCONST().ToString();
            outputStream.WriteLine($"ldc.i4.{value}");
        }

        public override void ExitBinaryOp([NotNull] BinaryOpContext context)
        {
            var text = context.GetText();
            var opToken = context.op().GetText();
            switch (opToken)
            {
                case "+":
                    outputStream.WriteLine("add.ovf");
                    break;
                case "-":
                    outputStream.WriteLine("sub.ovf");
                    break;
                case "*":
                    outputStream.WriteLine("mul.ovf");
                    break;
                case "/":
                    outputStream.WriteLine("div.ovf");
                    break;
                case "&":
                    outputStream.WriteLine("and");
                    break;
                case "|":
                    outputStream.WriteLine("or");
                    break;
                case "<":
                    outputStream.WriteLine("clt");
                    break;
                case ">":
                    outputStream.WriteLine("cgt");
                    break;
                case "=":
                    outputStream.WriteLine("ceq");
                    break;
                default:
                    throw new NotImplementedException(opToken);
            }
        }

        public override void ExitUnaryOpMinus([NotNull] UnaryOpMinusContext context)
        {
            outputStream.WriteLine("neg");
        }

        public override void ExitUnaryOpNot([NotNull] UnaryOpNotContext context)
        {
            outputStream.WriteLine("not");
        }

        private readonly JackParser parser;
        private readonly CommonTokenStream tokens;
        private readonly StreamWriter outputStream;
        private const string jackAssemblyName = "JackExecutable";
        private readonly SymbolTable symbolTable;

        private int ifCookie = 0;
        private Stack<int> intCookieStack = new();
    }
}
