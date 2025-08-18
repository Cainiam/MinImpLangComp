# MinImpLangComp Grammar specification

## Introduction  

This document defines the lexical tokens and the grammar (EBNF) for MinImpLangComp, a small imperative language built for learning (interpreter, IL generation, and C# transpilation).

## Author  

Jordan Boisdenghien  

## Tokens  

The following lexemes map directly to MinImpLangComp.Lexing.TokenType.
| <b>Token type</b> | <b>Description</b> | <b>Examples</b> |  
|------------|-------------|----------|  
| Integer | Integer numbers | ```"10"```, ```"0"```, ```"1369"``` |  
| Float | Floating point numbers (Double) | ```"3.14"```, ```"0.9"```, ```".5"```, ```"97."``` |  
| StringLiteral | string literal with escapes | ```"hello"```, ```"line\n"``` |  
| Identifier | Variable/function name | ```"x"```, ```"var1"```, ```"lastSum"``` |  
| <b>Keywords</b> |
| Set | Variable declarations | ```set``` |  
| Bind | Constant declarations | ```bind``` |  
| If / Else | Conditional | ```if```, ```else``` |
| While / For | Loop | ```while```, ```for``` |
| Function | Function declaration | ```function``` |  
| True / False | Boolean literals | ```true```, ```false``` |  
| Null | Null literal | ```null``` |  
| Break / Continue | Loop control | ```break```, ```continue``` | 
| Return | Return statement | ```return``` |  
| <b>Types</b> | Type keywords for annotations | ```int```, ```float```, ```bool```, ```string``` |  
| <b>Operators</b> |
| Plus / Minus | Addition / substraction | ```+```, ```-``` |  
| Multiply / Divide / Modulo | Multiplication / division / modulo | ```*```, ```/```, ```%``` |  
| Assign | Assignement | ```=``` |  
| Less / Greater | Relational | ```<```, ```>``` |  
| LessEqual / GreaterEqual | Relational | ```<=```, ```>=``` |  
| Equalequal / NotEqual | Equality | ```==```, ```!=``` |  
| AndAnd / OrOr | Logical AND / OR | ```&&```, ```\|\|``` |  
| BitwiseAnd / BitwiseOr | Bitwise AND / OR | ```&```, ```\|``` |  
| Not | Logical not | ```!``` |  
| PlusPlus / MinusMinus | Unary inc/dec (statement form) | ```++```, ```--``` |  
| <b>Punctuation</b> |
| Semicolon | Statement terminator | ```;``` |  
| Colon | Type annotation separator | ```:``` |  
| Dot | Dot | ```.``` |  
| Comma | Separator | ```,``` |  
| LeftParen | Left parenthesis | ```(``` |  
| RightParen | Right parenthesis | ```)``` |  
| LeftBrace | Left curly bracket | ```{``` |
| RightBrace | Right curly bracket | ```}``` |
| LeftBracket  | Left square bracket | ```[``` |  
| RightBracket | Right square bracket | ```]``` |
| <b>Special</b> |  
| LexicalError | Lexical error token (malformed number, string) | ```1.2.3```, unterminated string |
| Unknow | Any unrecognized character (unknown) | ```@```, ```#``` |
| EOF | End of input |  |


## Grammar rules

```ebnf

======================================== Program & block ========================================

program         = { statement } ;

block           = "{" { statement } "}" ;

========================================== Statements ===========================================

statement       = variableDecl
                | constantDecl
                | assignment ";"
                | arrayAssignment ";"
                | expression ";"
                | ifStatement
                | whileStatement
                | forStatement
                | functionDecl
                | returnStatement
                | breakStatement
                | continueStatement
                ;

=================================== Declarations & assignment ===================================

variableDecl    = "set" Identifier [ ":" type ] "=" expression ";" ;
constantDecl    = "bind" Identifier [ ":" type ] "=" expression ";" ;

assignment      = Identifier "=" expression ;

arrayAssignment = Identifier "[" expression "]" "=" expression ;

========================================= Control flow ==========================================

ifStatement     = "if" "(" expression ")" block [ "else" block ] ;

whileStatement  = "while" "(" expression ")" block ;

forStatement    = "for" "(" assignment ";" expression ";" assignment ")" block ;

breakStatement  = "break" ";" ;
continueStatement = "continue" ";" ;

returnStatement = "return" expression ";" ;

=========================================== Functions ===========================================

functionDecl    = "function" Identifier "(" [ paramList ] ")" block ;

paramList       = Identifier { "," Identifier } ;

callExpr        = Identifier "(" [ argList ] ")" ;
argList         = expression { "," expression } ;

========================================== Expressions ==========================================

expression      = logicalOr ;

logicalOr       = logicalAnd { "||" logicalAnd } ;
logicalAnd      = bitwiseOr { "&&" bitwiseOr } ;
bitwiseOr       = bitwiseAnd { "|" bitwiseAnd } ;
bitwiseAnd      = equality { "&" equality } ;
equality        = relational { ( "==" | "!=" ) relational } ;
relational      = additive { ( "<" | ">" | "<=" | ">=" ) additive } ;
additive        = multiplicative { ( "+" | "-" ) multiplicative } ;
multiplicative  = unary { ( "*" | "/" | "%" ) unary } ;

unary           = "!" unary
                | "++" Identifier
                | "--" Identifier
                | primary
                ;

primary         = Integer
                | Float
                | StringLiteral
                | "true" | "false" | "null"
                | Identifier
                | callExpr
                | arrayLiteral
                | indexedRef
                | "(" expression ")"
                ;

arrayLiteral    = "[" [ argList ] "]" ;
indexedRef      = Identifier "[" expression "]" ;

============================================= Types ============================================

type            = "int" | "float" | "bool" | "string" ;

========================================= String escapes ========================================

string          = "\"" { character | escape } "\"" ;

escape          = "\\" ( "n" | "t" | "\"" | "\\" ) ;

character       = ? tout caractère sauf " et \ ? ;
```

## Semantics notes

- ```+``` performs string concatenation if either operand is a string; otherwise it performs numeric addition (int/double).
- ```input()``` reads from standard input and returns int, double, or raw string depending on the content.
- ```print(...)``` writes each argument on its own line.
- Type annotations in ```set```/```bind``` are optional and validated at runtime by the interpreter.
- Prefix ```++id``` / ```--id``` mutate the integer variable and evaluate to the new value.
- Array literals use brackets: ```[expr, expr, ...]```. Indexing: ```name[expr]```.
- Operator precedence (highest to lowest).
- Unary ```++``` / ```--``` are prefix and apply to an Identifier. ```!``` applies to any expression.
- ```Unknow``` is intentionally spelled this way for backward compatibility.

## Futures extensions

- <b>Comments & literals</b> :
  - Line (```// …```) and block (```/* … */```) comments,
  - Hex/bin integers (```0xFF```, ```0b1010```) and numeric separators (```1_000_000```),
  - Char literals (```'a'```), raw/multiline strings, string interpolation (```"Hello, {name}"```).
- <b>Types & type system</b> :
  - Tuples (```(int, string)```), enums, records/structs,
  - Optionals/nullable types (```int?```) and union types (```int | string```),
  - Type aliases and simple generics (```List<int>```),
  - Basic type inference (```set x = 42```; infers ```int```).
- <b>Data structures</b> :
  - Dictionaries/maps (```{ key: value }```) and sets,
  - Slices/ranges (```arr[1..^1]```).
- <b>Functions & control flow</b> :
  - Lambda expressions and higher-order functions,
  - Postfix ```id++``` / ```id--```,
  - ```switch```/```match``` with pattern matching and guards,
  - ```do … while```, ```foreach``` loops,
  - Short function syntax / expression-bodied functions.
- <b>Modules & namespacing</b> :
  - ```import```/```export```, packages, visibility (```public```/```internal```).
- <b>Objects & methods</b> :
  - Method call syntax (```obj.method(args)```), lightweight objects/records,
  - Operator overloading (carefully scoped).
- <b>Error handling</b> :
  - ```try``` / ```catch``` / ```finally```, ```throw```, ```assert```,
  - Result/Either types as an alternative to exceptions.
- <b>Standard library</b> :
  - Collections, math, string, IO, file system, time, random,
  - JSON/CSV utilities.
- <b>Interop</b> :
  - Safe FFI to .NET methods/types with explicit bindings.
- <b>Concurrency</b> :
  - Tasks/futures, ```async```/```await```, channels/actors.
- <b>Compiler/runtime</b> :
  - Source locations & better diagnostics,
  - Optimizations: constant folding, dead-code elimination, tail-call, CFG/SSA,
  - Sandbox limits (CPU/memory/time), deterministic mode.
- <b>Tooling</b> :
  - Formatter, linter, language server (LSP) support,
  - Built-in test runner,
  - Enhanced REPL (history, multiline, ```:load```, ```:type```, ```:ast```).
- <b>Transpiler</b> :
  - Project layout output, partial classes, debug symbols,
  - Sourcemaps back to MILC for stack traces.

