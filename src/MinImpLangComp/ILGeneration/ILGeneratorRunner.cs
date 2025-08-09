using MinImpLangComp.AST;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace MinImpLangComp.ILGeneration
{
    public static class ILGeneratorRunner
    {
        // Runner actuel :
        public static object? GenerateAndRunIL(List<Statement> statements)
        {
            // Set-up :
            var method = new DynamicMethod("Eval", typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();

            // Pour set / bind :
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();

            // Pour return
            string? lastVariableToReturn = null;

            // Génération du corps des instructions :
            for(int i = 0; i < statements.Count; i++)
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
            return del();
        }

        // Runner statement pour envoyer un input
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

        // Méthode précédente maintenue pour les tests
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
            return del();
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
