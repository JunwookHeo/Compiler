%namespace CSJT11

%{
	int lines = 0;
%}

/* 3.3. Unicode Escapes */
RawInputCharacter			[.]*	
HexDigit							[0-9a-fA-F]
UnicodeMarker					[u]+
UnicodeEscape					\\{UnicodeMarker}{HexDigit}{HexDigit}{HexDigit}{HexDigit}
UnicodeInputCharacter	{UnicodeEscape}|{RawInputCharacter}

/* 3.6. White Space */
LineTerminator	  \n
WhiteSpace				" "|\t|\r|{LineTerminator}

/* 3.7. Comments */
InputCharacter			{UnicodeInputCharacter}
EndOfLineComment		\/\/{InputCharacter}*
NotStarNotSlash			[^\*\/]
TraditionalComment	\/\*{NotStarNotSlash}*\*\/
Comment							{TraditionalComment}|{EndOfLineComment}

/*3.8. Identifiers */
JavaLetter				[a-zA-Z_]
JavaLetterOrDigit [a-zA-Z0-9_]
IdentifierChars		{JavaLetter}{JavaLetterOrDigit}*
Identifier				{IdentifierChars} 

/* 3.10.1. Integer Literals */
/* Decimal Interger literal */
IntegerTypeSuffix		l|L
NonZeroDigit			[1-9]
Digit					0|{NonZeroDigit}
Underscores				[\_]+
DigitOrUnderscore		{Digit}|\_
DigitsAndUnderscores	{DigitOrUnderscore}+
Digits					{Digit}|{Digit}{DigitsAndUnderscores}?{Digit}
DecimalNumeral			0|{NonZeroDigit}{Digits}?|{NonZeroDigit}{Underscores}{Digits}
DecimalIntegerLiteral	{DecimalNumeral}{IntegerTypeSuffix}?

/* Octal Interger literal */
OctalDigit              [0-7]
OctalDigitOrUnderscore  {OctalDigit}|\_ 
OctalDigitsAndUnderscores   {OctalDigitOrUnderscore}+ 
OctalDigits             {OctalDigit}|{OctalDigit}{OctalDigitsAndUnderscores}?{OctalDigit} 
OctalNumeral            0{Underscores}?{OctalDigits}
OctalIntegerLiteral     {OctalNumeral}{IntegerTypeSuffix}?

/*
TODO : Add belows to IntegerLiteral
|{HexIntegerLiteral} 
|{BinaryIntegerLiteral}
*/

/* 3.10.2. String Literals  (not very sure)*/
ZeroToThree [0-3]
OctalEscape ^\\{OctalDigit}|^\\{OctalDigit}{OctalDigit}|^\\{ZeroToThree}{OctalDigit}{OctalDigit} 
EscapeSequence \b|\t|\n|\f|\r|\"|\'|\\|{OctalEscape}
StringCharacter [^\"\\\r\n]|{EscapeSequence}
StringLiteral "{StringCharacter}*"


%%

/* 3.7. Comments */
{Comment}					/* Ignore Comment */

/*  3.9. Keywords  */
abstract					{ return (int)Tokens.Abstract; }  
continue					{ return (int)Tokens.Continue; }
for								{ return (int)Tokens.For; } 
new								{ return (int)Tokens.New; }
switch						{ return (int)Tokens.Switch; }
assert						{ return (int)Tokens.Assert; }
default						{ return (int)Tokens.Default; }
if								{ return (int)Tokens.If; }
package						{ return (int)Tokens.Package; }
synchronized			{ return (int)Tokens.Synchronized; }
boolean						{ return (int)Tokens.Boolean; }
do								{ return (int)Tokens.Do; }
goto							{ return (int)Tokens.Goto; }
private						{ return (int)Tokens.Private; }
this							{ return (int)Tokens.This; }
break							{ return (int)Tokens.Break; }
double						{ return (int)Tokens.Double; }
implements				{ return (int)Tokens.Implements; }
protected					{ return (int)Tokens.Protected; }
throw							{ return (int)Tokens.Throw; }
byte							{ return (int)Tokens.Byte; }
else							{ return (int)Tokens.Else; }
import						{ return (int)Tokens.Import; }
public						{ return (int)Tokens.Public; }
throws						{ return (int)Tokens.Throws; }
case							{ return (int)Tokens.Case; }
enum							{ return (int)Tokens.Enum; }
instanceof				{ return (int)Tokens.Instanceof; }
return						{ return (int)Tokens.Return; }
transient					{ return (int)Tokens.Transient; }
catch							{ return (int)Tokens.Catch; }
extends						{ return (int)Tokens.Extends; }
int								{ return (int)Tokens.Int; }
short							{ return (int)Tokens.Short; }
try								{ return (int)Tokens.Try; }
char							{ return (int)Tokens.Char; }
final							{ return (int)Tokens.Final; }
interface					{ return (int)Tokens.Interface; }
static						{ return (int)Tokens.Static; }
void							{ return (int)Tokens.Void; }
class							{ return (int)Tokens.Class; }
finally						{ return (int)Tokens.Finally; }
long							{ return (int)Tokens.Long; }
strictfp					{ return (int)Tokens.Strictfp; }
volatile					{ return (int)Tokens.Volatile; }
const							{ return (int)Tokens.Const; }
float							{ return (int)Tokens.Float; }
native						{ return (int)Tokens.Native; }
super							{ return (int)Tokens.Super; }
while							{ return (int)Tokens.While; }
_									{ return (int)Tokens.Underscore; }

/*3.8. Identifiers */
{Identifier}      { yylval.name = yytext; return (int)Tokens.Identifier; } 

/* 3.10.1. Integer Literals */
{DecimalIntegerLiteral}	    { yylval.num = int.Parse(yytext); return (int)Tokens.IntegerLiteral; }
{OctalIntegerLiteral}       { yylval.num = System.Convert.ToInt32(yytext.Replace("_", ""), 8); return (int)Tokens.IntegerLiteral; }

/* 3.11. Separators */
"(" 							{return '(';}
")" 							{return ')';}
"{" 							{return '{';}
"}" 							{return '}';}
"[" 							{return '[';}
"]" 							{return ']';}
";" 							{return ';';}
"," 							{return ',';}
"." 							{return '.';}
"..." 						{return (int)Tokens.VariableArguments;}
"@" 							{return '@';}
"::" 							{return (int)Tokens.DoubleColon;}

/* 3.12. Operators */
"=" 							{return '=';}
">"								{return '>';}
"<"								{return '<';}
"!"								{return '!';}
"~"								{return '~';}
":"								{return ':';}
"?"								{return '?';}
"->"							{return (int)Tokens.Selection;}
"=="							{return (int)Tokens.Equal;}
">="							{return (int)Tokens.GreaterOrEqual;}
"<="							{return (int)Tokens.LessOrEqual;}
"!="							{return (int)Tokens.NotEqual;}
"&&"							{return (int)Tokens.AndCondition;}
"||"							{return (int)Tokens.OrCondition;}
"++"							{return (int)Tokens.Increment;}
"--" 							{return (int)Tokens.Decrement;}
"+" 							{return '+';}
"-" 							{return '-';}
"*" 							{return '*';}
"/" 							{return '/';}
"&" 							{return '&';}
"|" 							{return '|';}
"^" 							{return '^';}
"%" 							{return '%';}
"<<" 							{return (int)Tokens.SignedLeftShift;}
">>" 							{return (int)Tokens.SignedRightShift;}
">>>" 						{return (int)Tokens.UnsignedRightShift;}
"+=" 							{return (int)Tokens.AddAnd;}
"-=" 							{return (int)Tokens.SubtractAnd;}
"*=" 							{return (int)Tokens.MultiplyAnd;}
"/=" 							{return (int)Tokens.DivideAnd;}
"&=" 							{return (int)Tokens.BitwiseAnd;}
"|=" 							{return (int)Tokens.BitwiseInclusiveOr;}
"^=" 							{return (int)Tokens.BitwiseExclusiveOr;}
"%="							{return (int)Tokens.ModulusAnd;}
"<<="							{return (int)Tokens.LeftShiftAnd;}
">>="							{return (int)Tokens.RightShiftAnd;}
">>>="						{return (int)Tokens.ShiftRightZeroFill;}


/* 3.6. White Space */
{LineTerminator}	{ lines++; }
{WhiteSpace}			/* Ignore WhiteSpace except LineTerminator */


.	{
		throw new Exception(
		    String.Format("unexpected character '{0}'", yytext));
}	

%%

public override void yyerror( string format, params object[] args )
{
    System.Console.Error.WriteLine("Error: line {0}, {1}", lines,
        String.Format(format, args));
}

