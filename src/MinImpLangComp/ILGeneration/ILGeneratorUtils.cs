using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using System.Reflection;
using System.Reflection.Emit;

namespace MinImpLangComp.ILGeneration
{
    public class LoopContext
    {
        public Label ContinueLabel { get; }
        public Label BreakLabel { get; }

        public LoopContext(Label continueLabel, Label breakLabel)
        {
            ContinueLabel = continueLabel;
            BreakLabel = breakLabel;
        }
    }

    public static class ILGeneratorUtils
    {

        private static readonly HashSet<OperatorType> BooleanOperators = new()
        {
            OperatorType.Equalequal,
            OperatorType.NotEqual,
            OperatorType.Less,
            OperatorType.Greater,
            OperatorType.LessEqual,
            OperatorType.GreaterEqual,
            OperatorType.AndAnd,
            OperatorType.OrOr
        };

        private static readonly HashSet<OperatorType> NumericOperators = new()
        {
            OperatorType.Plus,
            OperatorType.Minus,
            OperatorType.Multiply,
            OperatorType.Divide,
            OperatorType.Modulo,
            OperatorType.BitwiseAnd,
            OperatorType.BitwiseOr
        };

        // Generate IL pour expression
        public static void GenerateIL(Expression expr, ILGenerator il, Dictionary<string, LocalBuilder> locals, HashSet<string> constants)
        {
            switch (expr)
            {
                case IntegerLiteral integerLiteral:
                    il.Emit(OpCodes.Ldc_I4, integerLiteral.Value);
                    break;
                case FloatLiteral floatLiteral:
                    il.Emit(OpCodes.Ldc_R8, floatLiteral.Value);
                    break;
                case BooleanLiteral booleanLiteral:
                    il.Emit(booleanLiteral.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    break;
                case VariableReference variableReference:
                    if (!locals.TryGetValue(variableReference.Name, out var local)) throw new InvalidOperationException($"Variable '{variableReference.Name}' not declared");
                    il.Emit(OpCodes.Ldloc, local);
                    break;
                case BinaryExpression binaryExpression:
                    if (binaryExpression.Operator == OperatorType.AndAnd) // Cas spécifique de l'opérateur logique "&&"
                    {
                        Label falseLabel = il.DefineLabel();
                        Label endLabel = il.DefineLabel();
                        GenerateIL(binaryExpression.Left, il, locals, constants);
                        il.Emit(OpCodes.Brfalse, falseLabel);
                        GenerateIL(binaryExpression.Right, il, locals, constants);
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
                        GenerateIL(binaryExpression.Left, il, locals, constants);
                        il.Emit(OpCodes.Brtrue, trueLabel);
                        GenerateIL(binaryExpression.Right, il, locals, constants);
                        il.Emit(OpCodes.Brtrue, trueLabel);
                        il.Emit(OpCodes.Ldc_I4_0); // false
                        il.Emit(OpCodes.Br, endLabel);
                        il.MarkLabel(trueLabel);
                        il.Emit(OpCodes.Ldc_I4_1); // true
                        il.MarkLabel(endLabel);
                    }
                    else
                    {
                        var targetType = GetExpressionType(binaryExpression, locals);
                        GenerateILWithConversion(binaryExpression.Left, il, targetType, locals, constants);
                        GenerateILWithConversion(binaryExpression.Right, il, targetType, locals, constants);
                        EmitBinaryOperator(binaryExpression.Operator, il); // Autre cas 
                    }
                    break;
                case FunctionCall functionCall when functionCall.Name == "print":
                    foreach(var arg in functionCall.Arguments)
                    {
                        GenerateIL(arg, il, locals, constants);
                        var argType = GetExpressionType(arg, locals);
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

        // Generate IL surchage pour statement avec variable et constante
        public static void GenerateIL(Statement stmt, ILGenerator il, Dictionary<string, LocalBuilder> locals, HashSet<string> constants, LoopContext? context = null)
        {
            switch (stmt)
            {
                case VariableDeclaration variableDeclaration:
                    if (locals.ContainsKey(variableDeclaration.Identifier)) throw new InvalidOperationException($"Variable '{variableDeclaration.Identifier}' already declared");
                    var typeVariable = GetSystemTypeFromAnnotation(variableDeclaration.TypeAnnotation);
                    var local = il.DeclareLocal(typeVariable);
                    locals[variableDeclaration.Identifier] = local;
                    GenerateILWithConversion(variableDeclaration.Expression, il, locals[variableDeclaration.Identifier].LocalType, locals, constants);
                    il.Emit(OpCodes.Stloc, locals[variableDeclaration.Identifier]);
                    break;
                case ConstantDeclaration constantDeclaration:
                    if (locals.ContainsKey(constantDeclaration.Identifier)) throw new InvalidOperationException($"Constant '{constantDeclaration.Identifier}' already declared");
                    var typeConstant = GetSystemTypeFromAnnotation(constantDeclaration.TypeAnnotation);
                    var localBind = il.DeclareLocal(typeConstant, pinned: false);
                    locals[constantDeclaration.Identifier] = localBind;
                    GenerateILWithConversion(constantDeclaration.Expression, il, typeConstant, locals, constants);
                    constants.Add(constantDeclaration.Identifier);
                    il.Emit(OpCodes.Stloc, localBind);
                    break;
                case ExpressionStatement expressionStatement:
                    GenerateIL(expressionStatement.Expression, il, locals, constants);
                    il.Emit(OpCodes.Pop);
                    break;
                case Assignment assignment:
                    if (!locals.TryGetValue(assignment.Identifier, out var localAssign)) throw new InvalidOperationException($"Variable '{assignment.Identifier}' not declared");
                    if (constants.Contains(assignment.Identifier)) throw new InvalidOperationException($"Cannot assign to constant '{assignment.Identifier}'");
                    GenerateILWithConversion(assignment.Expression, il, localAssign.LocalType, locals, constants);
                    il.Emit(OpCodes.Stloc, localAssign);
                    break;
                case IfStatement ifStatement:
                    var conditionType = GetExpressionType(ifStatement.Condition, locals);
                    if (conditionType != typeof(bool)) throw new InvalidOperationException("The condition in if-statement must be of type 'bool'");
                    var elseLabel = il.DefineLabel();
                    var endLabel = il.DefineLabel();
                    GenerateIL(ifStatement.Condition, il, locals, constants);
                    il.Emit(OpCodes.Brfalse, elseLabel);
                    GenerateIL(ifStatement.ThenBranch, il, locals, constants, context);
                    il.Emit(OpCodes.Br, endLabel);
                    il.MarkLabel(elseLabel);
                    if (ifStatement.ElseBranch != null) GenerateIL(ifStatement.ElseBranch, il, locals, constants, context);
                    il.MarkLabel(endLabel);
                    break;
                case WhileStatement whileStatement:
                    var conditionTypeWhile = GetExpressionType(whileStatement.Condition, locals);
                    if (conditionTypeWhile != typeof(bool)) throw new InvalidOperationException("The condition in while-statement must be of type 'bool'");
                    var conditionLabel = il.DefineLabel();
                    var loopEndLabel = il.DefineLabel();
                    var newContext = new LoopContext(conditionLabel, loopEndLabel);
                    il.MarkLabel(conditionLabel);
                    GenerateIL(whileStatement.Condition, il, locals, constants);
                    il.Emit(OpCodes.Brfalse, loopEndLabel);
                    GenerateIL(whileStatement.Body, il, locals, constants, newContext);
                    il.Emit(OpCodes.Br, conditionLabel);
                    il.MarkLabel(loopEndLabel);
                    break;
                case ForStatement forStatement:
                    GenerateIL(forStatement.Initializer, il, locals, constants, context);
                    var forStartLabel = il.DefineLabel();
                    var forEndLabel = il.DefineLabel();
                    var continueLabel = il.DefineLabel();
                    var loopContext = new LoopContext(continueLabel, forEndLabel);
                    il.MarkLabel(forStartLabel);
                    var conditionTypeFor = GetExpressionType(forStatement.Condition, locals);
                    if (conditionTypeFor != typeof(bool)) throw new InvalidOperationException("The condition in for-statement must be of type 'bool'");
                    GenerateILWithConversion(forStatement.Condition, il, typeof(bool), locals, constants);
                    il.Emit(OpCodes.Brfalse, forEndLabel);
                    GenerateIL(forStatement.Body, il, locals, constants, loopContext);
                    il.MarkLabel(continueLabel);
                    GenerateIL(forStatement.Increment, il, locals,  constants, context);
                    il.Emit(OpCodes.Br, forStartLabel);
                    il.MarkLabel(forEndLabel);
                    break;
                case BreakStatement:
                    if (context == null) throw new InvalidOperationException("Cannot use 'break' outside of a loop");
                    il.Emit(OpCodes.Br, context.BreakLabel);
                    break;
                case ContinueStatement:
                    if (context == null) throw new InvalidOperationException("Cannot use 'continue' outside of a loop");
                    il.Emit(OpCodes.Br, context.ContinueLabel);
                    break;
                case Block block:
                    foreach (var inner in block.Statements) GenerateIL(inner, il, locals, constants, context);
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

        private static Type GetExpressionType(Expression expression, Dictionary<string, LocalBuilder> locals)
        {
            return expression switch
            {
                IntegerLiteral => typeof(int),
                FloatLiteral => typeof(double),
                BooleanLiteral => typeof(bool),
                StringLiteral => typeof(string),
                BinaryExpression binary =>
                    BooleanOperators.Contains(binary.Operator)
                        ? typeof(bool)
                            : NumericOperators.Contains(binary.Operator)
                                ? GetExpressionType(binary.Left, locals) == typeof(double) || GetExpressionType(binary.Right, locals) == typeof(double)
                                    ? typeof(double)
                                    : typeof(int)
                                : throw new NotSupportedException($"Cannot determine type of binary operator: {binary.Operator}"),
                VariableReference variableReference =>
                    locals.TryGetValue(variableReference.Name, out var local) ? local.LocalType : throw new InvalidOperationException($"Cannot infer type: variable '{variableReference.Name}' not declared"),
                _ => throw new NotSupportedException($"Cannot determine type of expression: {expression.GetType().Name}")
            };
        }

        private static void GenerateILWithConversion(Expression expression, ILGenerator il, Type targetType, Dictionary<string, LocalBuilder> locals, HashSet<string> constants)
        {
            GenerateIL(expression, il, locals, constants);
            var actualType = GetExpressionType(expression, locals);
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
