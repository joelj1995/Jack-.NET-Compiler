using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Pattern;
using JackInterpreter;
using System.CodeDom.Compiler;
using System.Reflection.Emit;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputPath = @"C:\Users\colte\Downloads\project_11\Pong";
        var outputFilePath = @"C:\Jack\Jack.il";

        var subroutineSymbolTable = new SubroutineSymbolTable();

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass", 
            "dispose", 
            SubroutineKind.METHOD, 
            Array.Empty<string>(), 
            true, 
            "void");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "length",
            SubroutineKind.METHOD,
            new string[] { },
            true,
            "int16");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "charAt",
            SubroutineKind.METHOD,
            new string[] { "int16" },
            true,
            "char");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "appendChar",
            SubroutineKind.METHOD,
            new string[] { "char" },
            true,
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "eraseLastChar",
            SubroutineKind.METHOD,
            new string[] { },
            true,
            "void");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "intValue",
            SubroutineKind.METHOD,
            new string[] { },
            true,
            "int16");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "setInt",
            SubroutineKind.METHOD,
            new string[] { "int16" },
            true,
            "void");

        subroutineSymbolTable.Define(
            "class [NJackOS.Interface]NJackOS.Interface.JackStringClass",
            "setCharAt",
            SubroutineKind.METHOD,
            new string[] { "int16", "char" },
            true,
            "void");

        var walker = new ParseTreeWalker();
        var sourceTrees = new List<IParseTree>();
        foreach (var inputFilePath in Directory.GetFiles(inputPath))
        {
            if (!inputFilePath.EndsWith(".jack")) 
                continue;

            var inputFile = File.OpenRead(inputFilePath);

            var symbolTableListener = new PopulateSubroutineTableJackListener(subroutineSymbolTable);
            AntlrInputStream antlrInputStream = new AntlrInputStream(inputFile);
            JackLexer lexer = new JackLexer(antlrInputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);

            JackParser parser = new JackParser(tokens);
            IParseTree tree = parser.classDeclaration();

            walker.Walk(symbolTableListener, tree);
            sourceTrees.Add(tree);
        }

        var baseTextWriter = new StringWriter();
        var writer = new IndentedTextWriter(baseTextWriter, "    ");
        using (var outputFile = File.Create(outputFilePath))
        using (var outputStream = new StreamWriter(outputFile))
        {
            writer.WriteLine(".assembly extern mscorlib {}");
            writer.WriteLine(".assembly extern NJackOS.Interface {}");
            writer.WriteLine($".assembly {JackDefinitions.JackAssemblyName} {{}}");

            foreach (var tree in sourceTrees)
            {
                EmitILJackListener emitILJackListener = new EmitILJackListener(
                    writer,
                    subroutineSymbolTable);
                walker.Walk(emitILJackListener, tree);
                writer.WriteLine();
            }
            
            outputStream.Write(baseTextWriter.ToString());
        }
    }
}