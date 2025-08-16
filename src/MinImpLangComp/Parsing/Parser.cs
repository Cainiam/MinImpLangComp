using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.Exceptions;
using System.Globalization;

namespace MinImpLangComp.Parsing
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;
        public bool IsAtEnd => _currentToken.Type == TokenType.EOF; // Pour CompilerFacade

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        private void Eat(TokenType type)
        {
            if (_currentToken.Type == type) _currentToken = _lexer.GetNextToken();
            else throw new ParsingException($"Unexpected token: {_currentToken.Type}, expected: {type}");
        }

        public Expression ParseExpression()
        {
            var left = ParseTerm();
            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus || _currentToken.Type == TokenType.Less 
                || _currentToken.Type == TokenType.Greater || _currentToken.Type == TokenType.LessEqual || _currentToken.Type == TokenType.GreaterEqual 
                || _currentToken.Type == TokenType.Equalequal || _currentToken.Type == TokenType.NotEqual || _currentToken.Type == TokenType.AndAnd 
                || _currentToken.Type == TokenType.OrOr || _currentToken.Type == TokenType.BitwiseAnd || _currentToken.Type == TokenType.BitwiseOr)
            {
                OperatorType oper;
                switch(_currentToken.Type)
                {
                    case TokenType.Plus:
                        oper = OperatorType.Plus;
                        break;
                    case TokenType.Minus:
                        oper = OperatorType.Minus;
                        break;
                    case TokenType.Less:
                        oper = OperatorType.Less;
                        break;
                    case TokenType.Greater:
                        oper = OperatorType.Greater;
                        break;
                    case TokenType.LessEqual:
                        oper = OperatorType.LessEqual;
                        break;
                    case TokenType.GreaterEqual:
                        oper = OperatorType.GreaterEqual;
                        break;
                    case TokenType.Equalequal:
                        oper = OperatorType.Equalequal;
                        break;
                    case TokenType.NotEqual:
                        oper = OperatorType.NotEqual;
                        break;
                    case TokenType.AndAnd:
                        oper = OperatorType.AndAnd;
                        break;
                    case TokenType.OrOr:
                        oper = OperatorType.OrOr;
                        break;
                    case TokenType.BitwiseAnd:
                        oper = OperatorType.BitwiseAnd;
                        break;
                    case TokenType.BitwiseOr:
                        oper = OperatorType.BitwiseOr;
                        break;
                    default:
                        throw new ParsingException($"Unexpected operator token: {_currentToken.Type}");
                }
                Eat(_currentToken.Type);
                var right = ParseTerm();
                left = new BinaryExpression(left, oper, right);
            }
            return left;
        }

        private Expression ParseTerm()
        {
            var left = ParseFactor();
            while(_currentToken.Type == TokenType.Multiply || _currentToken.Type == TokenType.Divide || _currentToken.Type == TokenType.Modulo)
            {
                OperatorType oper;
                switch(_currentToken.Type)
                {
                    case TokenType.Multiply:
                        oper = OperatorType.Multiply;
                        break;
                    case TokenType.Divide:
                        oper = OperatorType.Divide;
                        break;
                    case TokenType.Modulo:
                        oper = OperatorType.Modulo;
                        break;
                    default:
                        throw new ParsingException($"Unexpected operator token: {_currentToken.Type}");
                }
                Eat(_currentToken.Type);
                var right = ParseFactor();
                left = new BinaryExpression(left, oper, right);
            }
            return left;
        }

        private Expression ParseFactor()
        {
            if (_currentToken.Type == TokenType.Integer)
            {
                int value = int.Parse(_currentToken.Value);
                Eat(TokenType.Integer);
                return new IntegerLiteral(value);
            }
            else if(_currentToken.Type == TokenType.Float)
            {
                double value = double.Parse(_currentToken.Value, CultureInfo.InvariantCulture);
                Eat(TokenType.Float);
                return new FloatLiteral(value);
            }
            else if (_currentToken.Type == TokenType.StringLiteral)
            {
                string value = _currentToken.Value;
                Eat(TokenType.StringLiteral);
                return new StringLiteral(value);
            }
            else if (_currentToken.Type == TokenType.LeftParen)
            {
                Eat(TokenType.LeftParen);
                var expr = ParseExpression();
                Eat(TokenType.RightParen);
                return expr;
            }
            else if (_currentToken.Type == TokenType.LeftBracket)
            {
                Eat(TokenType.LeftBracket);
                List<Expression> elements = new List<Expression>();
                if (_currentToken.Type != TokenType.RightBracket)
                {
                    elements.Add(ParseExpression());
                    while(_currentToken.Type == TokenType.Comma)
                    {
                        Eat(TokenType.Comma);
                        elements.Add(ParseExpression());
                    }
                }
                Eat(TokenType.RightBracket);
                return new ArrayLiteral(elements);
            }
            else if (_currentToken.Type == TokenType.Identifier)
            {
                string name = _currentToken.Value;
                Eat(TokenType.Identifier);
                if (_currentToken.Type == TokenType.LeftParen)
                {
                    Eat(TokenType.LeftParen);
                    List<Expression> arguments = new List<Expression>();
                    if (_currentToken.Type != TokenType.RightParen)
                    {
                        arguments.Add(ParseExpression());
                        while (_currentToken.Type == TokenType.Comma)
                        {
                            Eat(TokenType.Comma);
                            arguments.Add(ParseExpression());
                        }
                    }
                    Eat(TokenType.RightParen);
                    return new FunctionCall(name, arguments);
                }
                else if (_currentToken.Type == TokenType.LeftBracket)
                {
                    Eat(TokenType.LeftBracket);
                    var indexExpr = ParseExpression();
                    Eat(TokenType.RightBracket);
                    return new ArrayAccess(name, indexExpr);
                }
                else return new VariableReference(name);
            }
            else if (_currentToken.Type == TokenType.True)
            {
                Eat(TokenType.True);
                return new BooleanLiteral(true);
            }
            else if (_currentToken.Type == TokenType.False)
            {
                Eat(TokenType.False);
                return new BooleanLiteral(false);
            }
            else if(_currentToken.Type == TokenType.Null)
            {
                Eat(TokenType.Null);
                return new NullLiteral();
            }
            else if (_currentToken.Type == TokenType.Not)
            {
                Eat(TokenType.Not);
                var operand = ParseFactor();
                return new UnaryNotExpression(operand);
            }
            else throw new ParsingException($"Unexpected token; {_currentToken.Type}");
        }

        public Statement ParseStatement()
        {
            if (_currentToken.Type == TokenType.Set) return ParseVariableDeclaration();
            else if (_currentToken.Type == TokenType.Bind) return ParseConstantDeclaration();
            else if (_currentToken.Type == TokenType.Break)
            {
                Eat(TokenType.Break);
                Eat(TokenType.Semicolon);
                return new BreakStatement();
            }
            else if (_currentToken.Type == TokenType.Continue)
            {
                Eat(TokenType.Continue);
                Eat(TokenType.Semicolon);
                return new ContinueStatement();
            }
            else if (_currentToken.Type == TokenType.Identifier)
            {
                string identifier = _currentToken.Value;
                Eat(TokenType.Identifier);
                if (_currentToken.Type == TokenType.LeftBracket)
                {
                    Eat(TokenType.LeftBracket);
                    var indexEpr = ParseExpression();
                    Eat(TokenType.RightBracket);
                    if (_currentToken.Type == TokenType.Assign)
                    {
                        Eat(TokenType.Assign);
                        var valueExpr = ParseExpression();
                        Eat(TokenType.Semicolon);
                        return new ArrayAssignment(identifier, indexEpr, valueExpr);
                    }
                    else
                    {
                        Eat(TokenType.Semicolon);
                        return new ExpressionStatement(new ArrayAccess(identifier, indexEpr));
                    }
                }
                else if (_currentToken.Type == TokenType.LeftParen)
                {
                    Eat(TokenType.LeftParen);
                    List<Expression> arguments = new List<Expression>();
                    if (_currentToken.Type != TokenType.RightParen)
                    {
                        arguments.Add(ParseExpression());
                        while (_currentToken.Type == TokenType.Comma)
                        {
                            Eat(TokenType.Comma);
                            arguments.Add(ParseExpression());
                        }
                    }
                    Eat(TokenType.RightParen);
                    Eat(TokenType.Semicolon);
                    return new ExpressionStatement(new FunctionCall(identifier, arguments));
                }
                else if (_currentToken.Type == TokenType.Assign)
                {
                    Eat(TokenType.Assign);
                    var expr = ParseExpression();
                    Eat(TokenType.Semicolon);
                    return new Assignment(identifier, expr);
                }
                else
                {
                    var expr = new VariableReference(identifier);
                    Eat(TokenType.Semicolon);
                    return new ExpressionStatement(expr);
                }
            }
            else if (_currentToken.Type == TokenType.LeftBrace) return ParseBlock();
            else if (_currentToken.Type == TokenType.If) return ParseIfStatement();
            else if (_currentToken.Type == TokenType.While) return ParseWhileStatement();
            else if (_currentToken.Type == TokenType.For) return ParseForStatement();
            else if (_currentToken.Type == TokenType.PlusPlus || _currentToken.Type == TokenType.MinusMinus)
            {
                OperatorType oper = _currentToken.Type == TokenType.PlusPlus ? OperatorType.PlusPlus : OperatorType.MinusMinus;
                Eat(_currentToken.Type);
                if (_currentToken.Type != TokenType.Identifier) throw new ParsingException($"Expected identifier after {oper}");
                string identifier = _currentToken.Value;
                Eat(TokenType.Identifier);
                Eat(TokenType.Semicolon);
                return new ExpressionStatement(new UnaryExpression(oper, identifier));
            }
            else if (_currentToken.Type == TokenType.Function) return ParseFunctionDeclaration();
            else if (_currentToken.Type == TokenType.Return) return ParseReturnStatement();
            else
            {
                var expr = ParseExpression();
                Eat(TokenType.Semicolon);
                return new ExpressionStatement(expr);
            }
        }

        public Block ParseBlock()
        {
            Eat(TokenType.LeftBrace);
            List<Statement> statements = new List<Statement>();
            while(_currentToken.Type != TokenType.RightBrace && _currentToken.Type != TokenType.EOF)
            {
                statements.Add(ParseStatement());
            }
            Eat(TokenType.RightBrace);
            return new Block(statements);
        }

        public Statement ParseIfStatement()
        {
            Eat(TokenType.If);
            Eat(TokenType.LeftParen);
            var condition = ParseExpression();
            Eat(TokenType.RightParen);
            var thenBranch = ParseStatement();
            Statement? elseBranch = null;
            if(_currentToken.Type == TokenType.Else)
            {
                Eat(TokenType.Else);
                elseBranch = ParseStatement();
            }
            return new IfStatement(condition, thenBranch, elseBranch);
        }

        public Statement ParseWhileStatement()
        {
            Eat(TokenType.While);
            Eat(TokenType.LeftParen);
            var condition = ParseExpression();
            Eat(TokenType.RightParen);
            var body = ParseStatement();
            return new WhileStatement(condition, body);
        }

        public Statement ParseForStatement()
        {
            Eat(TokenType.For);
            Eat(TokenType.LeftParen);
            Statement? initializer;
            if (_currentToken.Type != TokenType.Semicolon) initializer = ParseStatement();
            else
            {
                initializer = null;
                Eat(TokenType.Semicolon);
            }
            Expression? condition = null;
            if (_currentToken.Type != TokenType.Semicolon) condition = ParseExpression();
            Eat(TokenType.Semicolon);
            Statement? increment = null;
            if (_currentToken.Type != TokenType.RightParen)
            {
                if (_currentToken.Type == TokenType.Identifier)
                {
                    string identifier = _currentToken.Value;
                    Eat(TokenType.Identifier);
                    Eat(TokenType.Assign);
                    var expr = ParseExpression();
                    increment = new Assignment(identifier, expr);
                }
                else increment = ParseStatement();
            }
            Eat(TokenType.RightParen);
            var body = ParseStatement();
            return new ForStatement(initializer, condition, increment, body);
        }

        private FunctionDeclaration ParseFunctionDeclaration()
        {
            Eat(TokenType.Function);
            string functionName = _currentToken.Value;
            Eat(TokenType.Identifier);
            Eat(TokenType.LeftParen);
            List<string> parameters = new List<string>();
            if(_currentToken.Type != TokenType.RightParen)
            {
                parameters.Add(_currentToken.Value);
                Eat(TokenType.Identifier);
                while(_currentToken.Type == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                    parameters.Add(_currentToken.Value);
                    Eat(TokenType.Identifier);
                }
            }
            Eat(TokenType.RightParen);
            var body = ParseBlock();
            return new FunctionDeclaration(functionName, parameters, body);
        }

        private ReturnStatement ParseReturnStatement()
        {
            Eat(TokenType.Return);
            var expr = ParseExpression();
            Eat(TokenType.Semicolon);
            return new ReturnStatement(expr);
        }

        private Statement ParseVariableDeclaration()
        {
            Eat(TokenType.Set);
            if (_currentToken.Type != TokenType.Identifier) throw new ParsingException("Expected identifier after 'set'");
            string variableName = _currentToken.Value;
            Eat(TokenType.Identifier);
            var typeAnnotation = ParseOptionalTypeAnnotation();
            Eat(TokenType.Assign);
            Expression value = ParseExpression();
            Eat(TokenType.Semicolon);
            return new VariableDeclaration(variableName, value, typeAnnotation);
        }

        private Statement ParseConstantDeclaration()
        {
            Eat(TokenType.Bind);
            if (_currentToken.Type != TokenType.Identifier) throw new ParsingException("Expected identifier after 'bind'");
            string constantName = _currentToken.Value;
            Eat(TokenType.Identifier);
            var typeAnnotation = ParseOptionalTypeAnnotation();
            Eat(TokenType.Assign);
            Expression value = ParseExpression();
            Eat(TokenType.Semicolon);
            return new ConstantDeclaration(constantName, value, typeAnnotation);
        }

        private TypeAnnotation? ParseOptionalTypeAnnotation()
        {
            if (_currentToken.Type == TokenType.Colon)
            {
                Eat(TokenType.Colon);
                switch(_currentToken.Type)
                {
                    case TokenType.TypeInt:
                        Eat(TokenType.TypeInt);
                        return new TypeAnnotation("int", TokenType.TypeInt);
                    case TokenType.TypeFloat:
                        Eat(TokenType.TypeFloat);
                        return new TypeAnnotation("float", TokenType.TypeFloat);
                    case TokenType.TypeBool:
                        Eat(TokenType.TypeBool);
                        return new TypeAnnotation("bool", TokenType.TypeBool);
                    case TokenType.TypeString:
                        Eat(TokenType.TypeString);
                        return new TypeAnnotation("string", TokenType.TypeString);
                    default:
                        throw new ParsingException($"Unexpected token {_currentToken.Value} after ':'. Expected an already know type");
                }
            }
            return null;
        }

        // Pour CompilerFacade
        public List<Statement> ParseProgram()
        {
            var statements = new List<Statement>();
            while(_currentToken.Type != TokenType.EOF)
            {
                statements.Add(ParseStatement());
            }
            return statements;
        }
    }
}
