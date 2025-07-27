using System.Text;

namespace MinImpLangComp.Lexing
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
            if (_input[_position] == '"') return String(); 

            switch (current)
            {
                case '+':
                    _position++;
                    if( _position < _input.Length && _input[_position] == '+')
                    {
                        _position++;
                        return new Token(TokenType.PlusPlus, "++");
                    }
                    return new Token(TokenType.Plus, "+");
                case '-':
                    _position++;
                    if (_position < _input.Length && _input[_position] == '-')
                    {
                        _position++;
                        return new Token(TokenType.MinusMinus, "--");
                    }
                    return new Token(TokenType.Minus, "-");
                case '*':
                    _position++;
                    return new Token(TokenType.Multiply, "*");
                case '/':
                    _position++;
                    return new Token(TokenType.Divide, "/");
                case '=':
                    _position++;
                    if (_position < _input.Length && _input[_position] == '=')
                    {
                        _position++;
                        return new Token(TokenType.Equalequal, "==");
                    }
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
                case '<':
                    _position++;
                    if(_position < _input.Length && _input[_position] == '=')
                    {
                        _position++;
                        return new Token(TokenType.LessEqual, "<=");
                    }
                    return new Token(TokenType.Less, "<");
                case '>':
                    _position++;
                    if(_position < _input.Length && _input[_position] == '=')
                    {
                        _position++;
                        return new Token(TokenType.GreaterEqual, ">=");
                    }
                    return new Token(TokenType.Greater, ">");
                case '!':
                    _position++;
                    if (_position < _input.Length && _input[_position] == '=')
                    {
                        _position++;
                        return new Token(TokenType.NotEqual, "!=");
                    }
                    return new Token(TokenType.Not, "!");
                case '&':
                    _position++;
                    if(_position < _input.Length && _input[_position] == '&')
                    {
                        _position++;
                        return new Token(TokenType.AndAnd, "&&");
                    }
                    return new Token(TokenType.BitwiseAnd, "&");
                case '|':
                    _position++;
                    if(_position < _input.Length && _input[_position] == '|')
                    {
                        _position++;
                        return new Token(TokenType.OrOr, "||");
                    }
                    return new Token(TokenType.BitwiseOr, "|");
                case '%':
                    _position++;
                    return new Token(TokenType.Modulo, "%");
                case '[':
                    _position++;
                    return new Token(TokenType.LeftBracket, "[");
                case ']':
                    _position++;
                    return new Token(TokenType.RightBracket, "]");
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
            if (value == ".") return new Token(TokenType.Dot, ".");
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
            
            switch (value)
            {
                case "let":
                    return new Token(TokenType.Let, value);
                case "if":
                    return new Token(TokenType.If, value);
                case "else":
                    return new Token(TokenType.Else, value);
                case "while":
                    return new Token(TokenType.While, value);
                case "for":
                    return new Token(TokenType.For, value);
                case "true":
                    return new Token(TokenType.True, value);
                case "false":
                    return new Token(TokenType.False, value);
                case "function":
                    return new Token(TokenType.Function, value);
                case "return":
                    return new Token(TokenType.Return, value);
                case "null":
                    return new Token(TokenType.Null, value);
                default:
                    return new Token(TokenType.Identifier, value);
            }
        }

        private Token String()
        {
            int start = _position;
            _position++;
            var stringB = new StringBuilder();
            while (_position < _input.Length && _input[_position] != '"')
            {
                if (_input[_position] == '\\')
                {
                    _position++;
                    if (_position >= _input.Length) return new Token(TokenType.LexicalError, _input.Substring(start, _position - start));
                    char escapeChar = _input[_position];
                    switch(escapeChar)
                    {
                        case 'n':
                            stringB.Append('\n');
                            break;
                        case 't':
                            stringB.Append('\t');
                            break;
                        case 'r':
                            stringB.Append('\r');
                            break;
                        case '\\':
                            stringB.Append('\\');
                            break;
                        case '"':
                            stringB.Append('\"');
                            break;
                        default:
                            stringB.Append(escapeChar);
                            break;
                    }
                }
                else
                {
                    stringB.Append(_input[_position]);
                }
                _position++;
            }
            if (_position >= _input.Length) return new Token(TokenType.LexicalError, _input.Substring(start, _position - start));
            _position++;
            string value = stringB.ToString();
            return new Token(TokenType.StringLiteral, value);
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
