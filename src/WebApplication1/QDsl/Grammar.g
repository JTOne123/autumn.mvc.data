grammar rsqlgrammar;

PARSER
 :  EXPRESSION EOF
 ;

COMPARISON_OPERATOR
 : GT 
 | GE 
 | LT 
 | LE 
 | EQ 
 | NOT EQ
 ;

CONTENT_COMPARISON_OPERATOR
 : IN
 | NOT_IN	
 ;

BINARY_OPERATOR
 : AND 
 | OR
 ;

BOOL
 : TRUE 
 | FALSE
 ;

PRIMITIVE
 : DIGIT
 | STRING
 | BOOL 
 ;

COMPARISON
 :left=PRIMITIVE op=COMPARISON_OPERATOR right=IDENTIFIER
  | left=IDENTIFIER op=COMPARISON_OPERATOR right=IDENTIFIER
  | left=IDENTIFIER op=COMPARISON_OPERATOR right=PRIMITIVE
  | left=IDENTIFIER op=CONTENT_COMPARISON_OPERATOR rigth=L_PAREN PRIMITIVE ((OR PRIMITIVE)+)? R_PAREN
  | left=L_PAREN PRIMITIVE ((OR PRIMITIVE)+)? R_PAREN op=CONTENT_COMPARISON_OPERATOR rigth=IDENTIFIER
  | BOOL	
;
  
 EXPRESSION
 :left = COMPARISON  (op = BINARY_OPERATOR right=COMPARISON)* 
 ; 
 
AND       	: ';' ;
OR         	: ',' ;
NOT        	: '!';
TRUE       	: 't' 'r' 'u' 'e' ;
FALSE      	: 'f' 'a' 'l' 's' 'e' ;
GT         	: '>' | '=' 'g' 't' '=' ;
GE        	: '>=' | '=' 'g' 'e' '=';
LT        	: '<' | '=' 't' '=' ;
LE         	: '<' '=' | '=' 'l' 'e' '=' ;
EQ         	: '=' '=' | '=' 'e' 'q' '=' ;
IN         	: '=' 'i' 'n' '=';
NOT_IN 	:  '=' 'o' 'u' 't' '=';
L_PAREN     	: '(' ;
R_PAREN     	: ')' ;
DIGIT    	: '-'? '0'..'9'('0'..'9')* ( '.' '0'..'9' ('0'..'9')*)? ;
STRING	: '\'' ~('\'')* '\'' ;	
IDENTIFIER 	: '[' ('a'..'z'|'A'..'Z'|'0'..'9'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_'|'.')*  ']' ;
