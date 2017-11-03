grammar Rsql;


LETTER	:	'a'..'z'|'A'..'Z';
DIGIT	:	'0'..'9';
NUMBER	:	('-'|'+')?'0'..'9'+('.''0'..'9'+)?;
DATE	:	'0'..'9' '0'..'9' '0'..'9' '0'..'9' '-' '0'..'9' '0'..'9' '-' '0'..'9' '0'..'9' ('T' '0'..'9' '0'..'9' ':' '0'..'9' '0'..'9' ':' '0'..'9' '0'..'9'(('+'|'-')'0'..'9' '0'..'9' ':' '0'..'9' '0'..'9')?)? ;
ALPHA	:	LETTER+;
ANY : . ; 

selector	: ~('\'' |  '(' | ')' | ';' | ',' | '=' | '~' | '<' | '>' |' ')+;	

eval 	:	
	or
	;
	
or	:
	and  (','and)*
	;

and 	:
	constraint (';'constraint)*
	;
	
constraint	:
 	group 
 	| comparison
;

group	:
	'('or')'
	;

comparison	
	:
	selector comparator (arguments)?
	;
	
comparator	
	:
	comp_fiql 
	| comp_alt
	;

comp_fiql	
	:
	'='(ALPHA('-'ALPHA)*)?'='
	;

comp_alt	
	:
	 ('>' | '<')'='? 
	;
		 	
reserved 	
	: 
	'\'' |  '(' | ')' | ';' | ',' | '=' | '~' | '<' | '>' |' '
	;
   
single_quote
	:
	 '\''('\\\'' | ~('\''))* '\''
	;

arguments
	:
	'(' value ( ',' value)* ')' 
	| value 
	;
value	
	: DATE
	| NUMBER
	| DIGIT
	| single_quote
	;


