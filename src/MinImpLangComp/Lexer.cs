using System;
using System.Collections.Generic;
using System.Text;

namespace MinImpLangComp
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
        }

        public Token GetNextToken()
        {
            SkipWhiteSpace();
            if (_position >= _input.Length) return new Token(TokenType.EOF, string.Empty);
            char current = _input[_position];
            if (char.IsDigit(current) || current == '.') return Number();
            if (char.IsLetter(current)) return Identifier();

            switch (current)
            {
                case '+':
                    _position++;
                    return new Token(TokenType.Plus, "+");
                case '-':
                    _position++;
                    return new Token(TokenType.Minus, "-");
                case '*':
                    _position++;
                    return new Token(TokenType.Multiply, "*");
                case '/':
                    _position++;
                    return new Token(TokenType.Divide, "/");
                case '=':
                    _position++;
                    return new Token(TokenType.Assign, "=");
                case ';':
                    _position++;
                    return new Token(TokenType.Semicolon, ";");
                case '.':
                    _position++;
                    return new Token(TokenType.Dot, ".");
                case ',':
                    _position++;
                    return new Token(TokenType.Comma, ",");
                case '(':
                    _position++;
                    return new Token(TokenType.LeftParen, "(");
                case ')':
                    _position++;
                    return new Token(TokenType.RightParen, ")");
                case '{':
                    _position++;
                    return new Token(TokenType.LeftBrace, "{");
                case '}':
                    _position++;
                    return new Token(TokenType.RightBrace, "}");
                default:
                    _position++;
                    return new Token(TokenType.Unknow, current.ToString());
            }
        }

        private Token Number()
        {
            int start = _position;
            bool hasDot = false;
            bool isInvalid = false;

            if (_input[_position] == '.')
            {
                hasDot = true;
                _position++;
                if(_position >= _input.Length || !char.IsDigit(_input[_position]))
                {
                    return new Token(TokenType.Dot, ".");
                }
            }

            while(_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == '.' ))
            {
                if (_input[_position] == '.')
                {
                    if (hasDot) //error = second dot
                    {
                        isInvalid = true; 
                    }
                    hasDot = true;
                }
                _position++;
            }
            string value = _input.Substring(start, _position - start);
            if (isInvalid) return new Token(TokenType.LexicalError, value);
            else if (hasDot) return new Token(TokenType.Float, value);
            else return new Token(TokenType.Integer, value);
        }

        private Token Identifier()
        {
            int start = _position;
            while(_position < _input.Length && char.IsLetterOrDigit(_input[_position]))
            {
                _position++;
            }
            string value = _input.Substring(start, _position - start);
            return new Token(TokenType.Identifier, value);
        }

        private void SkipWhiteSpace()
        {
            while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
            {
                _position++;
            }
        }
    }
}
