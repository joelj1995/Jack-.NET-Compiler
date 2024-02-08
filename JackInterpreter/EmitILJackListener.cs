using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Microsoft.VisualBasic.FileIO;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
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
            this.className = className;
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
                dataSymbolTable.Define(field.ID().ToString() ?? throw new NullReferenceException(), fieldTypeString, SymbolKind.FIELD);
            }
        }

        public override void EnterSubroutineDec([NotNull] SubroutineDecContext context)
        {
            dataSymbolTable.StartSubroutine();
            string subroutineName = context.subroutineName().ID().ToString() ?? 
                throw new NullReferenceException("Subroutine Name Null");
            var modifier = context.subroutineDecModifier();
            string modifierString;
            if (modifier is ConstructorContext)
            {
                modifierString = "specialname rtspecialname instance";
                subroutineName = ".ctor";
            }
            else if (modifier is FunctionContext)
            {
                modifierString = "static";
            }
            else if (modifier is MethodContext)
            {
                modifierString = "instance";
            }
            else
            {
                throw new NotImplementedException(modifier.GetText());
            }
            writer.WriteLine($".method public {modifierString} void {subroutineName}(");
            subroutineNames.Push( subroutineName );
        }

        public override void ExitSubroutineDec([NotNull] SubroutineDecContext context)
        {
            
        }

        public override void EnterParameterList([NotNull] ParameterListContext context)
        {
            writer.Indent++;
            var paramaters = context.parameter();
            for (int i = 0; i < paramaters.Length; i++)
            {
                string typeString;
                string paramName = paramaters[i].varName().ID().ToString() ?? throw new NullReferenceException();
                var paramType = paramaters[0].type();
                if (paramType is TypeIntContext)
                {
                    typeString = "int16";
                }
                else if (paramType is TypeCharContext)
                {
                    typeString = "char";
                }
                else if (paramType is TypeBoolContext)
                {
                    typeString = "bool";
                }
                else if (paramType is TypeClassContext classContext)
                {
                    var className = classContext.className().ID().ToString() ?? throw new NullReferenceException();
                    typeString = $"class {JackDefinitions.JackAssemblyName}.{className}";
                }
                else
                {
                    throw new NotImplementedException(paramType.GetText());
                }
                var separator = (i + 1 == paramaters.Length) ? "" : ",";
                writer.WriteLine($"{typeString} {paramName}{separator}");
                dataSymbolTable.Define(paramName, typeString, SymbolKind.ARG);
            }
            
            writer.Indent--;
        }

        public override void EnterSubroutineBody([NotNull] SubroutineBodyContext context)
        {
            writer.WriteLine(") cil managed");
            writer.WriteLine("{");
            writer.Indent++;
            if (subroutineNames.Peek().Equals("main"))
                writer.WriteLine($".entrypoint");
            writer.WriteLine($".maxstack 256");
            writer.WriteLine(".locals init(");
            writer.Indent++;
            int p = 0;
            int j = 0;
            foreach (var varDecContext in context.varDec())
            {
                var fieldType = varDecContext.type();
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

                var varNames = varDecContext.varName();
                for (int i = 0; i < varNames.Length; i++)
                {
                    var varNameString = varNames[i].GetText();
                    var separator = (i + 1 == varNames.Length && j + 1 == context.varDec().Length) ? "" : ",";
                    writer.WriteLine($"[{p}] {fieldTypeString} {varNameString}{separator}");
                    dataSymbolTable.Define(varNameString, fieldTypeString, SymbolKind.VAR);
                    
                    p++;
                }
                j++;
            }
            writer.Indent--;
            writer.WriteLine(")");
        }

        public override void ExitSubroutineBody([NotNull] SubroutineBodyContext context)
        {
            writer.WriteLine("ret");
            writer.Indent--;
            writer.WriteLine("}");
            subroutineNames.Pop();
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

        public override void ExitLetStatement([NotNull] LetStatementContext context)
        {
            var varName = context.varName().ID().ToString() ?? throw new NullReferenceException();
            var index = dataSymbolTable.IndexOf(varName);
            string op;
            switch (dataSymbolTable.KindOf(varName))
            {
                case SymbolKind.ARG:
                    writer.WriteLine($"starg.s {varName}");
                    return;
                case SymbolKind.VAR:
                    writer.WriteLine($"stloc {index}");
                    return;
                case SymbolKind.FIELD:
                    writer.WriteLine($"stfld {dataSymbolTable.TypeOf(varName)} {JackDefinitions.JackAssemblyName}.{className}::{varName}");
                    return;
                default:
                    throw new NotImplementedException(dataSymbolTable.KindOf(varName).ToString());
            }
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

        public override void EnterTermVarName([NotNull] TermVarNameContext context)
        {
            var varName = context.varName().ID().ToString() ?? throw new NullReferenceException();
            var index = dataSymbolTable.IndexOf(varName);
            string op;
            switch (dataSymbolTable.KindOf(varName))
            {
                case SymbolKind.ARG:
                    op = "ldarg";
                    break;
                case SymbolKind.VAR:
                    op = "ldloc";
                    break;
                case SymbolKind.FIELD:
                    writer.WriteLine($"ldfld {dataSymbolTable.TypeOf(varName)} {JackDefinitions.JackAssemblyName}.{className}::{varName}");
                    return;
                default:
                    throw new NotImplementedException(dataSymbolTable.KindOf(varName).ToString());
            }
            writer.WriteLine($"{op}.{index}");
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
        private readonly SubroutineSymbolTable subroutineSymbolTable;

        private int ifCookie = 0;
        private Stack<int> intCookieStack = new();
        private Stack<string> subroutineNames = new Stack<string>();

        private readonly record struct TableEntry(string name, string type, int index);
        private DataSymbolTable dataSymbolTable = new DataSymbolTable();
        private string className;
    }
}
