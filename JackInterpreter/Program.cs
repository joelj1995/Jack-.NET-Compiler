using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Pattern;
using JackInterpreter;
using System.Reflection.Emit;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputFilePath = @"C:\Users\colte\Downloads\project_11\Seven\Main2.jack";
        var outputFilePath = @"C:\Jack\Jack.il";

        using (var outputFile = File.Create(outputFilePath))
        using (var inputFile = File.OpenRead(inputFilePath))
        using (var outputStream = new StreamWriter(outputFile))
        {
            AntlrInputStream antlrInputStream = new AntlrInputStream(inputFile);
            JackLexer lexer = new JackLexer(antlrInputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);

            JackParser parser = new JackParser(tokens);
            IParseTree tree = parser.classDeclaration();
            ParseTreeWalker walker = new();

            SymbolTable symbolTable = new SymbolTable();
            EmitILJackListener emitILJackListener = new EmitILJackListener(
                Path.GetFileNameWithoutExtension(inputFilePath), 
                parser, 
                tokens, 
                outputStream,
                symbolTable);
            PopulateSymbolTableJackListener symbolTableListener = new PopulateSymbolTableJackListener(symbolTable);

            //walker.Walk(symbolTableListener, tree);
            walker.Walk(emitILJackListener, tree);
        }
    }
}