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
        public Type? RootClassType { get; private set; }
        public string? RootTypeName { get; private set; }

        public EmitILJackListener(string fileName, JackParser jackParser, CommonTokenStream tokens, AssemblyBuilder assemblyBuilder) : base()
        {
            this.parser = jackParser;
            this.tokens = tokens;
            this.assemblyBuilder = assemblyBuilder;
            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(fileName); // Might be better to share this across files
        }

        public override void EnterClassDeclaration(JackParser.ClassDeclarationContext context)
        {
            
            var className = context.className().ID().ToString() ?? throw new NullReferenceException("Empty name found for class.");
            this.RootTypeName = className;
            typeBuilder = moduleBuilder.DefineType(className, System.Reflection.TypeAttributes.Public);
        }

        public override void ExitClassDeclaration([NotNull] ClassDeclarationContext context)
        {
            this.RootClassType = typeBuilder.CreateType();
        }

        public override void EnterSubroutineDec([NotNull] JackParser.SubroutineDecContext context)
        {
            var subroutineName = context.subroutineName().ID().ToString() ?? throw new NullReferenceException("Empty name found for subroutine.");
            var paramList = context.parameterList();
            if (context.subroutineDecModifier() is FunctionContext ctx)
            {
                var staticMethod = typeBuilder.DefineMethod(
                    subroutineName, 
                    System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public,
                    null, // TODO: Get return type
                    []); // TODO: Get parameter types

                var subroutineStatements = context.subroutineBody().statements();

                var il = staticMethod.GetILGenerator();
                

                for (int i = 0; i < subroutineStatements.ChildCount; i++)
                {
                    var statement = subroutineStatements.statement(i);
                    if (statement is StatementForDoContext statementForDoContext)
                    {
                        var doStatementContext = statementForDoContext.doStatement();
                        var subroutineCallContext = doStatementContext.subroutineCall();
                        if (subroutineCallContext?.className()?.ID()?.ToString()?.Equals("Output", StringComparison.Ordinal) ?? false)
                        {
                            var innerExpr = subroutineCallContext.expressionList().GetText();
                            il.Emit(OpCodes.Ldstr, innerExpr);
                            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                        }
                    }
                    else if (statement is StatementForReturnContext statementForReturnContext)
                    {
                        il.Emit(OpCodes.Ret);
                    }
                    else
                    {
                        throw new Exception($"Statement type not recognized: {statement.GetText()}");
                    }
                }

                
            }
        }

        private TypeBuilder typeBuilder;

        private readonly JackParser parser;
        private readonly CommonTokenStream tokens;
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;
    }
}
