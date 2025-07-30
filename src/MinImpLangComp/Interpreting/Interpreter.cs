using MinImpLangComp.Exceptions;
using MinImpLangComp.AST;
using MinImpLangComp.Lexing;

namespace MinImpLangComp.Interpreting
{
    public class Interpreter
    {
        private readonly Dictionary<string, object> _environment = new Dictionary<string, object>();
        private readonly HashSet<string> _constant = new HashSet<string>();

        public Dictionary<string, object> GetEnvironment()
        {
            return _environment;
        }

        public object Evaluate (Node node)
        {
            switch (node)
            {
                case IntegerLiteral integerLiteral:
                    return integerLiteral.Value;
                case FloatLiteral floatLiteral:
                    return floatLiteral.Value;
                case StringLiteral stringLiteral:
                    return stringLiteral.Value;
                case BinaryExpression binary:
                    var left = Evaluate(binary.Left);
                    var right = Evaluate(binary.Right);
                    switch (binary.Operator)
                    {
                        case OperatorType.Plus:
                            if (left is string leftStr && right is string rightStr) return leftStr + rightStr;
                            else if (left is string leftStr2) return leftStr2 + right.ToString();
                            else if (right is string rightStr2) return left.ToString() + rightStr2;
                            else if (left is int leftInt && right is int rightInt) return leftInt + rightInt;
                            else
                            {
                                double leftVal = Convert.ToDouble(left);
                                double rightVal = Convert.ToDouble(right);
                                return leftVal + rightVal;
                            }
                        case OperatorType.Minus:
                            if (left is int li && right is int ri) return li - ri;
                            else return Convert.ToDouble(left) - Convert.ToDouble(right);
                        case OperatorType.Multiply:
                            if (left is int li2 && right is int ri2) return li2 * ri2;
                            else return Convert.ToDouble(left) * Convert.ToDouble(right);
                        case OperatorType.Divide:
                            if (left is int li3 && right is int ri3) return li3 / ri3;
                            else return Convert.ToDouble(left) / Convert.ToDouble(right);
                        case OperatorType.Modulo:
                            if (left is int li4 && right is int ri4) return li4 % ri4;
                            else return Convert.ToDouble(left) % Convert.ToDouble(right);
                        case OperatorType.Less:
                            return Convert.ToDouble(left) < Convert.ToDouble(right);
                        case OperatorType.Greater:
                            return Convert.ToDouble(left) > Convert.ToDouble(right);
                        case OperatorType.LessEqual:
                            return Convert.ToDouble(left) <= Convert.ToDouble(right);
                        case OperatorType.GreaterEqual:
                            return Convert.ToDouble(left) >= Convert.ToDouble(right);
                        case OperatorType.Equalequal:
                            return Equals(left, right);
                        case OperatorType.NotEqual:
                            return !Equals(left, right);
                        case OperatorType.AndAnd:
                            return Convert.ToBoolean(left) && Convert.ToBoolean(right);
                        case OperatorType.OrOr:
                            return Convert.ToBoolean(left) || Convert.ToBoolean(right);
                        case OperatorType.BitwiseAnd:
                            if (left is int li5 && right is int ri5) return li5 & ri5;
                            else throw new RuntimeException($"Operator {binary.Operator} only accept integer value");
                        case OperatorType.BitwiseOr:
                            if (left is int li6 && right is int ri6) return li6 | ri6;
                            else throw new RuntimeException($"Operator {binary.Operator} only accept integer value");
                        default:
                            throw new RuntimeException($"Unknow operator {binary.Operator}");
                    }
                case VariableReference variable:
                    if (_environment.TryGetValue(variable.Name, out var value)) return value;
                    else throw new RuntimeException($"Undefined variable {variable.Name}");
                case VariableDeclaration variableDeclaration:
                    if (_environment.ContainsKey(variableDeclaration.Identifier)) throw new RuntimeException($"Variable {variableDeclaration.Identifier} is already declared");
                    var declaredValue = Evaluate(variableDeclaration.Expression);
                    if (!IsCompatibleType(variableDeclaration.TypeAnnotation, declaredValue)) throw new RuntimeException($"Type mismatch in variable declaration '{variableDeclaration.Identifier}': expected '{variableDeclaration.TypeAnnotation}', got '{declaredValue?.GetType().Name ?? "null"}'");
                    _environment[variableDeclaration.Identifier] = declaredValue;
                    return declaredValue;
                case Assignment assign:
                    if (_constant.Contains(assign.Identifier)) throw new RuntimeException($"Cannot reassign to constant '{assign.Identifier}'");
                    var assignedValue = Evaluate(assign.Expression);
                    if(_environment.ContainsKey(assign.Identifier))
                    {
                        var declaredNode = _environment[assign.Identifier];
                        var declaredType = declaredNode switch
                        {
                            int => "int",
                            double => "float",
                            bool => "bool",
                            string => "string",
                            _ => null
                        };
                        if(declaredType != null)
                        {
                            var fakeAnnotation = new TypeAnnotation(declaredType, TokenType.Identifier);
                            if (!IsCompatibleType(fakeAnnotation, assignedValue)) throw new RuntimeException($"Type mismatch in assignment to '{assign.Identifier}': expected '{declaredType}', got '{assignedValue?.GetType().Name ?? "null"}'");
                        }
                    }
                    _environment[assign.Identifier] = assignedValue;
                    return assignedValue;
                case ConstantDeclaration constantDeclaration:
                    if (_environment.ContainsKey(constantDeclaration.Identifier)) throw new RuntimeException($"Constant '{constantDeclaration.Identifier}' already defined");
                    var constValue = Evaluate(constantDeclaration.Expression);
                    if (!IsCompatibleType(constantDeclaration.TypeAnnotation, constValue)) throw new RuntimeException($"Type mismatch in variable declaration '{constantDeclaration.Identifier}': expected '{constantDeclaration.TypeAnnotation}', got '{constValue?.GetType().Name ?? "null"}'");
                    _environment[constantDeclaration.Identifier] = constValue;
                    _constant.Add(constantDeclaration.Identifier);
                    return null;
                case Block block:
                    object? lastBlock = null;
                    foreach(var statement in block.Statements)
                    {
                        lastBlock = Evaluate(statement);
                    }
                    return lastBlock;
                case ExpressionStatement expressStatement:
                    return Evaluate(expressStatement.Expression);
                case IfStatement ifStatement:
                    var conditionValue = Evaluate(ifStatement.Condition);
                    if (Convert.ToBoolean(conditionValue)) return Evaluate(ifStatement.ThenBranch);
                    else if (ifStatement.ElseBranch != null) return Evaluate(ifStatement.ElseBranch);
                    return null;
                case WhileStatement whileStatement:
                    object? lastWhile = null;
                    while (Convert.ToBoolean(Evaluate(whileStatement.Condition)))
                    {
                        try
                        {
                            lastWhile = Evaluate(whileStatement.Body);
                        }
                        catch (ContinueException)
                        {
                            continue;
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                    }
                    return lastWhile;
                case ForStatement forStatement:
                    object? lastFor = null;
                    if (forStatement.Initializer != null) Evaluate(forStatement.Initializer);
                    while (Convert.ToBoolean(Evaluate(forStatement.Condition)))
                    {
                        try
                        {
                            lastFor = Evaluate(forStatement.Body);
                        }
                        catch (ContinueException)
                        {
                            if (forStatement.Increment != null) Evaluate(forStatement.Increment);
                            continue;
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                        if (forStatement.Increment != null) Evaluate(forStatement.Increment);
                    }
                    return lastFor;
                case BooleanLiteral booleanLiteral:
                    return booleanLiteral.Value;
                case NullLiteral:
                    return null;
                case BreakStatement:
                    throw new BreakException();
                case ContinueStatement:
                    throw new ContinueException();
                case UnaryExpression unary:
                    if (!_environment.ContainsKey(unary.Identifier)) throw new RuntimeException($"Undefined variable {unary.Identifier}");
                    if (_environment[unary.Identifier] is int currentInt)
                    {
                        int newValue = unary.Operator switch
                        {
                            OperatorType.PlusPlus => currentInt + 1,
                            OperatorType.MinusMinus => currentInt - 1,
                            _ => throw new RuntimeException($"Unknow unary operator {unary.Operator}")
                        };
                        _environment[unary.Identifier] = newValue;
                        return newValue;
                    }
                    else throw new RuntimeException($"Unsupported type for unary operation: {unary.Identifier}");
                case UnaryNotExpression unaryNot:
                    var valueNotExpr = Evaluate(unaryNot.Operand);
                    return !Convert.ToBoolean(valueNotExpr);
                case ReturnStatement returnStatement:
                    var returnValue = Evaluate(returnStatement.Expression);
                    throw new ReturnException(returnValue);
                case FunctionCall functionCall:
                    {
                        switch(functionCall.Name)
                        {
                            case "print":
                                foreach (var argument in functionCall.Arguments)
                                {
                                    var valueCall = Evaluate(argument);
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine(valueCall);
                                    Console.ResetColor();
                                }
                                return null;
                            case "input":
                                Console.Write("~ ");
                                string? userInput = Console.ReadLine();
                                if (int.TryParse(userInput, out int intResult)) return intResult;
                                else if (double.TryParse(userInput, out double doubleResult)) return doubleResult;
                                else return userInput ?? "";
                            default:
                                if(!_environment.ContainsKey(functionCall.Name))
                                    throw new RuntimeException($"Undefined function; {functionCall.Name}");
                                if (!(_environment[functionCall.Name] is FunctionDeclaration functionDeclaration))
                                    throw new RuntimeException($"{functionCall.Name} is not a function");
                                if (functionDeclaration.Parameters.Count != functionCall.Arguments.Count)
                                    throw new RuntimeException($"Function {functionCall.Name} expects {functionDeclaration.Parameters.Count} arguments but got {functionCall.Arguments.Count}");
                                var previousEnv = new Dictionary<string, object>(_environment);
                                for(int i = 0; i < functionDeclaration.Parameters.Count; i++)
                                {
                                    var argValue = Evaluate(functionCall.Arguments[i]);
                                    _environment[functionDeclaration.Parameters[i]] = argValue;
                                }
                                try 
                                {
                                    Evaluate(functionDeclaration.Body);
                                    _environment.Clear();
                                    foreach (var kvp in previousEnv) _environment[kvp.Key] = kvp.Value;
                                    return null;
                                }
                                catch(ReturnException re)
                                {
                                    _environment.Clear();
                                    foreach (var kvp in previousEnv) _environment[kvp.Key] = kvp.Value;
                                    return re.ReturnValue;
                                }
                        }
                    }
                case FunctionDeclaration functionDeclaration:
                    _environment[functionDeclaration.Name] = functionDeclaration;
                    return null;
                case ArrayLiteral arrayLiteral:
                    List<object> evaluatedElements = new List<object>();
                    foreach (var element in arrayLiteral.Elements) evaluatedElements.Add(Evaluate(element));
                    return evaluatedElements;
                case ArrayAccess arrayAccess:
                    if (!_environment.ContainsKey(arrayAccess.Identifier)) throw new RuntimeException($"Undefined array {arrayAccess.Identifier}");
                    var arrayObj = _environment[arrayAccess.Identifier];
                    if (arrayObj is List<object> list)
                    {
                        int index = Convert.ToInt32(Evaluate(arrayAccess.Index));
                        if (index < 0 || index >= list.Count) throw new RuntimeException($"Index out of range for array {arrayAccess.Identifier}");
                        return list[index];
                    }
                    else throw new RuntimeException($"{arrayAccess.Identifier} is not an array");
                case ArrayAssignment arrayAssignment:
                    if (!_environment.ContainsKey(arrayAssignment.Identifier)) throw new RuntimeException($"Undefined array {arrayAssignment.Identifier}");
                    var arrObj = _environment[arrayAssignment.Identifier];
                    if (arrObj is List<object> list2)
                    {
                        int indx = Convert.ToInt32(Evaluate(arrayAssignment.Index));
                        var val = Evaluate(arrayAssignment.Value);
                        if (indx < 0 || indx >= list2.Count) throw new RuntimeException($"Index out of range for array {arrayAssignment.Identifier}");
                        list2[indx] = val;
                        return null;
                    }
                    else throw new RuntimeException($"{arrayAssignment.Identifier} is not an array");
                default:
                    throw new RuntimeException($"Unsupported node type: {node.GetType().Name}");
                }
        }

        private bool IsCompatibleType(TypeAnnotation annotation, object? value)
        {
            if (annotation == null) return true;
            return annotation.TypeName switch
            {
                "int" => value is int,
                "float" => value is double || value is int,
                "bool" => value is bool,
                "string" => value is string,
                _ => throw new RuntimeException($"Unknow type '{annotation.TypeName}'")
            };
        }
    }
}
