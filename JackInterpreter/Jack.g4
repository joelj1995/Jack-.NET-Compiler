grammar Jack;

import JackLexerRules;

// PROGRAM STRUCTURE

classDeclaration:
	'class' className '{' classVarDec* subroutineDec* '}';

classVarDec:
	classVarDecModifier type varName (',' varName)* ';';

classVarDecModifier: 'static' # StaticDec | 'field' # FieldDec;

type:
	'int'		# TypeInt
	| 'char'	# TypeChar
	| 'boolean'	# TypeBool
	| className	# TypeClass;

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
	'if' '(' expression ')' '{' ifBody '}' (
		'else' '{' elseBody '}'
	)?;

ifBody: statements;
elseBody: statements;

whileStatement: 'while' '(' expression ')' '{' statements '}';

doStatement: 'do' subroutineCall ';';

returnStatement: 'return' expression? ';';

// EXPRESSIONS

expression: term (binaryOp)*;
binaryOp: op term;
term:
	litteralConst					# TermConst
	| keywordConst					# TermKeywordConst
	| varName						# TermVarName
	| varName '[' expression ']'	# TermArrayIndex
	| subroutineCall				# TermSubroutineCalls
	| '(' expression ')'			# TermParen
	| unaryOp term					# TermUnary;

subroutineCall:
	subroutineName '(' expressionList ')'								# ThisMethod
	| (className | varName) '.' subroutineName '(' expressionList ')'	# ThatMethod;

expressionList: (expression (',' expression)*)?;

op:
	'+'		# OpPlus
	| '-'	# OpMinus
	| '*'	# OpTimes
	| '/'	# OpDivide
	| '&'	# OpAnd
	| '|'	# OpOr
	| '<'	# OpLessThan
	| '>'	# OpGreaterThan
	| '='	# OpEqual;

unaryOp: '-' # UnaryOpMinus | '~' # UnaryOpNot;

keywordConst:
	'true'		# ConstTrue
	| 'false'	# ConstFalse
	| 'null'	# ConstNull
	| 'this'	# ConstThis;

litteralConst: INTCONST # ConstInt | STRINGCONST # ConstString;