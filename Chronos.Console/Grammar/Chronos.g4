grammar Chronos;

/*
  Parser rules
*/

command : create | trackAsset | add | stop | start | remove | replay | updateAssetPrice;    
query : bags; 

start : START TRACK;
stop : STOP TRACK;
replay : REPLAY date?;

create : CREATE ( createCoin | createBag );
createCoin : COIN name ticker guidOptional;
createBag : BAG name guidOptional;

trackAsset : TRACK asset? name duration;
updateAssetPrice : UPDATEPRICE asset? name price;  

add : ADD assetToBag;
assetToBag : quantity assetDescriptor to? bag? bagDescriptor;

remove : REMOVE removeAssetFromBag;
removeAssetFromBag : quantity assetDescriptor bagDescriptor;

to : TO;
bag : BAG;
bags : BAGS;

date : NUMBER NUMBER;
asset : COIN;
duration : NUMBER;
name : WORD;
price : ( DOUBLE | NUMBER );
ticker : WORD;
quantity : ( DOUBLE | NUMBER );
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
fragment M          : ('M'|'m') ;
fragment V          : ('V'|'v') ;
fragment L          : ('L'|'l') ;
fragment Y          : ('Y'|'y') ;

fragment LOWERCASE  : [a-z];
fragment UPPERCASE  : [A-Z];
fragment DIGIT     : [0-9];

CREATE : C R E A T E;
COIN : C O I N;
TRACK : T R A C K;
UPDATEPRICE : U P D A T E P R I C E;
GUID : G U I D;
BAG : B A G;
ADD : A D D;
BAGS : B A G S;
STOP : S T O P;
START : S T A R T;
REMOVE : R E M O V E;
TO : T O;
REPLAY : R E P L A Y;

WORD                : (LOWERCASE | UPPERCASE | '-')+ ;
WHITESPACE          : (' '|'\t')+ -> skip ;
NEWLINE             : ('\r'? '\n' | '\r')+ ;
NUMBER              : DIGIT+;
DOUBLE              : DIGIT+'.'DIGIT+;
