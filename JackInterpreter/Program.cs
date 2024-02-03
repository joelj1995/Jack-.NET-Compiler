using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Pattern;
using JackInterpreter;
using System.Reflection.Emit;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputFile = @"C:\Users\colte\Downloads\project_11\Seven\Main.jack";

        var inputStream = File.OpenRead(inputFile);

        AntlrInputStream antlrInputStream = new AntlrInputStream(inputStream);
        JackLexer lexer = new JackLexer(antlrInputStream);
        CommonTokenStream tokens = new CommonTokenStream(lexer);

        JackParser parser = new JackParser(tokens);
        IParseTree tree = parser.classDeclaration();
        ParseTreeWalker walker = new();


        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new System.Reflection.AssemblyName("Jack"), AssemblyBuilderAccess.Run);

        EmitILJackListener listener = new EmitILJackListener(Path.GetFileNameWithoutExtension(inputFile), parser, tokens, assemblyBuilder);
        walker.Walk(listener, tree);

        listener.RootClassType!.GetMethod("main")!.Invoke(null, null);
    }
}