grammar Query;


LETTER	:	'a'..'z'|'A'..'Z';
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
	('!='|'='(ALPHA('-'ALPHA)*)?'=')
	;

comp_alt	
	:
	 ('>' | '<')'='? 
	;
		 	
reserved 	
	: 
	'\'' |  '(' | ')' | ';' | ',' | '=' | '~' | '<' | '>' |' '
	;

unreserved
	: ~(  '(' | ')' | ';' | ',' | '=' | '~' | '<' | '>');
	
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
	: 
	unreserved+;


