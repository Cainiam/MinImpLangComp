# MinImpLangComp
A pedagogical project that implements a tiny imperative language and its toolchain in C#:
- Lexer & parser
- AST
- Tree-walking interpreter
- IL code generation via System.Reflection.Emit
- A simple C# transpiler
- CLI & REPLs
- Comprehensive unit tests (xUnit)

## Learning objectives:
This project is designed to learn how a language is built and executed:
- Specify a language (grammar, tokens, precedence)
- Implement a lexer and parser
- Design and build an AST
- Write a tree-walking interpreter
- Generate and execute IL at runtime
- Transpile to C# as an alternative backend
- Build a CLI with REPLs and a test suite (xUnit)

## Technologies used:
- <b>Language </b>: C# (.NET 8 LTS)
- <b>IL backend</b>: ```System.Reflection.Emit```
- <b>Testing</b>: xUnit
- <b>CLI</b>: .NET console app 
- <b>IDE</b>: Visual Studio 2022
- <b>Version control</b>: Git

## Project layout :
```ebnf
MinImpLangComp/
├─ docs/                             # Grammar, additional docs
├─ samples/                          # Example .milc programs
├─ src/
│  ├─ MinImpLangComp/                # Core library
│  │  ├─ AST/                        # AST nodes (expressions/statements)
│  │  ├─ Exceptions/                 # Runtime, parsing, control-flow exceptions
│  │  ├─ Facade/                     # CompilerFacade (Parse → IL → Run)
│  │  ├─ ILGeneration/               # ILGeneratorUtils & ILGeneratorRunner
│  │  ├─ Interpreting/               # Tree-walking Interpreter
│  │  ├─ Lexing/                     # Lexer + Token types
│  │  ├─ Parsing/                    # Parser
│  │  ├─ ReplLoop/                   # Interpreter REPL
│  │  └─ Runtime/                    # RuntimeIO (output buffer)
│  └─ MinImpLangComp.CLI/            # Command-line interface
├─ tests/
│  └─ MinImpLangComp.Tests/          # xUnit test suite
├─ global.json                       # pin the .NET SDK
└─ MinImpLangComp.sln
```

## Features :
- <b>Lexer</b>: integers, floats, strings (escapes), identifiers, keywords, and operators.
- <b>Parser</b>: statements, expressions, control flow, functions, arrays.
- <b>Interpreter</b>: evaluates AST with an environment for variables/constants.
- <b>IL Generator</b>: compiles AST to IL and runs it; output captured via a small runtime buffer.
- <b>Transpiler</b>: converts AST to compilable C# code.
- <b>REPLs</b>:
  - IL-backed REPL (compiles & runs each line),
  - Interpreter REPL (tree-walking).
- <b>Type annotations</b> on ```set```/```bind``` (optional): ```int```, ```float```, ```bool```, ```string```.
- <b>Builtins</b>: ```print(...)```, ```input()```.
- <b>Control flow</b>: ```if```/```else```, ```while```, ```for```, ```break```, ```continue```, ```return```.
- <b>Operators</b>: ```+ - * / %```, comparisons, equality, logical ```&&/\|\|```, unary ```!```, unary ```++/--```, bitwise ```&/|```.
- <b>Arrays</b>: literals ```[1, 2, 3]```, access ```a[i]```, assignment ```a[i] = v```.

## Language overview
- <b>Declarations</b>
  - Variable: ```set x = 42;``` or typed ```set x: int = 42;```
  - Constant: ```bind PI: float = 3.14;```
- <b>Functions</b>
```
function sum(a, b) {
  return a + b;
}
```
Call: sum(1, 2);
- <b>Control flow</b>
  - ```if (cond) { ... } else { ... }```
  - ```while (cond) { ... }```
  - ```for (init; cond; step) { ... }```
  - ```break;, continue;, return expr;```
- <b>Arrays</b>
  - ```set a = [1, 2, 3];```
  - ```a[0];```
  - ```a[1] = 99;```
The full grammar is documented in [docs/grammar.md](https://github.com/Cainiam/MinImpLangComp/blob/main/docs/grammar.md).

## Getting started:
### Prerequisites
- NET SDK <b>8.0</b> (LTS)

### Build
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### CLI usage:
Run the CLI project:
```bash
dotnet run --project MinImpLangComp.CLI -- <command> [args]
```
Commands:
- ```run <file.milc>``` — run a program from a file
- ```run -``` — read program from stdin
- ```run --pick``` — interactively pick a sample from /samples
- ```run sample:<name>``` — run a bundled sample by name
- ```samples``` — list available samples
- ```repl``` — IL-backed REPL
- ```repl-interp``` — Interpreter REPL
<b>Environment variable for samples</b>
- ```MINIMPLANGCOMP_SAMPLES_DIR``` — override the samples directory

## Examples:
### Hello world
```
print("Hello!");
```

### Variables, arithmetic, and types
```
set x: int = 10;
set y = 2.5;       // inferred as float (double)
print(x + y);      // 12.5
```

### Control flow
```
if (x > 5) {
  print("big");
} else {
  print("small");
}

set sum: int = 0;
for (set i: int = 0; i < 5; i = i + 1) {
  sum = sum + i;
}
print(sum); // 10
```

### Functions & return
```
function add(a, b) {
  return a + b;
}

print(add(5, 7)); // 12
```

### Arrays
```
set a = [1, 2, 3];
print(a[1]);    // 2
a[1] = 99;
print(a[1]);    // 99
```

### Input
```
set n: int = input();
print(n * 2);
```

## Testing:
The test suite uses xUnit and covers:
- IL generation and execution
- Interpreter evaluation & control flow
- Lexer tokenization
- Parser AST construction
- Transpiler output
- CLI commands
<b>Run</b>:
```
dotnet test
```

## Documentation:
- Language grammar: [docs/grammar.md](https://github.com/Cainiam/MinImpLangComp/blob/main/docs/grammar.md).
- Inline XML doc comments throughout the codebase for public APIs and key internals.

## License:
MIT — see [LICENSE](https://github.com/Cainiam/MinImpLangComp/blob/main/LICENSE)

## Author:
Jordan Boisdenghien.
