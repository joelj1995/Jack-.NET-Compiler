lexer grammar JackLexerRules;

KEYWORD:
	'class'
	| 'constructor'
	| 'function'
	| 'method'
	| 'field'
	| 'static'
	| 'var'
	| 'int'
	| 'char'
	| 'boolean'
	| 'void'
	| 'true'
	| 'false'
	| 'null'
	| 'this'
	| 'let'
	| 'do'
	| 'if'
	| 'else'
	| 'while'
	| 'return';

SYMBOL:
	'{'
	| '}'
	| '('
	| ')'
	| '['
	| ']'
	| '.'
	| ','
	| ';'
	| '+'
	| '-'
	| '*'
	| '/'
	| '&'
	| '|'
	| '<'
	| '>'
	| '='
	| '~';

ID: [a-zA-Z]+;
INTCONST: [0-9]+;
STRINGCONST: '"' ~('\\' | '"')* '"';

NEWLINE: '\r'? '\n' -> channel(HIDDEN);
COMMENT:
	'/*' .*? '*/' -> channel(HIDDEN); // match anything between /* and */
WS: [ \r\t\u000C\n]+ -> channel(HIDDEN);

LINE_COMMENT: '//' ~[\r\n]* '\r'? '\n' -> channel(HIDDEN);