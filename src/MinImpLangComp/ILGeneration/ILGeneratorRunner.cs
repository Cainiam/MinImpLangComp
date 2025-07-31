using MinImpLangComp.AST;
using System.Reflection;
using System.Reflection.Emit;

namespace MinImpLangComp.ILGeneration
{
    public static class ILGeneratorRunner
    {
        public static object? GenerateAndRunIL(Expression expression)
        {
            // Set-up :
            var method = new DynamicMethod("Eval", typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();

            // Génération du corps de l'expression :
            ILGeneratorUtils.GenerateIL(expression, il);

            // Boxing du type de retour :
            switch(expression)
            {
                case IntegerLiteral:
                    il.Emit(OpCodes.Box, typeof(int));
                    break;
                case FloatLiteral:
                    il.Emit(OpCodes.Box, typeof(double));
                    break;
                case BinaryExpression:
                    if (ContainsFloat(expression)) il.Emit(OpCodes.Box, typeof(double));
                    else il.Emit(OpCodes.Box, typeof(int));
                    break;
            }

            // Retour IL
            il.Emit(OpCodes.Ret);

            // Retour methode
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
