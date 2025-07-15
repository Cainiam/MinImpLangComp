# MinImpLangComp Grammar specification

## Introduction  

This document defines the grammar ruls and token specification for MinImpLangComp, an imperative langage for interpreter/compiler learning purpose.  

## Author  

Jordan Boisdenghien  

## Tokens  

| Token type | Description | Examples |  
|------------|-------------|----------|  
| Integer | Integer numbers | "10", "0", "1369" |  
| Float | Floating point numbers | "3.14", "0.9", ".5", "97." |  
| StringLiteral | string literal with escapes | "hello"; "line\n" |  
| Identifier | Function or variable name | "x", "var1", "lastSum" |  
| Let | Keyword for declarations | let |
| If | Keyword for conditional | if |
| Else | Keyword for conditional else | else |
| While | Keyword for loop | while |
| For | Keyword for loop | for |
| Plus | Addition operator | "+" |  
| Minus | Subtraction operator | "-" |  
| Multiply | Multplication operator | "*" |  
| Divide | Division operator | "/" |  
| Assign | Assignement operator | "=" |  
| Semicolon | Statement terminator | ";" |  
| Dot | Dot operator or separator | "." |  
| Comma | Comma separator | "," |  
| LeftParen | Left parenthesis | "(" |  
| RightParen | Right parenthesis | ")" |  
| LeftBrace | Left curly bracket | "{" |
| RightBrace | Right curly bracket | "]" |
| LexicalError | Invalid token (malformed number, string) | "1.2.3" |
| Unknow | Any unrecognized character | "@", "#" |
| EOF | End of input |  |


## Grammar rules

```ebnf
program         = { statement } ;

statement       = expression ";"  
                | assignement ";"  
                | ifStatement  
                | whileStatement ;

assignement     = Identifier "=" expression ;

ifStatement     = "if" "(" expression ")" block [ "else" block ] ;

whileStatement  = "while" "(" expression ")" block ;

forStatement    = "for" "(" [ assignement ] ";" [ expression ] ";" [ assignement ] ")" block ;  

block           = "{" { statement } "}" ;  

expression      = term { ("*" | "/") term } ;  

term            = factor { ("*" | "/") factor } ;  

factor          = number  
                | identifier  
                | "(" expression ")" ;  

identifier      = letter { letter | digit } ;  

number          = integer | float ;  

integer         = digit { digit } ;  

float           = (digit { digit } "." [ digit { digit } ] )  
                | ( "." digit { digit } ) ;

string          = "\"" { character | escapeSequence } "\"" ;  

escapeSequence  = "\\" ( "n" | "t" | "t" | "\"" | "\\" ) ;  

letter          = "A".."Z" | "a".."z" ;  
digit           = "0".."9" ;
character       = ? any character except " and \ ? ;  
```

## Futures extensions

- Function declerations and calls
- Control flow (if-else, while)
- Boolean expression

