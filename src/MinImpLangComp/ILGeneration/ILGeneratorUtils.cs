using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.Runtime;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

namespace MinImpLangComp.ILGeneration
{
    /// <summary>
    /// Holds loop labels used during IL emission for <c>break</c>/<c>continue</c>.
    /// </summary>
    public class LoopContext
    {
        /// <summary>
        /// Target label for <c>continue</c>.
        /// </summary>
        public Label ContinueLabel { get; }

        /// <summary>
        /// Target label for <c>break</c>.
        /// </summary>
        public Label BreakLabel { get; }

        /// <summary>
        /// Creates a new loop context with the given labels.
        /// </summary>
        /// <param name="continueLabel">Label for continue.</param>
        /// <param name="breakLabel">Label for break.</param>
        public LoopContext(Label continueLabel, Label breakLabel)
        {
            ContinueLabel = continueLabel;
            BreakLabel = breakLabel;
        }
    }

    /// <summary>
    /// Utilities to emit IL for MinImp AST nodes and to build dynamic function stubs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Maintains a thread-local function registry so multiple runs/tests do not clash.
    /// </para>
    /// </remarks>
    public static class ILGeneratorUtils
    {

        // Thread-local registry for compiled functions
        private static readonly ThreadLocal<Dictionary<string, MethodInfo>?> _functionRegistry = new ThreadLocal<Dictionary<string, MethodInfo>?>(() => null);

        /// <summary>
        /// Clears the thread-local function registry.
        /// </summary>
        public static void ClearFunctionRegistry() => _functionRegistry.Value = null;

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

        /// <summary>
        /// Emits IL fon an <see cref="Expression"/> and leaves its value on the evaluation stack.
        /// </summary>
        /// <param name="expr">Expression to be IL emitted.</param>
        /// <param name="il">ILGenerator.</param>
        /// <param name="locals">Variables.</param>
        /// <param name="constants">Constants.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
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
                case StringLiteral stringLiteral:
                    il.Emit(OpCodes.Ldstr, stringLiteral.Value);
                    break;
                case VariableReference variableReference:
                    if (!locals.TryGetValue(variableReference.Name, out var local)) throw new InvalidOperationException($"Variable '{variableReference.Name}' not declared");
                    il.Emit(OpCodes.Ldloc, local);
                    break;
                case BinaryExpression binaryExpression:
                    if (binaryExpression.Operator == OperatorType.AndAnd)
                    {
                        // Short-circuit &&
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
                    else if (binaryExpression.Operator == OperatorType.OrOr)
                    {
                        // Short-circuit ||
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

                        // Special-case: string concatenation with +
                        if(binaryExpression.Operator == OperatorType.Plus && targetType == typeof(string))
                        {
                            GenerateIL(binaryExpression.Left, il, locals, constants);
                            var lt = GetExpressionType(binaryExpression.Left, locals);
                            if (lt.IsValueType) il.Emit(OpCodes.Box, lt);
                            GenerateIL(binaryExpression.Right, il, locals, constants);
                            var rt = GetExpressionType(binaryExpression.Right, locals);
                            if (rt.IsValueType) il.Emit(OpCodes.Box, rt);
                            var concatObj = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) })!;
                            il.EmitCall(OpCodes.Call, concatObj, null);
                        }
                        else
                        {
                            GenerateILWithConversion(binaryExpression.Left, il, targetType, locals, constants);
                            GenerateILWithConversion(binaryExpression.Right, il, targetType, locals, constants);
                            EmitBinaryOperator(binaryExpression.Operator, il); // Autre cas 
                        }
                    }
                    break;
                case FunctionCall functionCall when functionCall.Name == "print":
                    foreach(var arg in functionCall.Arguments)
                    {
                        GenerateIL(arg, il, locals, constants);
                        var argType = GetExpressionType(arg, locals);
                        if (argType.IsValueType) il.Emit(OpCodes.Box, argType);
                        var printMethod = typeof(RuntimeIO).GetMethod(nameof(RuntimeIO.Print), new[] { typeof(object) })!;
                        il.EmitCall(OpCodes.Call, printMethod, null);
                    }
                    il.Emit(OpCodes.Ldnull);
                    break;
                case FunctionCall functionCall when functionCall.Name == "input":
                    if (functionCall.Arguments.Count != 0) throw new InvalidOperationException("input() does not accept any arguments");
                    il.EmitCall(OpCodes.Call, typeof(Console).GetMethod("ReadLine", Type.EmptyTypes)!, null);
                    break;
                case FunctionCall functionCall:
                    // First, check local delegate
                    if(locals.TryGetValue(functionCall.Name, out var functionLocal))
                    {
                        var delegateType = functionLocal.LocalType;
                        if (!typeof(Delegate).IsAssignableFrom(delegateType)) throw new InvalidOperationException($"Local '{functionCall.Name}' is not callable delegate");
                        var invokeMethod = delegateType.GetMethod("Invoke");
                        if (invokeMethod == null) throw new InvalidOperationException($"Cannot find Invole on delegate type '{delegateType.Name}'");
                        var parameters = invokeMethod.GetParameters();
                        if (parameters.Length != functionCall.Arguments.Count) throw new InvalidOperationException($"Function '{functionCall.Name}' expects {parameters.Length} argument but {functionCall.Arguments.Count} were provided");
                        il.Emit(OpCodes.Ldloc, functionLocal);
                        for (int i = 0; i < parameters.Length; i++) GenerateILWithConversion(functionCall.Arguments[i], il, parameters[i].ParameterType, locals, constants);
                        il.EmitCall(OpCodes.Callvirt, invokeMethod, null);
                        if (invokeMethod.ReturnType == typeof(void)) il.Emit(OpCodes.Ldnull);
                        break;
                    }
                    // Then check compiled functions
                    if(_functionRegistry.Value != null && _functionRegistry.Value.TryGetValue(functionCall.Name, out var targetMethod))
                    {
                        var paramInfos = targetMethod.GetParameters();
                        if (paramInfos.Length != functionCall.Arguments.Count) throw new InvalidOperationException($"Function '{functionCall.Name}' expects {paramInfos.Length} argument but {functionCall.Arguments.Count} were provided");
                        for (int i = 0; i < paramInfos.Length; i++) GenerateILWithConversion(functionCall.Arguments[i], il, paramInfos[i].ParameterType, locals, constants);
                        il.EmitCall(OpCodes.Call, targetMethod, null);
                        if (targetMethod.ReturnType == typeof(void)) il.Emit(OpCodes.Ldnull);
                        break;
                    }
                    throw new NotSupportedException($"Function '{functionCall.Name}' not supported: expected a delegate in locals or compiled function");
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expr.GetType().Name}");
            }
        }

        /// <summary>
        /// Emits IL for a <see cref="Statement"/>.
        /// </summary>
        /// <param name="stmt">Statement to be IL emits.</param>
        /// <param name="il">IlGenerator.</param>
        /// <param name="locals">Variables.</param>
        /// <param name="constants">Constants.</param>
        /// <param name="context">Context.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
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
                    GenerateIL(forStatement.Increment, il, locals,  constants, loopContext);
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
                case FunctionDeclaration:
                    // Handled by BuildAndRegisterFunctions; no body emission here.
                    break;
                case Block block:
                    foreach (var inner in block.Statements) GenerateIL(inner, il, locals, constants, context);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported statement type: {stmt.GetType().Name}");
            }
        }

        /// <summary>
        /// Emits the IL instruction corresponding to a binary operator (assuming operands are already on stack).
        /// </summary>
        /// <param name="oper">Operator to be IL emited.</param>
        /// <param name="il">ILGenerator.</param>
        /// <exception cref="NotSupportedException">Throws if binary operator is not supported.</exception>
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

        /// <summary>
        /// Returns the static type of an expression for IL emission purposes.
        /// </summary>
        /// <param name="expression">Expression to be checked.</param>
        /// <param name="locals">Variables.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws if problem with binary operator.</exception>
        /// <exception cref="NotSupportedException">Throws if problem with function or expression.</exception>
        private static Type GetExpressionType(Expression expression, Dictionary<string, LocalBuilder> locals)
        {
            switch(expression)
            {
                case IntegerLiteral:
                    return typeof(int);
                case FloatLiteral:
                    return typeof(double);
                case BooleanLiteral:
                    return typeof(bool);
                case StringLiteral:
                    return typeof(string);
                case VariableReference variableReference:
                    if (locals.TryGetValue(variableReference.Name, out var local)) return local.LocalType;
                    throw new InvalidOperationException($"Cannot infer type: variable '{variableReference.Name}' not declared");
                case BinaryExpression binaryExpression:
                    if (BooleanOperators.Contains(binaryExpression.Operator)) return typeof(bool);
                    var leftType = GetExpressionType(binaryExpression.Left, locals);
                    var rightType = GetExpressionType(binaryExpression.Right, locals);
                    if (binaryExpression.Operator == OperatorType.Plus && (leftType == typeof(string) || rightType == typeof(string))) return typeof(string);
                    if(NumericOperators.Contains(binaryExpression.Operator))
                        return (leftType == typeof(double) || rightType == typeof (double)) ? typeof(double) : typeof(int);
                    throw new NotSupportedException($"Cannot determine type of binary operator: {binaryExpression.Operator}");
                case FunctionCall functionCall:
                    if(functionCall.Name == "input") return typeof(string);
                    if (functionCall.Name == "print") return typeof(void);
                    if(locals.TryGetValue(functionCall.Name, out var functionLocal))
                    {
                        var delegateType = functionLocal.LocalType;
                        if(typeof(Delegate).IsAssignableFrom(delegateType))
                        {
                            var invoke = delegateType.GetMethod("Invoke");
                            if (invoke != null) return invoke.ReturnType == typeof(void) ? typeof(void) : invoke.ReturnType;
                        }
                    }
                    if(_functionRegistry.Value != null && _functionRegistry.Value.TryGetValue(functionCall.Name, out var methodInfo)) return methodInfo.ReturnType == typeof (void) ? typeof(void) : methodInfo.ReturnType;
                    throw new NotSupportedException($"Cannot determine type of function: '{functionCall.Name}'");
                default:
                    throw new NotSupportedException($"Cannot determine type of expression: {expression.GetType().Name}");
            }
        }

        /// <summary>
        /// Emits an expression and converts the value to <paramref name="targetType"/> when necessary.
        /// </summary>
        /// <param name="expression">Expression to be IL emited.</param>
        /// <param name="il">ILGenerator.</param>
        /// <param name="targetType">Type for the conversion.</param>
        /// <param name="locals">Variables.</param>
        /// <param name="constants">Constants.</param>
        private static void GenerateILWithConversion(Expression expression, ILGenerator il, Type targetType, Dictionary<string, LocalBuilder> locals, HashSet<string> constants)
        {
            GenerateIL(expression, il, locals, constants);
            var actualType = GetExpressionType(expression, locals);

            if (actualType == typeof(string))
            {
                if (targetType == typeof(int))
                {
                    var parseInt = typeof(int).GetMethod("Parse", new[] { typeof(string) })!;
                    il.EmitCall(OpCodes.Call, parseInt, null);
                    return;
                }

                if (targetType == typeof(double)) 
                {
                    var parseDouble = typeof(double).GetMethod("Parse", new[] { typeof(string) })!;
                    il.EmitCall(OpCodes.Call, parseDouble, null);
                    return;
                }

                if (targetType == typeof(bool))
                {
                    var trimMethod = typeof(string).GetMethod("Trim", Type.EmptyTypes)!;
                    il.EmitCall(OpCodes.Callvirt, trimMethod, null);
                    var parseBool = typeof(bool).GetMethod("Parse", new[] { typeof(string) })!;
                    il.EmitCall(OpCodes.Call, parseBool, null);
                    return;
                }
                // String -> string : no conversion
            }

            if (actualType == typeof(int) && targetType == typeof(double))
            {
                il.Emit(OpCodes.Conv_R8);
            }
        }

        /// <summary>
        /// Builds dynamic methods for all top-level function declarations and registers them for calls.
        /// </summary>
        /// <param name="statements">List of statements (functions) to be build.</param>
        /// <returns></returns>
        public static Dictionary<string, MethodInfo> BuildAndRegisterFunctions(List<Statement> statements)
        {
            // Prepare assembly/module/type
            var asmName = new AssemblyName("MinImpLangComp.Dynamic_" + Guid.NewGuid().ToString("N"));
            var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var module = asm.DefineDynamicModule(asmName.Name!);
            var typeBuilder = module.DefineType("Script" + Guid.NewGuid().ToString("N"), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            // Collect function declarations
            var functionDecls = new List<FunctionDeclaration>();
            foreach (var s in statements)
                if (s is FunctionDeclaration fd) functionDecls.Add(fd);
            var mbByName = new Dictionary<string, MethodBuilder>();
            var sigByName = new Dictionary<string, (Type Ret, Type[] Params, Dictionary<string, Type> ParamMap)>();

            // Signatures (params + return) inferred from bodies
            foreach(var fd in functionDecls)
            {
                var paramTypes = InferParameterTypes(fd);
                var paramMap = new Dictionary<string, Type>();
                for(int i = 0; i < fd.Parameters.Count; i++) paramMap[fd.Parameters[i]] = paramTypes[i];
                var retType = InferReturnType(fd, paramMap);
                var mb = typeBuilder.DefineMethod(fd.Name, MethodAttributes.Public | MethodAttributes.Static, retType, paramTypes);
                for (int i = 0; i < fd.Parameters.Count; i++) mb.DefineParameter(i + 1, ParameterAttributes.None, fd.Parameters[i]);
                mbByName[fd.Name] = mb;
                sigByName[fd.Name] = (retType, paramTypes, paramMap);
            }

            // Bodies
            foreach(var fd in functionDecls)
            {
                var (retType, paramTypes, paramMap) = sigByName[fd.Name];
                EmitFunctionBody(mbByName[fd.Name], fd, retType, paramTypes, paramMap);
            }

            // Build and register
            var builtType = typeBuilder.CreateType()!;
            var registry = new Dictionary<string, MethodInfo>();
            foreach (var fd in functionDecls)
            {
                var mi = builtType.GetMethod(fd.Name, BindingFlags.Public | BindingFlags.Static)!;
                registry[fd.Name] = mi;
            }
            _functionRegistry.Value = registry;
            return registry;
        }

        /// <summary>
        /// Naively infers parameter types from function usage (int by default).
        /// </summary>
        /// <param name="fd">Function decleration to be infered.</param>
        /// <returns>Types infered.</returns>
        private static Type[] InferParameterTypes(FunctionDeclaration fd)
        {
            var types = new Type[fd.Parameters.Count];
            for (int i = 0; i < types.Length; i++) types[i] = typeof(int);
            foreach (var stmt in fd.Body.Statements) InferParamTypesFromStatement(stmt, fd.Parameters, types);
            return types;
        }

        /// <summary>
        /// Walks statements to refine parameter types.
        /// </summary>
        /// <param name="stmt">Statement to be walked.</param>
        /// <param name="paramNames">List of parameters name.</param>
        /// <param name="types">List of parameters type.</param>
        private static void InferParamTypesFromStatement(Statement stmt, List<string> paramNames, Type[] types)
        {
            switch (stmt)
            {
                case ExpressionStatement expressionStatement:
                    InferParamTypesFromExpression(expressionStatement.Expression, paramNames, types);
                    break;
                case Assignment assignment:
                    InferParamTypesFromExpression(assignment.Expression, paramNames, types);
                    break;
                case IfStatement ifStatement:
                    InferParamTypesFromExpression(ifStatement.Condition, paramNames, types);
                    InferParamTypesFromStatement(ifStatement.ThenBranch, paramNames, types);
                    if(ifStatement.ElseBranch != null) InferParamTypesFromStatement(ifStatement.ElseBranch, paramNames, types);
                    break;
                case WhileStatement whileStatement:
                    InferParamTypesFromExpression(whileStatement.Condition, paramNames, types);
                    InferParamTypesFromStatement(whileStatement.Body, paramNames, types);
                    break;
                case ForStatement forStatement:
                    InferParamTypesFromStatement(forStatement.Initializer, paramNames, types);
                    InferParamTypesFromExpression(forStatement.Condition, paramNames, types);
                    InferParamTypesFromStatement(forStatement.Increment, paramNames, types);
                    InferParamTypesFromStatement(forStatement.Body, paramNames, types);
                    break;
                case ReturnStatement returnStatement:
                    InferParamTypesFromExpression(returnStatement.Expression, paramNames, types);
                    break;
                case VariableDeclaration variableDeclaration:
                    InferParamTypesFromExpression(variableDeclaration.Expression, paramNames, types);
                    break;
                case ConstantDeclaration constantDeclaration:
                    InferParamTypesFromExpression(constantDeclaration.Expression, paramNames, types);
                    break;
            }
        }

        /// <summary>
        /// Walks expression to refine parameter types.
        /// </summary>
        /// <param name="expr">Expression to be walked.</param>
        /// <param name="paramNames">List of parameters name.</param>
        /// <param name="types">Lost of paramters type.</param>
        private static void InferParamTypesFromExpression(Expression expr, List<string> paramNames, Type[] types)
        {
            switch(expr)
            {
                case VariableReference variableReference:
                    break;
                case BinaryExpression binaryExpression:
                    if (binaryExpression.Operator == OperatorType.Plus && (ContainsString(binaryExpression.Left) || ContainsString(binaryExpression.Right))) MarkParamsAs(paramNames, types, binaryExpression, typeof(string));
                    else if (IsNumericOperator(binaryExpression.Operator) && (ContainsFloat(binaryExpression.Left) || ContainsFloat(binaryExpression.Right))) MarkParamsAs(paramNames, types, binaryExpression, typeof(double), preferUpgrade: true);
                    InferParamTypesFromExpression(binaryExpression.Left, paramNames, types);
                    InferParamTypesFromExpression(binaryExpression.Right, paramNames, types);
                    break;
                case FunctionCall functionCall:
                    if(functionCall.Name == "print")
                    {
                        foreach(var arg in functionCall.Arguments)
                        {
                            if (arg is VariableReference variableReference)
                            {
                                int idx = paramNames.IndexOf(variableReference.Name);
                                if (idx >= 0)
                                {
                                    types[idx] = typeof(string);
                                }
                            }
                            else
                            {
                                InferParamTypesFromExpression(arg, paramNames, types);
                            }
                        }
                    }
                    else
                    {
                        foreach (var a in functionCall.Arguments) InferParamTypesFromExpression(a, paramNames, types);
                    }
                break;
            }
        }

        private static bool IsNumericOperator(OperatorType op)
        {
            return op == OperatorType.Plus || op == OperatorType.Minus || op == OperatorType.Multiply || op == OperatorType.Divide || op == OperatorType.Modulo || op == OperatorType.BitwiseAnd || op == OperatorType.BitwiseOr;
        }

        private static bool ContainsFloat(Expression e)
        {
            return e switch
            {
                FloatLiteral => true,
                BinaryExpression be => ContainsFloat(be.Left) || ContainsFloat(be.Right),
                FunctionCall fc => fc.Arguments.Exists(ContainsFloat),
                _ => false
            };
        }

        private static bool ContainsString(Expression e)
        {
            return e switch
            {
                StringLiteral => true,
                BinaryExpression be => ContainsString(be.Left) || ContainsString(be.Right),
                FunctionCall fc => fc.Arguments.Exists(ContainsString),
                _ => false
            };
        }

        private static void MarkParamsAs(List<string> paramNames, Type[] types, Expression e, Type t, bool preferUpgrade = false)
        {
            void Touch(Expression x)
            {
                if (x is VariableReference variableReference)
                {
                    int idx = paramNames.IndexOf(variableReference.Name);
                    if(idx >= 0)
                    {
                        if (preferUpgrade)
                        {
                            if (types[idx] == typeof(int)) types[idx] = t;
                        }
                        else
                        {
                            types[idx] = t;
                        }
                    }
                }
                else if (x is BinaryExpression binaryExpression)
                {
                    Touch(binaryExpression.Left);
                    Touch(binaryExpression.Right);
                }
                else if(x is FunctionCall functionCall)
                {
                    foreach (var a in functionCall.Arguments) Touch(a);
                }
            }
            Touch(e);
        }

        /// <summary>
        /// Infers the return type of a function from its first return statement (defaults to void).
        /// </summary>
        /// <param name="fd">Function declaration to be infered.</param>
        /// <param name="paramMap">Map of parameters type.</param>
        /// <returns></returns>
        private static Type InferReturnType(FunctionDeclaration fd, Dictionary<string, Type> paramMap)
        {
            foreach(var st in fd.Body.Statements)
            { 
                if(st is ReturnStatement rs)
                {
                    return InferExpressionTypeForFunction(rs.Expression, paramMap);
                }
            }
            return typeof(void);
        }

        /// <summary>
        /// Infers the type of an expression within a function context.
        /// </summary>
        /// <param name="e">Expression to be infered.</param>
        /// <param name="paramMap">Map of parameters type.</param>
        /// <returns></returns>
        private static Type InferExpressionTypeForFunction(Expression e, Dictionary<string, Type> paramMap)
        {
            switch(e)
            {
                case IntegerLiteral: 
                    return typeof(int);
                case FloatLiteral: 
                    return typeof(double);
                case BooleanLiteral: 
                    return typeof(bool);
                case StringLiteral: 
                    return typeof(string);
                case VariableReference variableReference:
                    if (paramMap.TryGetValue(variableReference.Name, out var pt)) return pt;
                    return typeof(int);
                case FunctionCall functionCall:
                    if (functionCall.Name == "input") return typeof(string);
                    if (_functionRegistry.Value != null && _functionRegistry.Value.TryGetValue(functionCall.Name, out var mi)) return mi.ReturnType;
                    return typeof(int);
                case BinaryExpression binaryExpression:
                    if(BooleanOperators.Contains(binaryExpression.Operator)) return typeof(bool);
                    if (binaryExpression.Operator == OperatorType.Plus && (ContainsString(binaryExpression.Left) || ContainsString(binaryExpression.Right))) return typeof(string);
                    var lt = InferExpressionTypeForFunction(binaryExpression.Left, paramMap);
                    var rt = InferExpressionTypeForFunction(binaryExpression.Right, paramMap);
                    if (lt == typeof(double) || rt == typeof(double)) return typeof(double);
                    return typeof(int);
                default:
                    return typeof(int);
            }
        }

        /// <summary>
        /// Emits the body of a compiled function
        /// </summary>
        /// <param name="mb">MethodBuilder.</param>
        /// <param name="fd">FunctionDecleration to be IL emited.</param>
        /// <param name="returnType">Return type.</param>
        /// <param name="paramTypes">Parameters types.</param>
        /// <param name="paramMap">Map of paramaters type.</param>
        private static void EmitFunctionBody(MethodBuilder mb, FunctionDeclaration fd, Type returnType, Type[] paramTypes, Dictionary<string, Type> paramMap)
        {
            // Set Up
            var il = mb.GetILGenerator();
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();

            // Copy params into local
            for(int i = 0; i < fd.Parameters.Count; i++)
            {
                var vLocal = il.DeclareLocal(paramTypes[i]);
                locals[fd.Parameters[i]] = vLocal;
                il.Emit(OpCodes.Ldarg, i);
                il.Emit(OpCodes.Stloc, vLocal);
            }

            // End label + return local if needed
            var endLabel = il.DefineLabel();
            LocalBuilder? retLocal = null;
            if(returnType != typeof(void)) retLocal = il.DeclareLocal(returnType);
            
            // Emit statements
            foreach(var st in fd.Body.Statements)
            {
                if (st is ReturnStatement returnStatement)
                {
                    GenerateILWithConversion(returnStatement.Expression, il, returnType, locals, constants);
                    il.Emit(OpCodes.Stloc, retLocal);
                    il.Emit(OpCodes.Br, endLabel);
                }
                else GenerateIL(st, il, locals, constants, null);
            }

            // Epilogue
            il.MarkLabel(endLabel);
            if (returnType != typeof(void)) il.Emit(OpCodes.Ldloc, retLocal!);
            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Maps MinImp type annotation to CLR types (thorws if missing / unknown).
        /// </summary>
        /// <param name="annotation">TypeAnnotation who will give the system type.</param>
        /// <returns>System type from <see cref="TypeAnnotation"/>.</returns>
        /// <exception cref="InvalidOperationException">Throws if type is missing.</exception>
        /// <exception cref="NotSupportedException">Throws if unknown type.</exception>
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
