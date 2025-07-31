using MinImpLangComp.AST;
using System.Reflection.Emit;

namespace MinImpLangComp.ILGeneration
{
    public static class ILGeneratorUtils
    {
        public static void GenerateIL(Expression expr, ILGenerator il)
        {
            switch (expr)
            {
                case IntegerLiteral integerLiteral:
                    il.Emit(OpCodes.Ldc_I4, integerLiteral.Value);
                    break;
                case FloatLiteral floatLiteral:
                    il.Emit(OpCodes.Ldc_R8, floatLiteral.Value);
                    break;
                case BinaryExpression binaryExpression:
                    if (binaryExpression.Operator == OperatorType.AndAnd) // Cas spécifique de l'opérateur logique "&&"
                    {
                        Label falseLabel = il.DefineLabel();
                        Label endLabel = il.DefineLabel();
                        GenerateIL(binaryExpression.Left, il);
                        il.Emit(OpCodes.Brfalse, falseLabel);
                        GenerateIL(binaryExpression.Right, il);
                        il.Emit(OpCodes.Brfalse, falseLabel);
                        il.Emit(OpCodes.Ldc_I4_1); // true
                        il.Emit(OpCodes.Br, endLabel);
                        il.MarkLabel(falseLabel);
                        il.Emit(OpCodes.Ldc_I4_0); // false
                        il.MarkLabel(endLabel);
                    }
                    else if (binaryExpression.Operator == OperatorType.OrOr) // Cas spécifique de l'opérateur logique "||"
                    {
                        Label trueLabel = il.DefineLabel();
                        Label endLabel = il.DefineLabel();
                        GenerateIL(binaryExpression.Left, il);
                        il.Emit(OpCodes.Brtrue, trueLabel);
                        GenerateIL(binaryExpression.Right, il);
                        il.Emit(OpCodes.Brtrue, trueLabel);
                        il.Emit(OpCodes.Ldc_I4_0); // false
                        il.Emit(OpCodes.Br, endLabel);
                        il.MarkLabel(trueLabel);
                        il.Emit(OpCodes.Ldc_I4_1); // true
                        il.MarkLabel(endLabel);
                    }
                    else
                    {
                        GenerateIL(binaryExpression.Left, il);
                        GenerateIL(binaryExpression.Right, il);
                        EmitBinaryOperator(binaryExpression.Operator, il); // Autre cas d'opérateur
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expr.GetType().Name}");
            }
        }

        private static void EmitBinaryOperator(OperatorType oper, ILGenerator il)
        {
            switch (oper)
            {
                case OperatorType.Plus:
                    il.Emit(OpCodes.Add);
                    break;
                case OperatorType.Minus:
                    il.Emit(OpCodes.Sub);
                    break;
                case OperatorType.Multiply:
                    il.Emit(OpCodes.Mul);
                    break;
                case OperatorType.Divide:
                    il.Emit(OpCodes.Div);
                    break;
                case OperatorType.Modulo:
                    il.Emit(OpCodes.Rem);
                    break;
                case OperatorType.Equalequal:
                    il.Emit(OpCodes.Ceq);
                    break;
                case OperatorType.NotEqual:
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case OperatorType.Less:
                    il.Emit(OpCodes.Clt);
                    break;
                case OperatorType.Greater:
                    il.Emit(OpCodes.Cgt);
                    break;
                case OperatorType.LessEqual:
                    il.Emit(OpCodes.Cgt);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case OperatorType.GreaterEqual:
                    il.Emit(OpCodes.Clt);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case OperatorType.BitwiseAnd:
                    il.Emit(OpCodes.And);
                    break;
                case OperatorType.BitwiseOr:
                    il.Emit(OpCodes.Or);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported binary operator {oper}");
            }
        }
    }
}
