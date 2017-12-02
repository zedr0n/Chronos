grammar Chronos;

/*
  Parser rules
*/

command : createCoin | trackCoin;
createCoin : CREATE COIN name ticker guid;
trackCoin : TRACK COIN name duration;
duration : NUMBER;
name : WORD;
ticker : WORD;
guid : newGuid | WORD;
newGuid : NEWLINE;

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
fragment K          : ('K'|'k') ;
fragment W          : ('W'|'w') ;
fragment G          : ('G'|'g') ;
fragment U          : ('U'|'u') ;
fragment D          : ('D'|'d') ;

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment DIGIT     : [0-9] ;

CREATE : C R E A T E;
COIN : C O I N;
TRACK : T R A C K;
GUID : G U I D;

WORD                : (LOWERCASE | UPPERCASE)+ ;
WHITESPACE          : (' '|'\t')+ -> skip ;
NEWLINE             : ('\r'? '\n' | '\r')+ ;
NUMBER              : DIGIT+;