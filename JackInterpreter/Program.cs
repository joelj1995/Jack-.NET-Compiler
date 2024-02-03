using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Tree.Pattern;

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

        Console.WriteLine(tree.ToStringTree(parser));
    }
}