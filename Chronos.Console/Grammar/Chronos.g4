grammar Chronos;

/*
  Parser rules
*/

createCoin : CREATE COIN name;
name : WORD;

/*
  Lexer Rules
*/

fragment C          : ('C'|'c') ;
fragment R          : ('R'|'r') ;
fragment E          : ('E'|'e') ;
fragment A          : ('A'|'a') ;
fragment T          : ('T'|'t') ;
fragment O          : ('O'|'o') ;
fragment I          : ('I'|'i') ;
fragment N          : ('N'|'n') ;

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;

CREATE : C R E A T E;
COIN : C O I N;

WORD                : (LOWERCASE | UPPERCASE)+ ;
WHITESPACE          : (' '|'\t')+ -> skip ;
NEWLINE             : ('\r'? '\n' | '\r')+ ;