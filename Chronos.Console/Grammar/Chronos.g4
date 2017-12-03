grammar Chronos;

/*
  Parser rules
*/

command : create | trackAsset | add | stop;  
query : bags; 

stop : STOP TRACK;

create : CREATE ( createCoin | createBag );
createCoin : COIN name ticker guidOptional;
createBag : BAG name guidOptional;

trackAsset : TRACK asset? name duration;

add : ADD assetToBag;
assetToBag : quantity assetDescriptor to? bag? bagDescriptor;

to : TO;
bag : BAG;
bags : BAGS;

asset : COIN;
duration : NUMBER;
name : WORD;
ticker : WORD;
quantity : DOUBLE;
guid : WORD;
guidOptional : newGuid | WORD;
newGuid : NEWLINE;

bagDescriptor : guid | WORD;
assetDescriptor : guid | WORD;

/*
  Lexer Rules
*/

fragment C          : ('C'|'c') ;
fragment R          : ('R'|'r') ;
fragment E          : ('E'|'e') ;
fragment A          : ('A'|'a') ;
fragment B          : ('B'|'b') ;
fragment T          : ('T'|'t') ;
fragment O          : ('O'|'o') ;
fragment I          : ('I'|'i') ;
fragment N          : ('N'|'n') ;
fragment K          : ('K'|'k') ;
fragment W          : ('W'|'w') ;
fragment G          : ('G'|'g') ;
fragment U          : ('U'|'u') ;
fragment D          : ('D'|'d') ;
fragment S          : ('S'|'s') ;
fragment P          : ('P'|'p') ;

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment DIGIT     : [0-9] ;

CREATE : C R E A T E;
COIN : C O I N;
TRACK : T R A C K;
GUID : G U I D;
BAG : B A G;
ADD : A D D;
BAGS : B A G S;
STOP : S T O P;
TO : T O;

WORD                : (LOWERCASE | UPPERCASE | '-')+ ;
WHITESPACE          : (' '|'\t')+ -> skip ;
NEWLINE             : ('\r'? '\n' | '\r')+ ;
NUMBER              : DIGIT+;
DOUBLE              : DIGIT+'.'?DIGIT*;
