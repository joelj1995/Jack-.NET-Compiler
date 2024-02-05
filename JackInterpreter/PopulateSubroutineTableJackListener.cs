﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal class PopulateSubroutineTableJackListener : JackBaseListener
    {
        public PopulateSubroutineTableJackListener(SubroutineSymbolTable subroutineSymbolTable)
        {
            this.subroutineSymbolTable = subroutineSymbolTable;
        }

        public override void EnterClassDeclaration([NotNull] JackParser.ClassDeclarationContext context)
        {
            string className = context.className().ID().ToString() ??
                throw new NullReferenceException("Class Name Null");

            this.className = className;
        }

        public override void ExitClassDeclaration([NotNull] JackParser.ClassDeclarationContext context)
        {
            if (subroutineKinds.Count > 0)
                throw new Exception("All subroutines kinds should be consumed before exiting class declaration.");
        }

        public override void EnterConstructor([NotNull] JackParser.ConstructorContext context)
        {
            subroutineKinds.Push(SubroutineKind.CONSTRUCTOR);
        }

        public override void EnterFunction([NotNull] JackParser.FunctionContext context)
        {
            subroutineKinds.Push(SubroutineKind.FUNCTION);
        }

        public override void EnterMethod([NotNull] JackParser.MethodContext context)
        {
            subroutineKinds.Push(SubroutineKind.METHOD);
        }

        public override void ExitSubroutineDec([NotNull] JackParser.SubroutineDecContext context)
        {
            var subroutineKind = subroutineKinds.Pop();
            var subroutineName = context.subroutineName().ID().ToString()!;
            subroutineSymbolTable.Define(className, subroutineName, subroutineKind);
        }

        private readonly SubroutineSymbolTable subroutineSymbolTable;
        private string className;
        private Stack<SubroutineKind> subroutineKinds = new Stack<SubroutineKind>();
    }
}
