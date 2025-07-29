# Etude sur l'IL et sur "System.Reflection.Emit"

## Auteur :
Jordan Boisdenghien

## Objectif :
Dans le cadre de mon implémentation d'un constructeur minale dans le cadre de mon projet de langage basé sur C#, j'étudie la génération d'IL (intermediate language) de .NET via "System.Reflection.Emit". L'objectif est de comprendre les bases de la génération dynamique des méthodes IL et de produire un premier exemple fonctionnel qui servira de base pour les screens du présent document afin de l'illustrer.

## Qu'est-ce que l' "IL" dans .NET ?
### Définition


### Rôle de l'IL


### Langage de "bas niveau"


## Qu'est-ce que "System.Reflection.Emit" ?
### Présentation de l'espace de noms


### Explication des classes principales :
- AssemblyBuilder :
  
- ModuleBuilder :
  
- TypeBuilder :
  
- MethodBuilder :
  
- ILGenerator :

Autres classes non abordées : ConstructorBuilder, CustomAttributeBuilder, DynamicILInfo,  DynamicMethod,  EnumBuilder, EventBuilder, FieldBuilder, GenericTypeParameterBuilder, LocalBuilder, OpCodes, ParameterBuilder,  PersistedAssemblyBuilder, PropertyBuilder, SignatureHelper.

## Génération d'une méthode IL :
1. Créer un assembly dynamique :
   
2. Ajouter un module :
   
3. Définir une classe :
   
4. Définir une méthode :
   
5. Générer le corps de l'IL :
    
6. Finaliser le type et invoquer la méthode :


## Exemples étudiés :
Nous allons procédés à la génération d'une fonction "Add(int a, int b)" 


## Leçons retenues :
* ?

## Impact sur "MinImpLangComp" :


## Conclusion :

## Quelques sources utilisées :
- https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit?view=net-9.0 (SOURCE PRINCIPALE)
- https://hardyian.medium.com/how-to-use-net-reflection-for-dynamic-code-generation-cb4d1a1a24aa
- https://www.codeproject.com/Articles/121568/Dynamic-Type-Using-Reflection-Emit
