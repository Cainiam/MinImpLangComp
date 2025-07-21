using MinImpLangComp.Exceptions;
using MinImpLangComp.AST;

namespace MinImpLangComp.Interpreting
{
    public class Interpreter
    {
        private readonly Dictionary<string, object> _environment = new Dictionary<string, object>();

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
                case BinaryExpression binary:
                    var left = Evaluate(binary.Left);
                    var right = Evaluate(binary.Right);
                    if (left is int leftInteger && right is int rightInteger)
                    {
                        return binary.Operator switch
                        {
                            OperatorType.Plus => leftInteger + rightInteger,
                            OperatorType.Minus => leftInteger - rightInteger,
                            OperatorType.Multiply => leftInteger * rightInteger,
                            OperatorType.Divide => leftInteger / rightInteger,
                            OperatorType.Less => leftInteger < rightInteger,
                            OperatorType.Greater => leftInteger > rightInteger,
                            OperatorType.LessEqual => leftInteger <= rightInteger,
                            OperatorType.GreaterEqual => leftInteger >= rightInteger,
                            OperatorType.Equalequal => leftInteger == rightInteger,
                            OperatorType.NotEqual => leftInteger != rightInteger,
                            _ => throw new RuntimeException($"Unknown operator {binary.Operator}")
                        };
                    }
                    else
                    {
                        double leftValue = Convert.ToDouble(left);
                        double rightValue = Convert.ToDouble(right);
                        return binary.Operator switch
                        {
                            OperatorType.Plus => leftValue + rightValue,
                            OperatorType.Minus => leftValue - rightValue,
                            OperatorType.Multiply => leftValue * rightValue,
                            OperatorType.Divide => leftValue / rightValue,
                            OperatorType.Less => leftValue < rightValue,
                            OperatorType.Greater => leftValue > rightValue,
                            OperatorType.LessEqual => leftValue <= rightValue,
                            OperatorType.GreaterEqual => leftValue >= rightValue,
                            OperatorType.Equalequal => leftValue == rightValue,
                            OperatorType.NotEqual => leftValue != rightValue,
                            _ => throw new RuntimeException($"Unknown operator {binary.Operator}")
                        };
                    }
                case VariableReference variable:
                    if (_environment.TryGetValue(variable.Name, out var value)) return value;
                    else throw new RuntimeException($"Undefined variable {variable.Name}");
                case Assignment assign:
                    var assignedValue = Evaluate(assign.Expression);
                    _environment[assign.Identifier] = assignedValue;
                    return assignedValue;
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
                    while (Convert.ToBoolean(Evaluate(whileStatement.Condition))) lastWhile = Evaluate(whileStatement.Body);
                    return lastWhile;
                case ForStatement forStatement:
                    object? lastFor = null;
                    if (forStatement.Initializer != null) Evaluate(forStatement.Initializer);
                    while (Convert.ToBoolean(Evaluate(forStatement.Condition)))
                    {
                        lastFor = Evaluate(forStatement.Body);
                        if(forStatement.Increment != null) Evaluate(forStatement.Increment);
                    }
                    return lastFor;
                case BooleanLiteral booleanLiteral:
                    return booleanLiteral.Value;
                default:
                    throw new RuntimeException($"Unsupported node type: {node.GetType().Name}");
                }
        }
    }
}
