using MinImpLangComp.AST;
using MinImpLangComp.LexerSpace;
using System;
using System.Globalization;

namespace MinImpLangComp.ParserSpace
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        private void Eat(TokenType type)
        {
            if (_currentToken.Type == type) _currentToken = _lexer.GetNextToken();
            else throw new Exception($"Unexpected token: {_currentToken.Type}, expected: {type}");
        }

        public Expression ParseExpression()
        {
            var left = ParseTerm();
            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
            {
                string oper = _currentToken.Value;
                Eat(_currentToken.Type);
                var right = ParseTerm();
                left = new BinaryExpression(left, oper, right);
            }
            return left;
        }

        private Expression ParseTerm()
        {
            var left = ParseFactor();
            while(_currentToken.Type == TokenType.Multiply || _currentToken.Type == TokenType.Divide)
            {
                string oper = _currentToken.Value;
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
            else if (_currentToken.Type == TokenType.LeftParen)
            {
                Eat(TokenType.LeftParen);
                var expr = ParseExpression();
                Eat(TokenType.RightParen);
                return expr;
            }
            else throw new Exception($"Unexpected token; {_currentToken.Type}");
        }

        public Statement ParseStatement()
        {
            if(_currentToken.Type == TokenType.Let)
            {
                Eat(TokenType.Let);
                string identifier = _currentToken.Value;
                Eat(TokenType.Identifier);
                Eat(TokenType.Assign);
                var expr = ParseExpression();
                Eat(TokenType.Semicolon);
                return new Assignment(identifier, expr);
            }
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
    }
}
