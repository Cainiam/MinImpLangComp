using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using System.Globalization;
using System.Text;

namespace MinImpLangComp.Transpiling
{
    /// <summary>
    /// Very lightweight source-to-source transpiler that emits a C# program from a MinImp AST <see cref="Block"/> (treated as a "program").
    /// </summary>
    /// <remarks>
    /// <para>
    /// This transpiler is intnentionnaly simple and conservative: it keeps generation rules close to the original implementation and does not attempt advanced type inference.
    /// </para>
    /// </remarks>
    public class Transpiler
    {

        /// <summary>
        /// Transpiles a MinImp <see cref="Block"/> into a self-contained C# Program.
        /// </summary>
        /// <param name="program">The AST program block.</param>
        /// <returns>The generated C# source code.</returns>
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

        /// <summary>
        /// Transpiles a single statement into C#.
        /// </summary>
        /// <param name="statement"><see cref="Statement"/> to be transpiled.</param>
        /// <returns>The generated C# source code.</returns>
        /// <exception cref="NotImplementedException">Throws if action not implemented</exception>
        private string TranspileStatement(Statement statement)
        {
            switch (statement)
            {
                case ExpressionStatement expressionStatement:
                    if (expressionStatement.Expression is UnaryExpression unary) return $"{unary.Identifier}{(unary.Operator == OperatorType.PlusPlus ? "++" : "--")};";
                    return TranspileExpression(expressionStatement.Expression) + ";";
                case VariableDeclaration variableDeclaration:
                    return $"{ResolveCSharpType(variableDeclaration.TypeAnnotation)} {variableDeclaration.Identifier} = {TranspileExpression(variableDeclaration.Expression)};";
                case Assignment assignment:
                    return $"var {assignment.Identifier} = {TranspileExpression(assignment.Expression)};";
                case ConstantDeclaration constantDeclaration:
                    return $"{ResolveCSharpType(constantDeclaration.TypeAnnotation)} {constantDeclaration.Identifier} = {TranspileExpression(constantDeclaration.Expression)};";
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
                case ReturnStatement returnStatement:
                    return $"return {TranspileExpression(returnStatement.Expression)};";
                case BreakStatement:
                    return "break;\n";
                case ContinueStatement:
                    return "continue;\n";
                default:
                    throw new NotImplementedException($"Transpilation not yet implemented for {statement.GetType()}");
            }
        }

        /// <summary>
        /// Transpiles an expression into C#.
        /// </summary>
        /// <param name="expression"><see cref="Expression"/> to be transpiled.</param>
        /// <returns>The generated C# source code.</returns>
        /// <exception cref="NotImplementedException">Throws  if action not implemented.</exception>
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
                case NullLiteral:
                    return "null";
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

        /// <summary>
        /// Maps MinImp <see cref="OperatorType"/> to teh corresponding C# operator symbol.
        /// </summary>
        /// <param name="operatorType"><see cref="OperatorType"/> who will give its symbol.</param>
        /// <returns>The character (symbol) of the <see cref="OperatorType"/>.</returns>
        /// <exception cref="NotImplementedException">Throws if <see cref="OperatorType"/> doesn't exist.</exception>
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

        /// <summary>
        /// Resolves a MinImp type annotation into a C# type identifier (or <c>var</c> when missing)
        /// </summary>
        /// <param name="annotation"><see cref="TypeAnnotation"/> to be map into a C# type identifier.</param>
        /// <returns>The C# type identifier</returns>
        /// <exception cref="NotImplementedException">Throws is <see cref="TypeAnnotation"/> doesn't exist.</exception>
        private string ResolveCSharpType(TypeAnnotation? annotation)
        {
            if (annotation == null) return "var";
            return annotation.TypeToken switch
            {
                TokenType.TypeInt => "int",
                TokenType.TypeFloat => "double",
                TokenType.TypeBool => "bool",
                TokenType.TypeString => "string",
                _ => throw new NotImplementedException($"Unsupported type annotation: {annotation.TypeToken}")
            };
        }
    }
}
