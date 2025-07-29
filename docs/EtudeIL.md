# Etude sur l'IL et sur "System.Reflection.Emit"

## Auteur :
Jordan Boisdenghien

## Objectif :
Dans le cadre de mon implémentation d'un constructeur minale dans le cadre de mon projet de langage basé sur C#, j'étudie la génération d'IL (intermediate language) de .NET via "System.Reflection.Emit". L'objectif est de comprendre les bases de la génération dynamique des méthodes IL et de produire un premier exemple fonctionnel qui servira de base pour les screens du présent document afin de l'illustrer.

## Qu'est-ce que l' "IL" dans .NET ?
### Définition
Intermediate Language (IL), aussi Common Intermediate Language (CIL), est le langage intermédiaire dans lequel sont compilés tout les programmes .NET, comme par exemple C#. C'est un langage de bas niveau mais indépendant de la plateforme et qui est destiné à être exécuté par le Common Language Runtime (CLR).

### Rôle de l'IL
* Lors de la compilation du code C#, le code est traduit en IL (et pas directement en langage machine).
* Il est ensuite converti en code natif à l'éxécution par le compilateur JIT ("Just-In-Time") du CLR.
* C'est cela qui permet à un binaire .NET d'être portable entre plusieurs plateformes, tant qu'il y a un runtime .NET de disponible.

### Langage de "bas niveau"
* Plus proche de l'assembleur que du code C#, mais tout de même plus abstrait que le langage machine.
* Utilise une pile pour évaluer les expressions.
* Chaque instruction IL est typée et suit une structure régulière.

## Qu'est-ce que "System.Reflection.Emit" ?
### Présentation de l'espace de noms
System.Reflection.Emit est un espace de nom du framework .NET qui permet de générer dynamiquement du code IL en mémoire.
Il est utilisé pour crééer des assemblies, types, méthodes et instruction IL à l'éxécution sans passer par une "compilation classique".
Cela permet : des générateurs de code dynamiques, des compilateurs personnalisés ou des moteurs de script intégrés.

### Explication des classes principales :
- AssemblyBuilder : Permet de définir un assembly dynamique qui contiendra des modules.
- ModuleBuilder : Représente un module dans l'assembly.
- TypeBuilder : Sert à définir dynamique une classe ou une structure type .NET.
- MethodBuilder : Permet de définir une méthode à l'intérieur d'un type dynamique.
- ILGenerator : Fournit les méthodes permettant d'émettre les instructions IL dans le corps de la méthode.
Autres classes non abordées ici : ConstructorBuilder, CustomAttributeBuilder, DynamicILInfo,  DynamicMethod,  EnumBuilder, EventBuilder, FieldBuilder, GenericTypeParameterBuilder, LocalBuilder, OpCodes, ParameterBuilder,  PersistedAssemblyBuilder, PropertyBuilder, SignatureHelper.

## Génération d'une méthode IL via un exemple étudié :
Nous allons procédés à la génération d'une fonction "Add(int a, int b)"  
<img width="702" height="474" alt="image" src="https://github.com/user-attachments/assets/6a87ade5-5429-4366-9639-1671b88d0c95" />  
  
1. Préparation du contexte dynamique :
<img width="707" height="75" alt="image" src="https://github.com/user-attachments/assets/33cf3b30-c16a-48a2-a18c-6e63d95ec860" />  

Création d'un assembly dynamique  
<img width="395" height="23" alt="image" src="https://github.com/user-attachments/assets/4286e9c0-583a-4fbc-9ce6-6708c49a0a39" />  
* On nomme notre assembly dynamique.
* Pas de génération de dll, ici on identifie juste l'assembly dans l'environnement éxécutif.
  
Déclaration de l'assembly en mémoire  
<img width="714" height="33" alt="image" src="https://github.com/user-attachments/assets/6b3fa11a-241f-47a4-85f5-12c5c3ac8d83" />  
* On crée un assembly dynamique (un conteneur de code) qui existe qu'en mémoire.
* Le ".Run" signifie que l'assembly est éxécutable mais pas sauvegardé.

Création d'un module dans l'assembly  
<img width="484" height="18" alt="image" src="https://github.com/user-attachments/assets/e2e7e20a-dc4b-45dc-a396-97a536ce8ab6" />  
* Module = fichier logique à l'intérieur d'un assembly.
* On créé un seul module, ici, qui contiendra la classe et la méthode dynamique.

Visuellement l'objectif est d'atteindre ceci lors que l'on déclare la méthode Add:  
```
Assembly (DynamicILAssembly)  
└── Module ("MainModule")  
    └── Type ("MathOperations")  
        └── Method ("Add(int, int)")  
```  

2. Déclaration d'un type (classe dynamique) :  
<img width="609" height="34" alt="image" src="https://github.com/user-attachments/assets/1d2ee836-bba5-4c64-866d-ca7a9a0f551f" />  

* On créé une classe dynamique nommée Mathperation marquée comme public pour pouvoir être utilisée librement.
* Cette classe contiendra la méthode Add que l'on va créé .

3. Déclaration de la méthode Add :  
<img width="636" height="130" alt="image" src="https://github.com/user-attachments/assets/21f60560-a6ce-47b9-9d7c-f9f1fa97abc2" />  

* On créé un méthode "Add" avec les caractéristique suivante:
  * public = accessible depuis l'extérieur.
  * static = appelable sans instance de classe.
  * typeof(int) = retourne une valeur de type int (entier).
  * new[] {typeof(int), typeof(int)} = prend deux paramètres de type int (entier).
* Cela revient à public static int Add(int a, int b)

4. Génération du corps IL de la méthode Add :  
<img width="459" height="98" alt="image" src="https://github.com/user-attachments/assets/c9a54d9d-65f3-474d-95a6-c2333ba63953" />  

* GetILGenerator() permet d'écrire des instructions IL.
* En IL, la pile est utilisée pour passer des arguments, exécuter les opérations puis retourner une valeur.
* On écrit les instructions suivante dans notre code :
  * Ldarg_0 -> on charge le premier argument (index 0) de la méthode, le "a", sur la pile.
  * LDarg_1 -> on charge le second argument (index 1) de la méthode, le "b", sur la pile. Ici, notre pile est donc [a, b].
  * Add -> réalise l'addition des 2 valeurs présente en haut de la pile, elle devient [a + b].
  * Ret -> retourne la valeur au sommet de la pile. Cela revient à "dépilé" la pile, qui est [a + b]. Cela retourne donc a + b.

5. Création finale du type (compilation en mémoire) :  
<img width="391" height="37" alt="image" src="https://github.com/user-attachments/assets/1c6d0585-1008-4a19-bcbc-71c05b95cf0d" />  

* On appelle CreateType() une fois le type et ses méthodes définies pour le compiler et le rendre utilisable.

6. Invocation de la méthode dynamique via Reflection :  
<img width="650" height="81" alt="image" src="https://github.com/user-attachments/assets/82826742-63b9-490c-b516-2d24fe556b22" />  

* GetMethod permet de récupérer une méthode via son nom.
* Invoke :
  * Premier argument : null, car méthode static donc pas besoin d'instance.
  * Second argument : new object[] { 3, 4 }, ce sont les arguments passé à la méthode.
* Le résultat "7" est affiché dans la console (voir screenshot plus bas).

Cela génère le code suivant en C# de façon dynamique en mémoire via Reflection.Emit :  
```csharp
public static int Add(int a, int b)
{
    return a + b;
}
```

Voici le retour de notre code en console :  
<img width="323" height="80" alt="image" src="https://github.com/user-attachments/assets/c44cf378-5782-42d9-ba84-4374159dad69" />  

## Leçons retenues :
* L'IL est un langage de bas niveau exigeant mais "puissant" : il offre un contrôle total sur le comportement du langage mais impose une rigueur dans la gestion de la pile et des types.
* System.Reflection.Emit est complexe mais adapté : bien que sa syntaxe puisse être peu intuitive, il est parfaitement adapté à la génération dynamique de code dans des contextes comme un langage embarqué ou une VM personnalisée.
* Un modèle AST défini clair est indispensabe pour commencer : avant de générer l'IL, il est essentiel d'avoir établi un arbre syntaxique bien structuré et représentatif. Cela permet d'associer chaque noeud à une séquence IL bien définie et d'éviter les erreurs de logique.

## Impact sur "MinImpLangComp" :
L'apprentisage de System.Reflection.Emit ouvre de nouvelles perspectives pour le projet de "MinImpLangComp" :  
* Possible de transpiler directement par l'IL, plutôt que d'interpréter.
* Permet à terme d'exécuter les programmes en mode natif CLR avec des performances supérieurs à l'interpréteur.
* Pourra servir de base pour générer des exécutables .NET autonome à partir du langage.
* Pourra intégrer, à terme, un générateur d'IL ciblant l'env .NET tout en gardant une compatibilité complète avec l'AST et les règles déjà définies.

## Conclusion :
Cette étude m'aura permis de :
* Comprendre le fonctionnement d'IL .NET de System.Reflection.Emit.
* Réaliser un exemple de génération dynamique de méthode IL.
* Identifier les prérequis et les pièges éventuels de la génération IL.


## Quelques sources utilisées :
- https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit?view=net-9.0
- https://learn.microsoft.com/en-us/dotnet/standard/managed-code
- https://en.wikipedia.org/wiki/Common_Intermediate_Language
- https://hardyian.medium.com/how-to-use-net-reflection-for-dynamic-code-generation-cb4d1a1a24aa
- https://www.codeproject.com/Articles/121568/Dynamic-Type-Using-Reflection-Emit
