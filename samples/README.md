# MinImpLangComp - Samples
This folder contains small programs in the MinImpLangComp language. They are used for quick demos and as fixtures for tests.  
> You can run any sample through the CLI. See usage below.  

## Quick start
```bash
# List samples
dotnet run -p src/MinImpLangComp.CLI -- samples

# Run by name (without path)
dotnet run -p src/MinImpLangComp.CLI -- run sample:hello

# Interactively pick one
dotnet run -p src/MinImpLangComp.CLI -- run --pick
```
You can also override the samples directory via the environment variable:
```bash
# Point the CLI to a custom samples directory
export MINIMPLANGCOMP_SAMPLES_DIR="$(pwd)/samples"
```

## Samples index

| File | What it demonstrates | Expected output |
|------------|-------------|----------|  
| hello.milc | Printing a string literal | ```Hello!``` |
| ret.milc | Functions and ```return``` | ```8``` |
| sum.milc | Variables, arithmetic, and ```print``` | ```12``` |

## Details
### ```hello.milc```
```
print("Hello!");
```
### Run
```bash
dotnet run -p src/MinImpLangComp.CLI -- run sample:hello
```
### Expected output
```
Hello!
```

### ```ret.milc```
```
function add(a, b) {
  return a + b;
}

print(add(3, 5));
```
### Run
```bash
dotnet run -p src/MinImpLangComp.CLI -- run sample:ret
```
### Expected output
```
8
```

### ```sum.milc```
```
set a: int = 5;
set b: int = 7;
print(a + b);
```
### Run
```bash
dotnet run -p src/MinImpLangComp.CLI -- run sample:sum
```

### Expected output
```
12
```
