﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.CodeDom.Compiler;
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
        public EmitILJackListener(IndentedTextWriter outputStream, SubroutineSymbolTable subroutineSymbolTable) : base()
        {
            this.writer = outputStream;
            this.subroutineSymbolTable = subroutineSymbolTable;
        }

        public override void EnterClassDeclaration(JackParser.ClassDeclarationContext context)
        {
            string className = context.className().ID().ToString() ?? 
                throw new NullReferenceException("Class Name Null");
            writer.WriteLine($".class public auto ansi beforefieldinit {JackDefinitions.JackAssemblyName}.{className} extends [mscorlib]System.Object");
            writer.WriteLine("{");
            writer.Indent++;
        }

        public override void ExitClassDeclaration([NotNull] ClassDeclarationContext context)
        {
            writer.Indent--;
            writer.WriteLine("}");
        }

        public override void EnterClassVarDec([NotNull] ClassVarDecContext context)
        {
            var modifier = context.classVarDecModifier().GetText();
            switch (modifier)
            {
                case "field":
                    modifier = "";
                    break;
                case "static":
                    break;
                default:
                    throw new NotImplementedException(modifier);

            }
            var fieldType = context.type();
            var fieldTypeString = "UNKNOWN";
            if (fieldType is TypeIntContext)
            {
                fieldTypeString = "int16";
            }
            else if (fieldType is TypeCharContext)
            {
                fieldTypeString = "char";
            }
            else if (fieldType is TypeBoolContext)
            {
                fieldTypeString = "bool";
            }
            else if (fieldType is TypeClassContext classContext)
            {
                var className = classContext.className().ID().ToString() ?? throw new NullReferenceException();
                fieldTypeString = $"class {JackDefinitions.JackAssemblyName}.{className}";
            }
            else
            {
                throw new NotImplementedException(fieldType.GetText());
            }

            foreach (var field in context.varName())
            {
                writer.WriteLine($".field private {modifier} {fieldTypeString} {field.ID()}");
            }
        }

        public override void EnterSubroutineDec([NotNull] SubroutineDecContext context)
        {
            symbolTable.StartSubroutine();
            string subroutineName = context.subroutineName().ID().ToString() ?? 
                throw new NullReferenceException("Subroutine Name Null");
            string modifier = context.subroutineDecModifier() is FunctionContext ctx ? "static" : "";
            writer.WriteLine($".method {modifier} public void {subroutineName}() cil managed");
            writer.WriteLine("{");
            writer.Indent++;
            if (subroutineName.Equals("main"))
                writer.WriteLine($".entrypoint");
            writer.WriteLine($".maxstack 256");
        }

        public override void ExitSubroutineDec([NotNull] SubroutineDecContext context)
        {
            writer.Indent--;
            writer.WriteLine("}");
        }

        public override void EnterSubroutineBody([NotNull] SubroutineBodyContext context)
        {
        }

        public override void ExitSubroutineBody([NotNull] SubroutineBodyContext context)
        {
            writer.WriteLine("ret");
        }

        public override void EnterIfStatement([NotNull] IfStatementContext context)
        {
            
        }

        public override void EnterIfBody([NotNull] IfBodyContext context)
        {
            var nextCookie = ifCookie++;
            intCookieStack.Push(nextCookie);
            writer.WriteLine($"brfalse IF_ELSE_{nextCookie}");
        }

        public override void ExitIfBody([NotNull] IfBodyContext context)
        { 
            var lastCookie = intCookieStack.Peek();
            writer.WriteLine($"br IF_EXIT_{lastCookie}");
            writer.WriteLine($"IF_ELSE_{lastCookie}:");
        }

        public override void EnterElseBody([NotNull] ElseBodyContext context)
        {
            base.EnterElseBody(context);
        }

        public override void ExitIfStatement([NotNull] IfStatementContext context)
        {
            var lastCookie = intCookieStack.Pop();
            writer.WriteLine($"IF_EXIT_{lastCookie}:");
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
                        writer.WriteLine("call void [mscorlib]System.Console::Write(int32)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
        }

        public override void EnterConstInt([NotNull] ConstIntContext context)
        {
            var value = context.INTCONST().ToString();
            writer.WriteLine($"ldc.i4 {value}");
        }

        public override void ExitBinaryOp([NotNull] BinaryOpContext context)
        {
            var text = context.GetText();
            var opToken = context.op().GetText();
            switch (opToken)
            {
                case "+":
                    writer.WriteLine("add.ovf");
                    break;
                case "-":
                    writer.WriteLine("sub.ovf");
                    break;
                case "*":
                    writer.WriteLine("mul.ovf");
                    break;
                case "/":
                    writer.WriteLine("div.ovf");
                    break;
                case "&":
                    writer.WriteLine("and");
                    break;
                case "|":
                    writer.WriteLine("or");
                    break;
                case "<":
                    writer.WriteLine("clt");
                    break;
                case ">":
                    writer.WriteLine("cgt");
                    break;
                case "=":
                    writer.WriteLine("ceq");
                    break;
                default:
                    throw new NotImplementedException(opToken);
            }
        }

        public override void ExitUnaryOpMinus([NotNull] UnaryOpMinusContext context)
        {
            writer.WriteLine("neg");
        }

        public override void ExitUnaryOpNot([NotNull] UnaryOpNotContext context)
        {
            writer.WriteLine("not");
        }

        private readonly IndentedTextWriter writer;
        private readonly DataSymbolTable symbolTable = new DataSymbolTable();
        private readonly SubroutineSymbolTable subroutineSymbolTable;

        private int ifCookie = 0;
        private Stack<int> intCookieStack = new();
    }
}
