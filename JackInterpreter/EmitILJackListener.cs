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
        /*
         * This is the heart of the compiler, but has kind of turned into a hot mess.
         * The following refactorings would help bring some more order to this code:
         * 
         * 1 - Move IL generation out into its own set of classes that hide all the string 
         * manipulation nastiness
         * 
         * 2 - Break out duplicated switch statements into methods.
         * 
         * 3 - Break this up into partial classes that handle different concerns (expressions,
         * statements, declarations, subroutine bodies)
         * 
         * 4 - Decide on a sane way to order methods within the class.
         * 
         * 5 - Define all the metadata needed to link to NJackOS in another class so that a loop
         * can be used instead of the long if/elif chain.
         */

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
            var fieldTypeString = JackToCLRTranslation.GetFieldTypeString(fieldType);

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
                string typeString = JackToCLRTranslation.GetFieldTypeString(paramType);
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
                var fieldTypeString = JackToCLRTranslation.GetFieldTypeString(fieldType);

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

        public override void EnterElseBody([NotNull] ElseBodyContext context)
        {
            base.EnterElseBody(context);
        }

        public override void ExitIfStatement([NotNull] IfStatementContext context)
        {
            var lastCookie = intCookieStack.Pop();
            writer.WriteLine($"IF_EXIT_{lastCookie}:");
        }

        public override void EnterWhileStatement([NotNull] WhileStatementContext context)
        {
            var nextCookie = ifCookie++;
            intCookieStack.Push(nextCookie);
            writer.WriteLine($"WHILE_ENTER_{nextCookie}:");
        }

        public override void EnterWhileBody([NotNull] WhileBodyContext context)
        {
            var nextCookie = intCookieStack.Peek();
            writer.WriteLine($"brfalse WHILE_EXIT_{nextCookie}");
        }

        public override void ExitWhileBody([NotNull] WhileBodyContext context)
        {
            var lastCookie = intCookieStack.Pop();
            writer.WriteLine($"br WHILE_ENTER_{lastCookie}");
            writer.WriteLine($"WHILE_EXIT_{lastCookie}:");
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
                        op = "ldloc.s";
                        writer.WriteLine($"{op} {index}");
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
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackString [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_String()");
            }
            else if (lhs.Equals("Keyboard"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackKeyboard [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Keyboard()");
            }
            else if (lhs.Equals("Math"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackMath [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_JMath()");
            }
            else if (lhs.Equals("Memory"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackMemory [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Memory()");
            }
            else if (lhs.Equals("Sys"))
            {
                writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackSys [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_Sys()");
            }
            else
            {
                if (!dataSymbolTable.KindOf(lhs).Equals(SymbolKind.NONE))
                {
                    string op;
                    var index = dataSymbolTable.IndexOf(lhs);
                    switch (dataSymbolTable.KindOf(lhs))
                    {
                        case SymbolKind.ARG:
                            op = "ldarg";
                            writer.WriteLine($"{op}.{index}");
                            break;
                        case SymbolKind.VAR:
                            op = "ldloc.s";
                            writer.WriteLine($"{op} {index}");
                            break;
                        case SymbolKind.FIELD:
                            writer.WriteLine($"ldfld {dataSymbolTable.TypeOf(lhs)} {JackDefinitions.JackAssemblyName}.{className}::{lhs}");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                }
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
                        writer.WriteLine("callvirt instance void [NJackOS.Interface]NJackOS.Interface.IJackOutput::printString(class [NJackOS.Interface]NJackOS.Interface.JackStringClass)");
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
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("String"))
            {
                switch (rhs)
                {
                    case "new":
                        writer.WriteLine("callvirt instance class [NJackOS.Interface]NJackOS.Interface.JackStringClass [NJackOS.Interface]NJackOS.Interface.IJackString::New(int16)");
                        break;
                    case "backSpace":
                        writer.WriteLine("callvirt instance char [NJackOS.Interface]NJackOS.Interface.IJackString::backSpace()");
                        break;
                    case "doubleQuote":
                        writer.WriteLine("callvirt instance char [NJackOS.Interface]NJackOS.Interface.IJackString::doubleQuote()");
                        break;
                    case "newLine":
                        writer.WriteLine("callvirt instance char [NJackOS.Interface]NJackOS.Interface.IJackString::newLine()");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("Keyboard"))
            {
                switch (rhs)
                {
                    case "keyPressed":
                        writer.WriteLine("callvirt instance char [NJackOS.Interface]NJackOS.Interface.IJackKeyboard::keyPressed()");
                        break;
                    case "readChar":
                        writer.WriteLine("callvirt instance char [NJackOS.Interface]NJackOS.Interface.IJackKeyboard::readChar()");
                        break;
                    case "readLine":
                        writer.WriteLine("callvirt instance class [NJackOS.Interface]NJackOS.Interface.JackStringClass [NJackOS.Interface]NJackOS.Interface.IJackKeyboard::readLine(class [NJackOS.Interface]NJackOS.Interface.JackStringClass)");
                        break;
                    case "readInt":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackKeyboard::readInt(class [NJackOS.Interface]NJackOS.Interface.JackStringClass)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("Math"))
            {
                switch (rhs)
                {
                    case "abs":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMath::abs(int16)");
                        break;
                    case "max":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMath::max(int16,int16)");
                        break;
                    case "min":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMath::min(int16,int16)");
                        break;
                    case "sqrt":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMath::sqrt(int16)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("Memory"))
            {
                switch (rhs)
                {
                    case "peek":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMemory::peek(int16)");
                        break;
                    case "poke":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMemory::poke(int16,int16)");
                        break;
                    case "alloc":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMemory::alloc(int16)");
                        break;
                    case "deAlloc":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackMemory::deAlloc(class [NJackOS.Interface]NJackOS.Interface.JackArrayClass)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else if (lhs.Equals("Sys"))
            {
                switch (rhs)
                {
                    case "halt":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackSys::halt()");
                        break;
                    case "error":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackSys::error(int16)");
                        break;
                    case "wait":
                        writer.WriteLine("callvirt instance int16 [NJackOS.Interface]NJackOS.Interface.IJackSys::wait(int16)");
                        break;
                    default:
                        throw new NotImplementedException(rhs);
                }
            }
            else
            {
                if (!dataSymbolTable.KindOf(lhs).Equals(SymbolKind.NONE))
                {
                    var type = dataSymbolTable.TypeOf(lhs);
                    var subroutineEntry = subroutineSymbolTable.Get(type, rhs);
                    writer.WriteLine(subroutineEntry.GenerateInstanceInvocationIL(type));
                }
                else
                {
                    var subroutineEntry = subroutineSymbolTable.Get($"class {JackDefinitions.JackAssemblyName}.{lhs}", rhs);
                    writer.WriteLine(subroutineEntry.GenerateInstanceInvocationIL($"{JackDefinitions.JackAssemblyName}.{lhs}"));
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
            writer.WriteLine("call class [NJackOS.Interface]NJackOS.Interface.IJackString [NJackOS.Interface]NJackOS.Interface.JackOSProvider::get_String()");
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
                    writer.WriteLine($"{op}.{index}");
                    break;
                case SymbolKind.VAR:
                    op = "ldloc.s";
                    writer.WriteLine($"{op} {index}");
                    break;
                case SymbolKind.FIELD:
                    writer.WriteLine($"ldfld {dataSymbolTable.TypeOf(varName)} {JackDefinitions.JackAssemblyName}.{className}::{varName}");
                    return;
                default:
                    throw new NotImplementedException(dataSymbolTable.KindOf(varName).ToString());
            }
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
                writer.WriteLine("ldc.i4.0");
                writer.WriteLine("ceq");
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
                    op = "ldloc.s";
                    writer.WriteLine($"{op} {index}");
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

        private readonly IndentedTextWriter writer;
        private readonly SubroutineSymbolTable subroutineSymbolTable;

        private int ifCookie = 0;
        private Stack<int> intCookieStack = new();
        private Stack<string> subroutineNames = new Stack<string>();
        private Stack<string> staticClassForMethodInvocation = new Stack<string>();

        private readonly record struct TableEntry(string name, string type, int index);
        private DataSymbolTable dataSymbolTable = new DataSymbolTable();
        private string className;
    }
}
