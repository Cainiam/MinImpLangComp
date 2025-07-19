# MinImpLangComp

## Description :
Projet à but pédagogique visant à créer un mini-langage impératif et son compilateur / interpréteur en C#.

## Auteur :
- Jordan Boisdenghien, diplômé d'un bachelier en informatique de gestion.

## Objectifs globaux:
Projet complet pour apprendre :
- Conception d'un langage (déf. grammaire et syntaxe),
- Développement d'un lexer et parser,
- Construction d'un AST,
- Implémentation d'un interpréteur,
- Génération de code IL pour .NET.

## Technologies utilisées :
- Langage : C# (.NET 8 LTS)
- IDE Visual Studio 2022
- Versioning via Git

## Structure du projet :
|  Program.cs  
|  Lexer.cs  
|  Parser.cs  
|  AST /  
||----- Node.cs  
||----- Expression.cs  
||----- Statement.cs  
||----- Block.cs  
||----- ...  
|  Interpreter.cs  
|  ILGenerator.cs  
|_ Token.cs  

## Fonctionnalités principales :
- Lexer : découpage du code en token.
- Parser : construction de l'AST.
- Interpréteur : exécution des instructions.
- Transpileur : génèration du code c# à partir du langage.
- Générateur IL : compilation vers l'assembly .NET.
- REPL : interface interactive en ligne de commande.

## Exemple de code :
[TODO]

## Licence :
Ce projet est publié sous licence MIT (voir le fichier LICENSE).
