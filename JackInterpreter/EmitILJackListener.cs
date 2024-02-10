using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Microsoft.VisualBasic.FileIO;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var fieldTypeString = GetFieldTypeString(fieldType);

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
                
                string paramName = paramaters[i].varName().ID().ToString() ?? throw new NullReferenceException();
                var paramType = paramaters[0].type();
                string typeString = GetFieldTypeString(paramType);
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
                var fieldTypeString = GetFieldTypeString(fieldType);

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

        public override void ExitLetStatementArrayIndex([NotNull] LetStatementArrayIndexContext context)
        {

        }

        public override void EnterLetStatement([NotNull] LetStatementContext context)
        {
            writer.WriteLine("// " + context.GetText());
            var varName = context.varName().ID().ToString() ?? throw new NullReferenceException();
            var index = dataSymbolTable.IndexOf(varName);
            if (context.letStatementArrayIndex() is not null)
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackArray [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Array()");
                string op;
                switch (dataSymbolTable.KindOf(varName))
                {
                    case SymbolKind.ARG:
                        op = "ldarg";
                        writer.WriteLine($"{op}.{index}");
                        break;
                    case SymbolKind.VAR:
                        op = "ldloc";
                        writer.WriteLine($"{op}.{index}");
                        break;
                    case SymbolKind.FIELD:
                        writer.WriteLine($"ldfld {dataSymbolTable.TypeOf(varName)} {JackDefinitions.JackAssemblyName}.{className}::{varName}");
                        break;
                    default:
                        throw new NotImplementedException();
                }
                writer.WriteLine("callvirt instance class [NJackOS.Interface]NJackOS.Interface.JackArrayClass [NJackOS.Interface]NJackOS.Interface.IJackArray::FromCLRShort(int16)");
                
            }
        }

        public override void ExitLetStatement([NotNull] LetStatementContext context)
        {
            var varName = context.varName().ID().ToString() ?? throw new NullReferenceException();
            var index = dataSymbolTable.IndexOf(varName);

            var arrayIndexContext = context.letStatementArrayIndex();
            if (arrayIndexContext is not null)
            {
                writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.JackObject::set_Item(int16, int16)");
                return;
            }

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
            writer.WriteLine("// " + context.GetText());
            var lhs = context.className()?.ID()?.ToString() ??
                context.varName()?.ID()?.ToString() ??
                throw new NullReferenceException("lhs");
            if (lhs.Equals("Output"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackOutput [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Output()");
            }
            else if (lhs.Equals("Screen"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackScreen [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Screen()");
            }
            else if (lhs.Equals("Array"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackArray [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Array()");
            }
            else if (lhs.Equals("String"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackArray [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_String()");
            }
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
                    case "moveCursor":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::moveCursor(int16, int16)");
                        break;
                    case "printChar":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::printChar(char c)");
                        break;
                    case "printString":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::printString(JackStringClass)");
                        break;
                    case "printInt":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::printInt(int16)");
                        break;
                    case "println":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::println()");
                        break;
                    case "backSpace":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::backSpace()");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("Screen"))
            {
                switch (rhs)
                {
                    case "clearScreen":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackScreen::clearScreen()");
                        break;
                    case "drawLine":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackScreen::drawLine(int16 x1, int16 x2, int16 y1, int16 y2)");
                        break;
                    case "drawCircle":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackScreen::drawCircle(int16 cx, int16 cy, int16 r)");
                        break;
                    case "drawPixel":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackScreen::drawPixel(int16 x, int16 y)");
                        break;
                    case "drawRectangle":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackScreen::drawRectangle(int16 x1, int16 x2, int16 y1, int16 y2)");
                        break;
                    case "setColor":
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackScreen::setColor(bool b)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("Array"))
            {
                switch (rhs)
                {
                    case "new":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackArray::New(int16)");
                        break;
                }
            }
            else if (lhs.Equals("String"))
            {
                switch (rhs)
                {
                    case "new":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackString::New(int16)");
                        break;
                }
            }
        }

        public override void EnterConstFalse([NotNull] ConstFalseContext context)
        {
            writer.WriteLine("ldc.i4 0");
        }

        public override void EnterConstTrue([NotNull] ConstTrueContext context)
        {
            writer.WriteLine("ldc.i4 1");
        }

        public override void EnterConstInt([NotNull] ConstIntContext context)
        {
            var value = context.INTCONST().ToString();
            writer.WriteLine($"ldc.i4 {value}");
        }

        public override void EnterConstString([NotNull] ConstStringContext context)
        {
            writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackArray [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_String()");
            var value = context.STRINGCONST().ToString();
            writer.WriteLine($"ldstr {value}");
            writer.WriteLine("callvirt instance class [NJackOS.Interface]NJackOS.Interface.JackStringClass [NJackOS.Interface]NJackOS.Interface.IJackString::FromCLRString(string)");
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

        public override void ExitTermUnary([NotNull] TermUnaryContext context)
        {
            var unaryOp = context.unaryOp();
            if (unaryOp is UnaryOpMinusContext)
            {
                writer.WriteLine("neg");
            }
            else if (unaryOp is UnaryOpNotContext)
            {
                writer.WriteLine("not");
            }
            else
            {
                throw new NotImplementedException(unaryOp.GetText());
            }
        }

        public override void EnterTermArrayIndex([NotNull] TermArrayIndexContext context)
        {
            var varName = context.varName().ID().ToString() ?? throw new NullReferenceException();
            var index = dataSymbolTable.IndexOf(varName);
            writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackArray [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Array()");
            string op;
            switch (dataSymbolTable.KindOf(varName))
            {
                case SymbolKind.ARG:
                    op = "ldarg";
                    writer.WriteLine($"{op}.{index}");
                    break;
                case SymbolKind.VAR:
                    op = "ldloc";
                    writer.WriteLine($"{op}.{index}");
                    break;
                case SymbolKind.FIELD:
                    writer.WriteLine($"ldfld {dataSymbolTable.TypeOf(varName)} {JackDefinitions.JackAssemblyName}.{className}::{varName}");
                    break;
                default:
                    throw new NotImplementedException();
            }
            writer.WriteLine("callvirt instance class [NJackOS.Interface]NJackOS.Interface.JackArrayClass [NJackOS.Interface]NJackOS.Interface.IJackArray::FromCLRShort(int16)");
        }

        public override void ExitTermArrayIndex([NotNull] TermArrayIndexContext context)
        {
            writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.JackObject::get_Item(int16)");
        }

        private string GetFieldTypeString(TypeContext fieldType)
        {
            string fieldTypeString;
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
                if (className.Equals("Array"))
                {
                    return "int16";
                }
                fieldTypeString = $"class {JackDefinitions.JackAssemblyName}.{className}";
            }
            else
            {
                throw new NotImplementedException(fieldType.GetText());
            }
            return fieldTypeString;
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
