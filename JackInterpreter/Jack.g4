grammar Jack;

import JackLexerRules;

// PROGRAM STRUCTURE

classDeclaration:
	'class' className '{' classVarDec* subroutineDec* '}';

classVarDec: ('static' | 'field') type varName (',' varName)* ';';

type: 'int' | 'char' | 'boolean' | className;

subroutineDec:
	subroutineDecModifier ('void' | type) subroutineName '(' parameterList ')' subroutineBody;

subroutineDecModifier:
	'constructor'	# Constructor
	| 'function'	# Function
	| 'method'		# Method;

parameterList: ((parameter) (',' parameter)*)?;

parameter: type varName;

subroutineBody: '{' varDec* statements '}';

varDec: 'var' type varName (',' varName)* ';';

className: ID;

subroutineName: ID;

varName: ID;

// STATEMENTS

statements: statement*;

statement:
	letStatement		# StatementForLet
	| ifStatement		# StatementForIf
	| whileStatement	# StatementForWhile
	| doStatement		# StatementForDo
	| returnStatement	# StatementForReturn;

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
	| keywordConst
	| varName
	| varName '[' expression ']'
	| subroutineCall
	| '(' expression ')'
	| unaryOp term;

subroutineCall:
	subroutineName '(' expressionList ')'								# ThisMethod
	| (className | varName) '.' subroutineName '(' expressionList ')'	# ThatMethod;

expressionList: (expression (',' expression)*)?;

op: '+' | '-' | '*' | '/' | '&' | '|' | '<' | '>' | '=';
unaryOp: '-' | '~';
keywordConst: 'true' | 'false' | 'null' | 'this';