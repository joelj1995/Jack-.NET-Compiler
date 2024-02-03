//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Jack.g4 by ANTLR 4.9.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="JackParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.3")]
[System.CLSCompliant(false)]
public interface IJackListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.classDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassDeclaration([NotNull] JackParser.ClassDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.classDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassDeclaration([NotNull] JackParser.ClassDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.classVarDec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassVarDec([NotNull] JackParser.ClassVarDecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.classVarDec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassVarDec([NotNull] JackParser.ClassVarDecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] JackParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] JackParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.subroutineDec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubroutineDec([NotNull] JackParser.SubroutineDecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.subroutineDec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubroutineDec([NotNull] JackParser.SubroutineDecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameterList([NotNull] JackParser.ParameterListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameterList([NotNull] JackParser.ParameterListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.subroutineBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubroutineBody([NotNull] JackParser.SubroutineBodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.subroutineBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubroutineBody([NotNull] JackParser.SubroutineBodyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.varDec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarDec([NotNull] JackParser.VarDecContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.varDec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarDec([NotNull] JackParser.VarDecContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.className"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassName([NotNull] JackParser.ClassNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.className"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassName([NotNull] JackParser.ClassNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.subroutineName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubroutineName([NotNull] JackParser.SubroutineNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.subroutineName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubroutineName([NotNull] JackParser.SubroutineNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.varName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarName([NotNull] JackParser.VarNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.varName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarName([NotNull] JackParser.VarNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.statements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatements([NotNull] JackParser.StatementsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.statements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatements([NotNull] JackParser.StatementsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] JackParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] JackParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.letStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLetStatement([NotNull] JackParser.LetStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.letStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLetStatement([NotNull] JackParser.LetStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.ifStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatement([NotNull] JackParser.IfStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.ifStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatement([NotNull] JackParser.IfStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatement([NotNull] JackParser.WhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatement([NotNull] JackParser.WhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.doStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatement([NotNull] JackParser.DoStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.doStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatement([NotNull] JackParser.DoStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] JackParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] JackParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] JackParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] JackParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTerm([NotNull] JackParser.TermContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTerm([NotNull] JackParser.TermContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.subroutineCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubroutineCall([NotNull] JackParser.SubroutineCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.subroutineCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubroutineCall([NotNull] JackParser.SubroutineCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.expressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionList([NotNull] JackParser.ExpressionListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.expressionList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionList([NotNull] JackParser.ExpressionListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.op"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOp([NotNull] JackParser.OpContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.op"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOp([NotNull] JackParser.OpContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="JackParser.unaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryOp([NotNull] JackParser.UnaryOpContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="JackParser.unaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryOp([NotNull] JackParser.UnaryOpContext context);
}
