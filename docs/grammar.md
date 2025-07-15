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
| Identifier | Function or variable name | "x", "var1", "lastSum" |  
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
| LexicalError | Invalid token (malformed number) | "1.2.3" |
| Unknow | Any unrecognized character | "@", "#" |
| EOF | End of input |  |


## Grammar rules

program = { statement } ;
statement = expression ";"
| assignement ";" ;
assignement = Identifier "=" expression ;
expression = term { ("*" | "/") term } ;
term = factor { ("*"|"/") factor } ;
factor = number
| identifier
| "(" expression ")" ;
identifier = letter { letter | digit } ;
number = integer | float ;
integer = digit { digit } ;
float (digit { digit } "." [ digit { digit } ] )
| ( "." digit { digit } ) ;
letter = "A".."Z" | "a".."z" ;
digit = "0".."9" ;

## Futures extensions

- Function declerations and calls
- Control flow (if-else, while)
- Boolean expression

