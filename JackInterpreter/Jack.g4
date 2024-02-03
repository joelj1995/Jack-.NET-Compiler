grammar Jack;

import JackLexerRules;

// PROGRAM STRUCTURE

classDeclaration:
	'class' ID '{' classVarDec* subroutineDec* '}';

classVarDec: ('static' | 'field') type varName (',' varName)* ';';

type: 'int' | 'char' | 'boolean' | className;

subroutineDec: ('constructor' | 'function' | 'method') (
		'void'
		| type
	) subroutineName '(' parameterList ')' subroutineBody;

parameterList: ((type varName) (',' type varName)*)?;

subroutineBody: '{' varDec* statements '}';

varDec: 'var' type varName (',' varName)* ';';

className: ID;

subroutineName: ID;

varName: ID;

// STATEMENTS

statements: statement*;

statement:
	letStatement
	| ifStatement
	| whileStatement
	| doStatement
	| returnStatement;

letStatement:
	'let' varName ('[' expression ']')? '=' expression ';';

ifStatement:
	'if' '(' expression ')' '{' statements '}' (
		'else' '{' statements '}'
	)?;

whileStatement: 'while' '(' expression ')' '{' statements '}';

doStatement: 'do' subroutineCall ';';

returnStatement: 'return' expression? ';';

// EXPRESSIONS

expression: term (op term)*;
term:
	INTCONST
	| STRINGCONST
	| KEYWORDCONST
	| varName
	| varName '[' expression ']'
	| subroutineCall
	| '(' expression ')'
	| unaryOp term;

subroutineCall:
	subroutineName '(' expressionList ')'
	| (className | varName) '.' subroutineName '(' expressionList ')';

expressionList: (expression (',' expression)*)?;

op: '+' | '-' | '*' | '/' | '&' | '|' | '<' | '>' | '=';
unaryOp: '-' | '~';
KEYWORDCONST: 'true' | 'false' | 'null' | 'this';