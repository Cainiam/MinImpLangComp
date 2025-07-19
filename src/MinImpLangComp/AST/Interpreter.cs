using System;
using System.Collections.Generic;

namespace MinImpLangComp.AST
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
                            "+" => leftInteger + rightInteger,
                            "-" => leftInteger - rightInteger,
                            "*" => leftInteger * rightInteger,
                            "/" => leftInteger / rightInteger,
                            _ => throw new Exception($"Unknown operator {binary.Operator}")
                        };
                    }
                    else
                    {
                        double leftValue = Convert.ToDouble(left);
                        double rightValue = Convert.ToDouble(right);
                        return binary.Operator switch
                        {
                            "+" => leftValue + rightValue,
                            "-" => leftValue - rightValue,
                            "*" => leftValue * rightValue,
                            "/" => leftValue / rightValue,
                            _ => throw new Exception($"Unknown operator {binary.Operator}")
                        };
                    }
                case VariableReference variable:
                    if (_environment.TryGetValue(variable.Name, out var value)) return value;
                    else throw new Exception($"Undefined variable {variable.Name}");
                case Assignment assign:
                    var assignedValue = Evaluate(assign.Expression);
                    _environment[assign.Identifier] = assignedValue;
                    return assignedValue;
                case Block block:
                    object? last = null;
                    foreach(var statement in block.Statements)
                    {
                        last = Evaluate(statement);
                    }
                    return last;
                case ExpressionStatement expressStatement:
                    return Evaluate(expressStatement.Expression);
                default:
                    throw new Exception($"Unsupported node type: {node.GetType().Name}");
                }
        }
    }
}
