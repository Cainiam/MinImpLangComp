using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using System.Reflection;
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
                        var targetType = GetExpressionType(binaryExpression);
                        GenerateILWithConversion(binaryExpression.Left, il, targetType);
                        GenerateILWithConversion(binaryExpression.Right, il, targetType);
                        EmitBinaryOperator(binaryExpression.Operator, il); // Autre cas 
                    }
                    break;
                case FunctionCall functionCall when functionCall.Name == "print":
                    foreach(var arg in functionCall.Arguments)
                    {
                        GenerateIL(arg, il);
                        var argType = GetExpressionType(arg);
                        var writeLineMethod = typeof(Console).GetMethod(
                            "WriteLine",
                            BindingFlags.Public | BindingFlags.Static,
                            null,
                            new Type[] { argType },
                            null
                        );
                        if(writeLineMethod == null)
                        {
                            il.Emit(OpCodes.Box, argType);
                            writeLineMethod = typeof(Console).GetMethod("WriteLine", new[] { typeof(object) });
                        }
                        il.EmitCall(OpCodes.Call, writeLineMethod!, null);
                    }
                    il.Emit(OpCodes.Ldnull);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expr.GetType().Name}");
            }
        }

        public static void GenerateIL(Statement stmt, ILGenerator il, Dictionary<string, LocalBuilder> locals, HashSet<string> constants)
        {
            switch (stmt)
            {
                case VariableDeclaration variableDeclaration:
                    if (locals.ContainsKey(variableDeclaration.Identifier)) throw new InvalidOperationException($"Variable '{variableDeclaration.Identifier}' already declared");
                    var typeVariable = GetSystemTypeFromAnnotation(variableDeclaration.TypeAnnotation);
                    var local = il.DeclareLocal(typeVariable);
                    locals[variableDeclaration.Identifier] = local;
                    GenerateILWithConversion(variableDeclaration.Expression, il, locals[variableDeclaration.Identifier].LocalType);
                    il.Emit(OpCodes.Stloc, locals[variableDeclaration.Identifier]);
                    break;
                case ConstantDeclaration constantDeclaration:
                    if (locals.ContainsKey(constantDeclaration.Identifier)) throw new InvalidOperationException($"Constant '{constantDeclaration.Identifier}' already declared");
                    var typeConstant = GetSystemTypeFromAnnotation(constantDeclaration.TypeAnnotation);
                    var localBind = il.DeclareLocal(typeConstant, pinned: false);
                    locals[constantDeclaration.Identifier] = localBind;
                    GenerateILWithConversion(constantDeclaration.Expression, il, typeConstant);
                    constants.Add(constantDeclaration.Identifier);
                    il.Emit(OpCodes.Stloc, localBind);
                    break;
                case ExpressionStatement expressionStatement:
                    GenerateIL(expressionStatement.Expression, il);
                    il.Emit(OpCodes.Pop);
                    break;
                case Assignment assignment:
                    if (!locals.TryGetValue(assignment.Identifier, out var localAssign)) throw new InvalidOperationException($"Variable '{assignment.Identifier}' not declared");
                    if (constants.Contains(assignment.Identifier)) throw new InvalidOperationException($"Cannot assign to constant '{assignment.Identifier}'");
                    GenerateILWithConversion(assignment.Expression, il, localAssign.LocalType);
                    il.Emit(OpCodes.Stloc, localAssign);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported statement type: {stmt.GetType().Name}");
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

        private static Type GetExpressionType(Expression expression)
        {
            return expression switch
            {
                IntegerLiteral => typeof(int),
                FloatLiteral => typeof(double),
                StringLiteral => typeof(string),
                BinaryExpression binary => GetExpressionType(binary.Left) == typeof(double) || GetExpressionType(binary.Right) == typeof(double) ? typeof(double) : typeof(int),
                _ => throw new NotSupportedException($"Cannot determine type of expression: {expression.GetType().Name}")
            };
        }

        private static void GenerateILWithConversion(Expression expression, ILGenerator il, Type targetType)
        {
            GenerateIL(expression, il);
            var actualType = GetExpressionType(expression);
            if (actualType == typeof(int) && targetType == typeof(double))
            {
                il.Emit(OpCodes.Conv_R8);
            }
        }

        private static Type GetSystemTypeFromAnnotation(TypeAnnotation? annotation)
        {
            if (annotation == null) throw new InvalidOperationException("Missing type annotation");
            return annotation.TypeToken switch
            {
                TokenType.TypeInt => typeof(int),
                TokenType.TypeFloat => typeof(double),
                TokenType.TypeBool => typeof(bool),
                TokenType.TypeString => typeof(string),
                _ => throw new NotSupportedException($"Unknown type annotation: {annotation.TypeName}")
            };
        }
    }
}
