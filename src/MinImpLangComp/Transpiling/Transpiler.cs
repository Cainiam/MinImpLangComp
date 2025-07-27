using MinImpLangComp.AST;
using System.Globalization;
using System.Text;

namespace MinImpLangComp.Transpiling
{
    public class Transpiler
    {
        public string Transpile(Block program)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace MinImpLangComp");
            sb.AppendLine("{");
            sb.AppendLine("    class Program");
            sb.AppendLine("    {");
            sb.AppendLine("        static void Main(string[] args)");
            sb.AppendLine("        {");

            foreach(var statement in program.Statements) sb.AppendLine("            " + TranspileStatement(statement));


            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string TranspileStatement(Statement statement)
        {
            switch (statement)
            {
                case ExpressionStatement expressionStatement:
                    return TranspileExpression(expressionStatement.Expression) + ";";
                case Assignment assignment:
                    return $"var {assignment.Identifier} = {TranspileExpression(assignment.Expression)};";
                case IfStatement ifStatement:
                    var sbIF = new StringBuilder();
                    sbIF.AppendLine($"if ({TranspileExpression(ifStatement.Condition)})");
                    sbIF.AppendLine("{");
                    sbIF.AppendLine("    " + TranspileStatement(ifStatement.ThenBranch));
                    sbIF.AppendLine("}");
                    if (ifStatement.ElseBranch != null)
                    {
                        sbIF.AppendLine("else");
                        sbIF.AppendLine("{");
                        sbIF.AppendLine("    " + TranspileStatement(ifStatement.ElseBranch));
                        sbIF.AppendLine("}");
                    }
                    return sbIF.ToString();
                case WhileStatement whileStatement:
                    var sbWhile = new StringBuilder();
                    sbWhile.AppendLine($"while ({TranspileExpression(whileStatement.Condition)})");
                    sbWhile.AppendLine("{");
                    sbWhile.AppendLine("    " + TranspileStatement(whileStatement.Body));
                    sbWhile.AppendLine("}");
                    return sbWhile.ToString();
                case ForStatement forStatement:
                    var init = TranspileStatement(forStatement.Initializer).TrimEnd(';');
                    var condition = TranspileExpression(forStatement.Condition);
                    string increment;
                    if (forStatement.Increment is Assignment assign) increment = $"{assign.Identifier} = {TranspileExpression(assign.Expression)}";
                    else increment = TranspileStatement(forStatement.Increment).TrimEnd(';');
                    var sbFor = new StringBuilder();
                    sbFor.AppendLine($"for ({init}; {condition}; {increment})");
                    sbFor.AppendLine("{");
                    sbFor.AppendLine("    " + TranspileStatement(forStatement.Body));
                    sbFor.AppendLine("}");
                    return sbFor.ToString();
                case Block block:
                    var sbBlock = new StringBuilder();
                    sbBlock.AppendLine("{");
                    foreach (var stmt in block.Statements) sbBlock.AppendLine("    " + TranspileStatement(stmt));
                    sbBlock.AppendLine("}");
                    return sbBlock.ToString();
                case FunctionDeclaration functionDeclaration:
                    var sbFunc = new StringBuilder();
                    var parameters = string.Join(", ", functionDeclaration.Parameters.Select(p => $"dynamic {p}"));
                    sbFunc.AppendLine($"static void {functionDeclaration.Name}({parameters})");
                    sbFunc.AppendLine("{");
                    if (functionDeclaration.Body is Block blockFunc)
                    {
                        foreach (var stmt in blockFunc.Statements) sbFunc.Append("    " + TranspileStatement(stmt));
                    }
                    else sbFunc.AppendLine("    " + TranspileStatement(functionDeclaration.Body));
                    sbFunc.AppendLine("}");
                    return sbFunc.ToString();
                default:
                    throw new NotImplementedException($"Transpilation not yet implemented for {statement.GetType()}");
            }
        }

        private string TranspileExpression(Expression expression)
        {
            switch (expression)
            {
                case IntegerLiteral i:
                    return i.Value.ToString();
                case FloatLiteral f:
                    return f.Value.ToString(CultureInfo.InvariantCulture);
                case StringLiteral s:
                    return $"\"{s.Value}\"";
                case BooleanLiteral b:
                    return b.Value ? "true" : "false";
                case VariableReference v:
                    return v.Name;
                case BinaryExpression binaryExpression:
                    return $"({TranspileExpression(binaryExpression.Left)} {GetOperatorSymbol(binaryExpression.Operator)} {TranspileExpression(binaryExpression.Right)})";
                case FunctionCall call when call.Name == "print":
                    return $"Console.WriteLine({string.Join(", ", call.Arguments.Select(TranspileExpression))})";
                case FunctionCall call when call.Name == "input":
                    return "Console.ReadLine()";
                case FunctionCall call:
                    return $"{call.Name}({string.Join(", ", call.Arguments.Select(TranspileExpression))})";
                default:
                    throw new NotImplementedException($"Transpilation not yet implemented for {expression.GetType()}");
            }
        }

        private string GetOperatorSymbol(OperatorType operatorType)
        {
            return operatorType switch
            {
                OperatorType.Plus => "+",
                OperatorType.Minus => "-",
                OperatorType.Multiply => "*",
                OperatorType.Divide => "/",
                OperatorType.Modulo => "%",
                OperatorType.Less => "<",
                OperatorType.Greater => ">",
                OperatorType.LessEqual => "<=",
                OperatorType.GreaterEqual => ">=",
                OperatorType.Equalequal => "==",
                OperatorType.NotEqual => "!=",
                OperatorType.AndAnd => "&&",
                OperatorType.OrOr => "||",
                OperatorType.BitwiseAnd => "&",
                OperatorType.BitwiseOr => "|",
                _ => throw new NotImplementedException($"Operator {operatorType} is not supported yet")
            };
        }
    }
}
