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
        var inputPath = @"C:\Users\colte\Downloads\project_12\ScreenTest";
        var outputFilePath = @"C:\Jack\Jack.il";

        var subroutineSymbolTable = new SubroutineSymbolTable();

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