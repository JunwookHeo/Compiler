%namespace CSJT11

%{
public static AST.CompilationUnit Root;

%}

%union
{
  public AST.CompilationUnit compilationUnit;
  public AST.ClassModifier classmodifier;
  public List<AST.ClassModifier> classmodifiers;
  public AST.Declaration declaration;
  public List<AST.Declaration> declarations;
  public AST.MethodDeclaration methodDeclaration;
  public AST.MethodModifier methodmodifier;
  public List<AST.MethodModifier> methodmodifiers;
  public AST.Type type;
  public AST.FormalParameter formalParameter;
  public List<AST.FormalParameter> formalParameters;
  public AST.Statement statement;
  public List<AST.Statement> statements;
  public AST.BlockStatements blockStatements;
  public AST.Expression expression;
  public AST.AssignmentExpression assignmentExpression;
  public int num;
  public char c;
  public string name;
  public float f;
  public List<string> nameList;
}

%token <name> Identifier
%token <num> IntegerLiteral
%token <f> FloatingPointLiteral

/*  3.9. Keywords  */
%token Abstract Continue For New Switch Assert Default If Package Synchronized Boolean Do Goto Private This Break Double Implements Protected Throw Byte Else Import Public Throws Case Enum Instanceof Return Transient Catch Extends Int Short Try Char Final Interface Static Void Class Finally Long Strictfp Volatile Const Float Native Super While Underscore

/* 3.11. Separators */
%token VariableArguments DoubleColon

/* 3.12. Operators */
%token Selection Equal GreaterOrEqual LessOrEqual NotEqual AndCondition OrCondition Increment Decrement SignedLeftShift SignedRightShift UnsignedRightShift AddAnd SubtractAnd MultiplyAnd DivideAnd BitwiseAnd BitwiseInclusiveOr BitwiseExclusiveOr ModulusAnd LeftShiftAnd RightShiftAnd ShiftRightZeroFill

%type <classmodifier>  ClassModifier
%type <classmodifiers>  ClassModifiers
%type <declaration>  ClassDeclaration NormalClassDeclaration ClassBodyDeclaration TypeDeclaration ClassMemberDeclaration LocalVariableDeclaration
%type <declarations>  ClassBodyDeclarations ClassBody TypeDeclarations 
%type <methodDeclaration>  MethodDeclaration MethodDeclarator MethodHeader 
%type <methodmodifiers> MethodModifiers
%type <methodmodifier> MethodModifier
%type <type> UnannType UnannReferenceType UnannArrayType UnannClassOrInterfaceType UnannClassType LocalVariableType UnannPrimitiveType NumericType IntegeralType Result FloatingPointType
%type <compilationUnit> CompilationUnit OrdinaryCompilationUnit
%type <formalParameters> FormalParameters FormalParameterList FormalParameterList_opt
%type <formalParameter> FormalParameter
%type <statements> BlockStatements_opt BlockStatements 
%type <statement> MethodBody Block Statement BlockStatement LocalVariableDeclarationStatement StatementWithoutTrailingSubstatement ExpressionStatement
%type <statement> ReturnStatement IfThenStatement

%type <nameList> VariableDeclarationList VariableDeclarators PackageDeclaration PackageDeclaration_opt Identifiers
%type <name> VariableDeclarator VariableDeclaratorId TypeIdentifier

%type <expression> Expression StatementExpression  LeftHandSide ExpressionName AssignmentExpression ConditionalExpression ConditionalOrExpression ConditionalAndExpression InclusiveOrExpression ExclusiveOrExpression AndExpression EqualityExpression RelationalExpression ShiftExpression AdditiveExpression MultiplicativeExpression UnaryExpression UnaryExpressionNotPlusMinus PostfixExpression Primary PrimaryNoNewArray Literal
%type <expression> Expression_opt PostIncrementExpression PreIncrementExpression 

%type <assignmentExpression> Assignment AssignmentOperator
/* %type <num> AssignmentOperator */

%left '='
%nonassoc '<'
%nonassoc '>'
%left '+' '-'
%left '*' '/'

%%

CompilationUnit
	:OrdinaryCompilationUnit { Root = $1; }
	;

OrdinaryCompilationUnit 
	:PackageDeclaration_opt ImportDeclarations TypeDeclarations   { $$ = new AST.CompilationUnit($1, $3); }
	;

PackageDeclaration_opt
    : PackageDeclaration  { $$ = $1; } 
	| /* empty */
	;
    
PackageDeclaration
    : PackageModifiers Package Identifiers Identifier ';' { $$ = $3; $$.Add($4); } 
    ;
    
PackageModifiers
    : /* empty */
    ; 
    
Identifiers
    : Identifiers Identifier '.'   { $$ = $1; $$.Add($2); }
    | /* empty */                   { $$ = new List<string>(); }
    ;
    
ImportDeclarations
	: /* empty */
	;

TypeDeclarations
	:TypeDeclarations TypeDeclaration  { $$ = $1; $$.Add($2); }
	| /* empty*/  { $$ = new List<AST.Declaration>(); }
	;

TypeDeclaration
	:ClassDeclaration   { $$ = $1; } 
	;

ClassDeclaration  
	:NormalClassDeclaration { $$ = $1; }
	;

NormalClassDeclaration
	:ClassModifiers Class TypeIdentifier TypeParameters_opt Superclass_opt Superinterfaces_opt ClassBody  { $$ = new AST.ClassDeclaration($1, $3, $7); }
	;

ClassModifiers
	:ClassModifiers ClassModifier  { $$ = $1; $$.Add($2); }
	| /* empty */ { $$ = new List<AST.ClassModifier>(); } 
	;

ClassModifier 
	:Public { $$ = AST.ClassModifier.Public; }
	;

TypeIdentifier
	:Identifier { $$ = $1; }
	;

TypeParameters_opt
	: /*empty*/
	;

Superclass_opt
	: /*empty*/
	;

Superinterfaces_opt
	: /*empty*/
	;

ClassBody
	:'{' ClassBodyDeclarations '}' { $$ = $2; } 
	;

ClassBodyDeclarations
	:ClassBodyDeclarations ClassBodyDeclaration { $$ = $1; $$.Add($2); } 
	|/*empty*/    { $$ = new List<AST.Declaration>(); }
	;

ClassBodyDeclaration
	:ClassMemberDeclaration { $$ = $1; }
	;

ClassMemberDeclaration
	:MethodDeclaration  { $$ = $1; }
  ;

MethodDeclaration 
  : MethodModifiers MethodHeader MethodBody  { $$ = $2; $$.SetMethodModifiers($1); $$.SetStatement($3); } 
  ;

MethodModifiers
  : MethodModifiers MethodModifier { $$ = $1; $$.Add($2); }
  | /* empty */ { $$ = new List<AST.MethodModifier>(); }
  ;

MethodModifier
  : Public  { $$ = AST.MethodModifier.Public; }
  | Static  { $$ = AST.MethodModifier.Static; }
  ;

MethodHeader
  : Result MethodDeclarator  { $$ = $2; $$.SetResult($1); } 
  ;

Result
	: Void { $$ = new AST.VoidType("void"); }
    | UnannType { $$ = $1; }
	;

MethodDeclarator
	: Identifier '(' ReceiverParameter_opt FormalParameterList_opt ')' Dims_opt  { $$ = new AST.MethodDeclaration(null, null, $1, $4, null); }
	;

ReceiverParameter_opt
	: /*empty*/
	;

FormalParameterList_opt
  : FormalParameterList  { $$ = $1; }
  ;

FormalParameterList
	: /*empty */
	| FormalParameters FormalParameter { $$ = $1; $$.Add($2); }
	;

FormalParameters
	: /*empty*/ { $$ = new List<AST.FormalParameter>();}
	| FormalParameters FormalParameter ',' { $$ = $1; $$.Add($2); }
	;

FormalParameter
	: VariableModifiers UnannType VariableDeclaratorId { $$ = new AST.FormalParameter($2, $3); }
	;

VariableDeclaratorId
	: Identifier Dims_opt   { $$ = $1; }
	;

Dims_opt
	: /*empty*/
	;

UnannType
	: UnannReferenceType  { $$ = $1; }
  | UnannPrimitiveType  { $$ = $1; }
	;

UnannReferenceType
	: UnannArrayType { $$ = $1; }
	;

UnannArrayType
	: UnannClassOrInterfaceType Dims  { $$ = new AST.ArrayType($1); } /* TODO: Should check Dims and make array type */
	;

UnannClassOrInterfaceType
	: UnannClassType  { $$ = $1; }
	;

UnannClassType
	: TypeIdentifier TypeArguments_opt { $$ = new AST.NameType($1); }
	;

TypeArguments_opt
	:/*empty*/
	;

/* TODO: Should check Dims     and make array type */
Dims
	: DimsArray DimsArrayOpt
  ;

DimsArrayOpt
 	: /* empty */
 	| DimsArray
 	;

DimsArray
 	: Annotations '[' ']'
 	;

Annotations
 	: /* empty */
  ;


MethodBody
	:Block  { $$ = $1; }
	;

Block
	: '{' BlockStatements_opt '}' { $$ = new AST.BlockStatements($2); }
	;

BlockStatements_opt
	: BlockStatements { $$ = $1; }
	;

BlockStatements
	: BlockStatements BlockStatement { $$ = $1; $$.Add($2); }
	| /*empty*/  { $$ = new List<AST.Statement>(); }
	;
    
BlockStatement
	: LocalVariableDeclarationStatement { $$ = $1; }
    | VariableModifiers Statement { $$ = $2; }
	;

LocalVariableDeclarationStatement
  : LocalVariableDeclaration ';'  { $$ = new AST.LocalVariableDeclarationStatement($1); }
  ;

LocalVariableDeclaration
	: VariableModifiers LocalVariableType VariableDeclarationList { $$ = new AST.LocalVariableDeclaration($2, $3); }
	;

VariableModifiers
	:/*empty*/
	;
LocalVariableType
	: UnannType { $$ = $1; }
	;

UnannPrimitiveType
	: NumericType { $$ = $1; }
	;

NumericType
	: IntegeralType { $$ = $1; }
    | FloatingPointType { $$ = $1; }
	;

IntegeralType
	: Int  { $$ = new AST.PrimitiveType(AST.UnannPrimitiveType.Int); }
  | Byte { $$ = new AST.PrimitiveType(AST.UnannPrimitiveType.Byte); }
  | Short { $$ = new AST.PrimitiveType(AST.UnannPrimitiveType.Short); }
  | Long { $$ = new AST.PrimitiveType(AST.UnannPrimitiveType.Long); }
  | Char { $$ = new AST.PrimitiveType(AST.UnannPrimitiveType.Char); }
	;
	
FloatingPointType
    : Float { $$ = new AST.PrimitiveType(AST.UnannPrimitiveType.Float); }
    ; 

VariableDeclarationList
	: VariableDeclarators VariableDeclarator { $$ = $1; $$.Add($2); }
	| /*empty */
	;

VariableDeclarators
	: VariableDeclarators VariableDeclarator ',' { $$ = $1; $$.Add($2); }
	| /*empty*/ { $$ = new List<string>(); }
	;

VariableDeclarator
	: VariableDeclaratorId VariableInitializer_opt { $$ = $1; }
	;

VariableInitializer_opt
	: /*empty*/
	;

Statement
  : StatementWithoutTrailingSubstatement  { $$ = $1; }
  | IfThenStatement { $$ = $1; }
  ;

StatementWithoutTrailingSubstatement
  : Block { $$ = $1; }
  | ExpressionStatement { $$ = $1; }
  | ReturnStatement     { $$ = $1; }
  ;

ExpressionStatement
  : StatementExpression ';' { $$ = new AST.ExpressionStatement($1); }
  ;

StatementExpression
  : Assignment  { $$ = $1; }
  | PreIncrementExpression { $$ = $1; }
  | PostIncrementExpression { $$ = $1; }
  ;

Assignment
  : LeftHandSide AssignmentOperator Expression  { $$ = $2; $$.SetExpressions($1, $3); } 
  ;

LeftHandSide
  : ExpressionName { $$ = $1; }
  ;

ExpressionName
  : Identifier  { $$ = new AST.ExpressionName($1); }
  ;

AssignmentOperator
  : '=' { $$ = new AST.AssignmentExpression(null, null); } /* '=' */
  | BitwiseAnd { $$ = new AST.AndAssignmentExpression(null, null); } /* '&=' */
  ;

Expression
  : AssignmentExpression { $$ = $1; }
  ;

AssignmentExpression
  : ConditionalExpression { $$ = $1; }
  | Assignment { $$ = $1; }
  ;

ConditionalExpression
  : ConditionalOrExpression { $$ = $1; }
  ;

ConditionalOrExpression
  : ConditionalAndExpression  { $$ = $1; }
  ;

ConditionalAndExpression
  : InclusiveOrExpression { $$ = $1; }
  ;

InclusiveOrExpression
  : ExclusiveOrExpression  { $$ = $1; }
  ;

ExclusiveOrExpression
  : AndExpression { $$ = $1; }
  ;

AndExpression
  : EqualityExpression { $$ = $1; }
  | AndExpression '&' EqualityExpression  { $$ = new AST.AndBinaryExpression($1, $3); }
  ;

EqualityExpression
  : RelationalExpression  { $$ = $1; }
  | EqualityExpression Equal RelationalExpression { $$ = new AST.EqualBinaryExpression($1, $3); }
  ;

RelationalExpression
  : ShiftExpression  { $$ = $1; }
  ;

ShiftExpression
  : AdditiveExpression { $$ = $1; }
  ;

AdditiveExpression
  : MultiplicativeExpression { $$ = $1; }
  | MultiplicativeExpression '+' AdditiveExpression   { $$ = new AST.AddBinaryExpression($1, $3); }
  | MultiplicativeExpression '-' AdditiveExpression   { $$ = new AST.SubBinaryExpression($1, $3); }
  ;
   
MultiplicativeExpression
  : UnaryExpression { $$ = $1; }
  ;

UnaryExpression
  : PreIncrementExpression  { $$ = $1; }
  | UnaryExpressionNotPlusMinus { $$ = $1; }
  ;

UnaryExpressionNotPlusMinus
  : PostfixExpression { $$ = $1; }
  ;

PostfixExpression
  : Primary { $$ = $1; }
  | ExpressionName { $$ = $1; }
  | PostIncrementExpression { $$ = $1; }
  ;

Primary
  : PrimaryNoNewArray { $$ = $1; }
  ;
PrimaryNoNewArray
  : Literal { $$ = $1; }
  ;

Literal
  : IntegerLiteral  { $$ = new AST.IntegerLiteral($1); }
  | FloatingPointLiteral { $$ = new AST.FloatingPointLiteral($1); }
  ;

PreIncrementExpression
  : Increment UnaryExpression { $$ = new AST.PreIncUnaryExpression($2); }
  ;
  
PostIncrementExpression
  : PostfixExpression Increment { $$ = new AST.PostIncUnaryExpression($1); }
  ;
  
ReturnStatement
  : Return Expression_opt ';' { $$ = new AST.ReturnStatement($2); }
  ;
  
Expression_opt
  : Expression { $$ = $1; }
  | /*empty*/ 
  ;
  
IfThenStatement
  : If '(' Expression ')' Statement { $$ = new AST.IfThenStatement($3, $5); }
  ;

%%

public Parser(Scanner scanner) : base(scanner)
{
}

