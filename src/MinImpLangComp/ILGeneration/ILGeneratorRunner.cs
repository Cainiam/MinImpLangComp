using MinImpLangComp.AST;
using MinImpLangComp.Runtime;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace MinImpLangComp.ILGeneration
{
    public static class ILGeneratorRunner
    {
        // Runner pour CompilerFacade
        public static void RunScript(List<Statement> statements)
        {
            var method = new DynamicMethod("ScriptMain", typeof(void), Type.EmptyTypes, typeof(ILGeneratorRunner).Module, true);
            var il = method.GetILGenerator();
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();
            foreach (var statement in statements) ILGeneratorUtils.GenerateIL(statement, il, locals, constants);
            il.Emit(OpCodes.Ret);
            var action = (Action)method.CreateDelegate(typeof(Action));
            action();
        }

        // Runner statetement pour les test de génération IL :
        public static object? GenerateAndRunIL(List<Statement> statements)
        {
            // Set-up :
            Dictionary<string, MethodInfo>? fnRegistry = null;
            var method = new DynamicMethod("Eval", typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();

            // Pour set / bind :
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();

            // Pour return
            string? lastVariableToReturn = null;

            try
            {
                // On construit les fonctions et reset le buffer d'output
                fnRegistry = ILGeneratorUtils.BuildAndRegisterFunctions(statements);
                RuntimeIO.Clear();

                // Génération du corps des instructions :
                for (int i = 0; i < statements.Count; i++)
                {
                    var statement = statements[i];
                    ILGeneratorUtils.GenerateIL(statement, il, locals, constants);
                    if (i == statements.Count - 1)
                    {
                        lastVariableToReturn = statement switch
                        {
                            VariableDeclaration variableDeclaration => variableDeclaration.Identifier,
                            Assignment assignment => assignment.Identifier,
                            ExpressionStatement expressionStatement when expressionStatement.Expression is VariableReference variableReference => variableReference.Name,
                            _ => null
                        };
                    }
                }

                // Emission retour
                if (lastVariableToReturn != null && locals.TryGetValue(lastVariableToReturn, out var local))
                {
                    il.Emit(OpCodes.Ldloc, local);
                    if (local.LocalType.IsValueType) il.Emit(OpCodes.Box, local.LocalType);
                }
                else il.Emit(OpCodes.Ldnull);

                // Retour Emit
                il.Emit(OpCodes.Ret);

                // Retour méthode :
                var del = (Func<object?>)method.CreateDelegate(typeof(Func<object?>));
                var result = del();

                // Affichage de la sortie console si rien sur la pile
                if(result == null)
                {
                    var output = RuntimeIO.Consume();
                    if (!string.IsNullOrEmpty(output)) Console.Write(output.TrimEnd('\r', '\n'));
                    return output;
                }
                // Sinon, on retourne la valeur
                return result;
            }
            finally
            {
                // Libération du registre
                ILGeneratorUtils.ClearFunctionRegistry();
            }
        }

        // Runner statetement pour les test de génération IL avec simulation d'un input utilisateur
        public static object? GenerateAndRunIL(List<Statement> statements, string? input)
        {
            var originalIn = Console.In;
            try
            {
                if (input != null) Console.SetIn(new StringReader(input));
                return GenerateAndRunIL(statements);
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        // Méthode précédente de runner maintenue pour les tests d'expression
        public static object? GenerateAndRunIL(Expression expression)
        {
            // Set-up :
            var method = new DynamicMethod("Eval", typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();

            // Pour set / bind :
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();

            // Génération du corps de l'expression :
            ILGeneratorUtils.GenerateIL(expression, il, locals, constants);

            // Boxing du type de retour :
            switch(expression)
            {
                case IntegerLiteral:
                    il.Emit(OpCodes.Box, typeof(int));
                    break;
                case FloatLiteral:
                    il.Emit(OpCodes.Box, typeof(double));
                    break;
                case BooleanLiteral:
                    il.Emit(OpCodes.Box, typeof(bool));
                    break;
                case BinaryExpression:
                    if (ContainsFloat(expression)) il.Emit(OpCodes.Box, typeof(double));
                    else il.Emit(OpCodes.Box, typeof(int));
                    break;
            }

            // Retour IL :
            il.Emit(OpCodes.Ret);

            // Retour méthode :
            var del = (Func<object?>)method.CreateDelegate(typeof(Func<object?>));
            var result = del();

            if(result == null)
            {
                var output = RuntimeIO.Consume();
                if (!string.IsNullOrEmpty(output)) Console.Write(output.TrimEnd('\r', '\n'));
            }
            return result;
        }

        public static bool ContainsFloat(Expression expression)
        {
            return expression switch
            {
                FloatLiteral => true,
                BinaryExpression binary => ContainsFloat(binary.Left) || ContainsFloat(binary.Right),
                _ => false
            };
        }
    }
}
