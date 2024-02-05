using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Pattern;
using JackInterpreter;
using System.Reflection.Emit;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputPath = @"C:\Users\colte\Downloads\project_09\Square";
        var outputFilePath = @"C:\Jack\Jack.il";

        var symbolTable = new DataSymbolTable();
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

        using (var outputFile = File.Create(outputFilePath))
        using (var outputStream = new StreamWriter(outputFile))
        {
            outputStream.WriteLine(".assembly extern mscorlib {}");
            outputStream.WriteLine($".assembly {JackDefinitions.JackAssemblyName} {{}}");

            foreach (var tree in sourceTrees)
            {
                EmitILJackListener emitILJackListener = new EmitILJackListener(
                    outputStream,
                    subroutineSymbolTable);
                walker.Walk(emitILJackListener, tree);
            }
            
        }
    }
}