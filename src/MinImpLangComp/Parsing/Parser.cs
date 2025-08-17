using MinImpLangComp.AST;
using MinImpLangComp.Lexing;
using MinImpLangComp.Exceptions;
using System.Globalization;

namespace MinImpLangComp.Parsing
{
    /// <summary>
    /// Recurvise-descent parser turning tokens into an AST.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The parser exposes granular methods to parser expressions and statements as well as a <see cref="ParseProgram"/> entry point used by the facade.
    /// </para>
    /// <para>
    /// Error policy: user-facing syntax errors raise <see cref="ParsingException"/>.
    /// </para>
    /// </remarks>
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

        /// <summary>
        /// Indicates whether the current lookahead token is EOF. Used by facade.
        /// </summary>
        public bool IsAtEnd => _currentToken.Type == TokenType.EOF;

        /// <summary>
        /// Initializes a new parser over a given lexer
        /// </summary>
        /// <param name="lexer">Given lexer to parse.</param>
        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        /// <summary>
        /// Consumes a toekn of the expetected <paramref name="type"/> or throws a <see cref="ParsingException"/>.
        /// </summary>
        /// <param name="type">Expected <paramref name="type"/></param>
        /// <exception cref="ParsingException">If type doesn't match</exception>
        private void Eat(TokenType type)
        {
            if (_currentToken.Type == type) _currentToken = _lexer.GetNextToken();
            else throw new ParsingException($"Unexpected token: {_currentToken.Type}, expected: {type}");
        }

        /// <summary>
        /// Returns the current token type without consuming.
        /// </summary>
        private TokenType CurrentType => _currentToken.Type;

        /// <summary>
        /// Parses an expression with precedence: additive/relationnal/logical over multiplicative.
        /// </summary>
        /// <returns></returns>
        public Expression ParseExpression()
        {
            var left = ParseTerm();

            // +, -,<, >, <=, >=, ==, !=, &&, ||, &, |
            while (TryMapAddRelLogicOperator(CurrentType, out var oper))
            {

                Eat(_currentToken.Type);
                var right = ParseTerm();
                left = new BinaryExpression(left, oper, right);
            }
            return left;
        }

        /// <summary>
        /// Parse multiplicative expressions: *, /, %.
        /// </summary>
        /// <returns><see cref="Expression"/></returns>
        private Expression ParseTerm()
        {
            var left = ParseFactor();
            while(TryMapMulOperator(CurrentType, out var oper))
            {
                Eat(_currentToken.Type);
                var right = ParseFactor();
                left = new BinaryExpression(left, oper, right);
            }
            return left;
        }


        /// <summary>
        /// Parses literals, grouped expressions, array, identifier, booleans, null and unary not.
        /// </summary>
        /// <returns><see cref="Expression"/>.</returns>
        /// <exception cref="ParsingException">Unexpected token.</exception>
        private Expression ParseFactor()
        {
            if (CurrentType == TokenType.Integer)
            {
                int value = int.Parse(_currentToken.Value);
                Eat(TokenType.Integer);
                return new IntegerLiteral(value);
            }
            else if(CurrentType == TokenType.Float)
            {
                double value = double.Parse(_currentToken.Value, CultureInfo.InvariantCulture);
                Eat(TokenType.Float);
                return new FloatLiteral(value);
            }
            else if (CurrentType == TokenType.StringLiteral)
            {
                string value = _currentToken.Value;
                Eat(TokenType.StringLiteral);
                return new StringLiteral(value);
            }
            else if (CurrentType == TokenType.LeftParen)
            {
                Eat(TokenType.LeftParen);
                var expr = ParseExpression();
                Eat(TokenType.RightParen);
                return expr;
            }
            else if (CurrentType == TokenType.LeftBracket)
            {
                Eat(TokenType.LeftBracket);
                var elements = new List<Expression>();
                if (CurrentType != TokenType.RightBracket)
                {
                    elements.Add(ParseExpression());
                    while(CurrentType == TokenType.Comma)
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
                if (CurrentType == TokenType.LeftParen)
                {
                    Eat(TokenType.LeftParen);
                    var arguments = ParseArgumentList();
                    Eat(TokenType.RightParen);
                    return new FunctionCall(name, arguments);
                }
                else if (CurrentType == TokenType.LeftBracket)
                {
                    Eat(TokenType.LeftBracket);
                    var indexExpr = ParseExpression();
                    Eat(TokenType.RightBracket);
                    return new ArrayAccess(name, indexExpr);
                }
                else return new VariableReference(name);
            }
            else if (CurrentType == TokenType.True)
            {
                Eat(TokenType.True);
                return new BooleanLiteral(true);
            }
            else if (CurrentType == TokenType.False)
            {
                Eat(TokenType.False);
                return new BooleanLiteral(false);
            }
            else if(CurrentType == TokenType.Null)
            {
                Eat(TokenType.Null);
                return new NullLiteral();
            }
            else if (CurrentType == TokenType.Not)
            {
                Eat(TokenType.Not);
                var operand = ParseFactor();
                return new UnaryNotExpression(operand);
            }
            else throw new ParsingException($"Unexpected token; {_currentToken.Type}");
        }

        /// <summary>
        /// Parses a single statement (declarations, control-flow, expression statements, ...)
        /// </summary>
        /// <returns><see cref="Statement"/>.</returns>
        /// <exception cref="ParsingException">No identifier.</exception>
        public Statement ParseStatement()
        {
            if (CurrentType == TokenType.Set) return ParseVariableDeclaration();
            else if (CurrentType == TokenType.Bind) return ParseConstantDeclaration();
            else if (CurrentType == TokenType.Break)
            {
                Eat(TokenType.Break);
                Eat(TokenType.Semicolon);
                return new BreakStatement();
            }
            else if (CurrentType == TokenType.Continue)
            {
                Eat(TokenType.Continue);
                Eat(TokenType.Semicolon);
                return new ContinueStatement();
            }
            else if (CurrentType == TokenType.Identifier)
            {
                string identifier = _currentToken.Value;
                Eat(TokenType.Identifier);
                if (CurrentType == TokenType.LeftBracket)
                {
                    Eat(TokenType.LeftBracket);
                    var indexEpr = ParseExpression();
                    Eat(TokenType.RightBracket);
                    if (CurrentType == TokenType.Assign)
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
                else if (CurrentType == TokenType.LeftParen)
                {
                    Eat(TokenType.LeftParen);
                    var arguments = ParseArgumentList();
                    Eat(TokenType.RightParen);
                    Eat(TokenType.Semicolon);
                    return new ExpressionStatement(new FunctionCall(identifier, arguments));
                }
                else if (CurrentType == TokenType.Assign)
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
            else if (CurrentType == TokenType.LeftBrace) return ParseBlock();
            else if (CurrentType == TokenType.If) return ParseIfStatement();
            else if (CurrentType == TokenType.While) return ParseWhileStatement();
            else if (CurrentType == TokenType.For) return ParseForStatement();
            else if (CurrentType == TokenType.PlusPlus || CurrentType == TokenType.MinusMinus)
            {
                OperatorType oper = _currentToken.Type == TokenType.PlusPlus ? OperatorType.PlusPlus : OperatorType.MinusMinus;
                Eat(CurrentType);
                if (CurrentType != TokenType.Identifier) throw new ParsingException($"Expected identifier after {oper}");
                string identifier = _currentToken.Value;
                Eat(TokenType.Identifier);
                Eat(TokenType.Semicolon);
                return new ExpressionStatement(new UnaryExpression(oper, identifier));
            }
            else if (CurrentType == TokenType.Function) return ParseFunctionDeclaration();
            else if (CurrentType == TokenType.Return) return ParseReturnStatement();
            else
            {
                var expr = ParseExpression();
                Eat(TokenType.Semicolon);
                return new ExpressionStatement(expr);
            }
        }

        /// <summary>
        /// Parses a block delimited by braces, containing zero or more statements.
        /// </summary>
        /// <returns><see cref="Block"/>.</returns>
        public Block ParseBlock()
        {
            Eat(TokenType.LeftBrace);
            var statements = new List<Statement>();
            while(CurrentType != TokenType.RightBrace && CurrentType != TokenType.EOF)
            {
                statements.Add(ParseStatement());
            }
            Eat(TokenType.RightBrace);
            return new Block(statements);
        }

        /// <summary>
        /// Parses an if/else statement.
        /// </summary>
        /// <returns><see cref="Statement"/>.</returns>
        public Statement ParseIfStatement()
        {
            Eat(TokenType.If);
            Eat(TokenType.LeftParen);
            var condition = ParseExpression();
            Eat(TokenType.RightParen);
            var thenBranch = ParseStatement();
            Statement? elseBranch = null;
            if(CurrentType == TokenType.Else)
            {
                Eat(TokenType.Else);
                elseBranch = ParseStatement();
            }
            return new IfStatement(condition, thenBranch, elseBranch);
        }

        /// <summary>
        /// Parses a while-loop.
        /// </summary>
        /// <returns><see cref="Statement"/>.</returns>
        public Statement ParseWhileStatement()
        {
            Eat(TokenType.While);
            Eat(TokenType.LeftParen);
            var condition = ParseExpression();
            Eat(TokenType.RightParen);
            var body = ParseStatement();
            return new WhileStatement(condition, body);
        }

        /// <summary>
        /// Parses a for-loop with optional initializer/condition/increment segments.
        /// </summary>
        /// <returns><see cref="Statement"/>.</returns>
        public Statement ParseForStatement()
        {
            Eat(TokenType.For);
            Eat(TokenType.LeftParen);
            Statement? initializer;
            if (CurrentType != TokenType.Semicolon) initializer = ParseStatement();
            else
            {
                initializer = null;
                Eat(TokenType.Semicolon);
            }
            Expression? condition = null;
            if (CurrentType != TokenType.Semicolon) condition = ParseExpression();
            Eat(TokenType.Semicolon);
            Statement? increment = null;
            if (CurrentType != TokenType.RightParen)
            {
                if (CurrentType == TokenType.Identifier)
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

        /// <summary>
        /// Parses a function declaration with a parameter list and a block body.
        /// </summary>
        /// <returns><see cref="FunctionDeclaration"/>.</returns>
        private FunctionDeclaration ParseFunctionDeclaration()
        {
            Eat(TokenType.Function);
            string functionName = _currentToken.Value;
            Eat(TokenType.Identifier);
            Eat(TokenType.LeftParen);
            var parameters = new List<string>();
            if(CurrentType != TokenType.RightParen)
            {
                parameters.Add(_currentToken.Value);
                Eat(TokenType.Identifier);
                while(CurrentType == TokenType.Comma)
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

        /// <summary>
        /// Parses a return statement with an expression.
        /// </summary>
        /// <returns><see cref="ReturnStatement"/>.</returns>
        private ReturnStatement ParseReturnStatement()
        {
            Eat(TokenType.Return);
            var expr = ParseExpression();
            Eat(TokenType.Semicolon);
            return new ReturnStatement(expr);
        }

        /// <summary>
        /// Parses a variable declaration (set &lt;id&gt; [:type] = expr;).
        /// </summary>
        /// <returns><see cref="Statement"/>.</returns>
        /// <exception cref="ParsingException">Identifier needed after 'set'.</exception>
        private Statement ParseVariableDeclaration()
        {
            Eat(TokenType.Set);
            if (CurrentType != TokenType.Identifier) throw new ParsingException("Expected identifier after 'set'");
            string variableName = _currentToken.Value;
            Eat(TokenType.Identifier);
            var typeAnnotation = ParseOptionalTypeAnnotation();
            Eat(TokenType.Assign);
            Expression value = ParseExpression();
            Eat(TokenType.Semicolon);
            return new VariableDeclaration(variableName, value, typeAnnotation);
        }

        /// <summary>
        /// Parses a constant declaration (bind &lt;id&gt; [:type] = expr;)
        /// </summary>
        /// <returns><see cref="Statement"/>.</returns>
        /// <exception cref="ParsingException">Identifier needed after 'bind'.</exception>
        private Statement ParseConstantDeclaration()
        {
            Eat(TokenType.Bind);
            if (CurrentType != TokenType.Identifier) throw new ParsingException("Expected identifier after 'bind'");
            string constantName = _currentToken.Value;
            Eat(TokenType.Identifier);
            var typeAnnotation = ParseOptionalTypeAnnotation();
            Eat(TokenType.Assign);
            Expression value = ParseExpression();
            Eat(TokenType.Semicolon);
            return new ConstantDeclaration(constantName, value, typeAnnotation);
        }

        /// <summary>
        /// Parses an optional type annotation (<c>: int|float|bool|string</c>).
        /// </summary>
        /// <returns><see cref="TypeAnnotation"/></returns>
        /// <exception cref="ParsingException">Unexpected token given.</exception>
        private TypeAnnotation? ParseOptionalTypeAnnotation()
        {
            if (CurrentType == TokenType.Colon)
            {
                Eat(TokenType.Colon);
                switch(CurrentType)
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

        /// <summary>
        /// Parses an entire program until EOF (used by the facade).
        /// </summary>
        /// <returns><see cref="List{T}"/> of <see cref="Statement"/>.</returns>
        public List<Statement> ParseProgram()
        {
            var statements = new List<Statement>();
            while(_currentToken.Type != TokenType.EOF)
            {
                statements.Add(ParseStatement());
            }
            return statements;
        }

        // =============================================================================================================
        // HELPER :
        // =============================================================================================================

        /// <summary>
        /// Parses a comma-separated argument list (possibly empty) ending whi RightParen. Caller must have consumed the LeftParen.
        /// </summary>
        /// <returns><see cref="List{T}"/> of <see cref="Expression"/>.</returns>
        private List<Expression> ParseArgumentList()
        {
            var args = new List<Expression>();
            if(CurrentType != TokenType.RightParen)
            {
                args.Add(ParseExpression());
                while(CurrentType == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                    args.Add(ParseExpression());
                }
            }
            return args;
        }

        /// <summary>
        /// Maps additive/relational/logical/bitwise tokens to <see cref="OperatorType"/>.
        /// </summary>
        /// <param name="t">A TokenType</param>
        /// <param name="oper">Out value</param>
        /// <returns><see cref="bool"/> true if known logic operator, if not false.</returns>
        private static bool TryMapAddRelLogicOperator(TokenType t, out OperatorType oper)
        {
            switch(t)
            {
                case TokenType.Plus:
                    oper = OperatorType.Plus;
                    return true;
                case TokenType.Minus:
                    oper = OperatorType.Minus;
                    return true;
                case TokenType.Less:
                    oper = OperatorType.Less;
                    return true;
                case TokenType.Greater:
                    oper = OperatorType.Greater;
                    return true;
                case TokenType.LessEqual:
                    oper = OperatorType.LessEqual;
                    return true;
                case TokenType.GreaterEqual:
                    oper = OperatorType.GreaterEqual;
                    return true;
                case TokenType.Equalequal:
                    oper = OperatorType.Equalequal;
                    return true;
                case TokenType.NotEqual:
                    oper = OperatorType.NotEqual;
                    return true;
                case TokenType.AndAnd:
                    oper = OperatorType.AndAnd;
                    return true;
                case TokenType.OrOr:
                    oper = OperatorType.OrOr;
                    return true;
                case TokenType.BitwiseAnd:
                    oper = OperatorType.BitwiseAnd;
                    return true;
                case TokenType.BitwiseOr:
                    oper = OperatorType.BitwiseOr;
                    return true;
                default:
                    oper = default;
                    return false;
            }
        }

        /// <summary>
        /// Maps multiplicative tokens to <see cref="OperatorType"/>.
        /// </summary>
        /// <param name="t">A TokenType</param>
        /// <param name="oper">Out value</param>
        /// <returns><see cref="bool"/> true if known multiplicative token, false if not.</returns>
        private static bool TryMapMulOperator(TokenType t, out OperatorType oper)
        {
            switch(t)
            {
                case TokenType.Multiply:
                    oper = OperatorType.Multiply;
                    return true;
                case TokenType.Divide:
                    oper = OperatorType.Divide;
                    return true;
                case TokenType.Modulo:
                    oper = OperatorType.Modulo;
                    return true;
                default:
                    oper = default;
                    return false;
            }
        }
    }
}
