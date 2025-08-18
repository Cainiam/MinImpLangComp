using System.Text;

namespace MinImpLangComp.Lexing
{
    /// <summary>
    /// Simple, single-pass, pull-based lexer that tokenizes input source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The lexer exposer a single public API: <see cref="GetNextToken"/>. Each call advances the internal cursor and returns the next <see cref="Token"/> until <see cref="TokenType.EOF"/> is produced.
    /// </para>
    /// <para>
    /// Error handling is non-throwing by design: malformed numeric literals or unterminated strings are emitted as <see cref="TokenType.LexicalError"/> tokens.
    /// </para>
    /// </remarks>
    public class Lexer
    {
        private readonly string _input;
        private readonly int _lenght;
        private int _position;

        /// <summary>
        /// Exact-case keywords mapping
        /// </summary>
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            ["set"] = TokenType.Set,
            ["bind"] = TokenType.Bind,
            ["if"] = TokenType.If,
            ["else"] = TokenType.Else,
            ["while"] = TokenType.While,
            ["for"] = TokenType.For,
            ["break"] = TokenType.Break,
            ["continue"] = TokenType.Continue,
            ["true"] = TokenType.True,
            ["false"] = TokenType.False,
            ["function"] = TokenType.Function,
            ["return"] = TokenType.Return,
            ["null"] = TokenType.Null,
            ["int"] = TokenType.TypeInt,
            ["float"] = TokenType.TypeFloat,
            ["bool"] = TokenType.TypeBool,
            ["string"] = TokenType.TypeString
        };
        
        /// <summary>
        /// Initializes a new <see cref="Lexer"/> over the given source
        /// </summary>
        /// <param name="input">Source code to tokenise. If <c>null</c>, an empty string is used.</param>
        public Lexer(string input)
        {
            _input = input;
            _lenght = _input.Length;
            _position = 0;
        }

        /// <summary>
        /// Produces the next token from input, advancing the internal cursor.
        /// </summary>
        /// <returns>The next <see cref="Token"/> or <see cref="TokenType.EOF"/> when the end is reached.</returns>
        /// <remarks>
        /// Whitespace is skipped. Numbers (including leading dot) are parsed by <see cref="Number"/>
        /// Identifier/keywords are parsed by <see cref="Identifier"/>
        /// String literals by <see cref="ReadString"/>
        /// Punctuation/operators are handled inline. Unknown signle characters yield <see cref="TokenType.Unknow"/>
        /// </remarks>
        public Token GetNextToken()
        {
            SkipWhiteSpace();
            if (IsAtEnd) return new Token(TokenType.EOF, string.Empty);
            char current = Current;

            // Number (including leading '.')
            if (char.IsDigit(current) || current == '.') return Number();

            //Identifier / keywords
            if (char.IsLetter(current)) return Identifier();

            // Strings
            if (current == '"') return ReadString(); 

            // Punctuation / operators
            switch (current)
            {
                case ':':
                    Advance();
                    return new Token(TokenType.Colon, ":");
                case '+':
                    Advance();
                    if (Match('+')) return new Token(TokenType.PlusPlus, "++");
                    return new Token(TokenType.Plus, "+");
                case '-':
                    Advance();
                    if(Match('-')) return new Token(TokenType.MinusMinus, "--");
                    return new Token(TokenType.Minus, "-");
                case '*':
                    Advance();
                    return new Token(TokenType.Multiply, "*");
                case '/':
                    Advance();
                    return new Token(TokenType.Divide, "/");
                case '%':
                    Advance();
                    return new Token(TokenType.Modulo, "%");
                case '=':
                    Advance();
                    if(Match('=')) return new Token(TokenType.Equalequal, "==");
                    return new Token(TokenType.Assign, "=");
                case ';':
                    Advance();
                    return new Token(TokenType.Semicolon, ";");
                case ',':
                    Advance();
                    return new Token(TokenType.Comma, ",");
                case '(':
                    Advance();
                    return new Token(TokenType.LeftParen, "(");
                case ')':
                    Advance();
                    return new Token(TokenType.RightParen, ")");
                case '{':
                    Advance();
                    return new Token(TokenType.LeftBrace, "{");
                case '}':
                    Advance();
                    return new Token(TokenType.RightBrace, "}");
                case '[':
                    Advance();
                    return new Token(TokenType.LeftBracket, "[");
                case ']':
                    Advance();
                    return new Token(TokenType.RightBracket, "]");
                case '<':
                    Advance();
                    if(Match('=')) return new Token(TokenType.LessEqual, "<=");
                    return new Token(TokenType.Less, "<");
                case '>':
                    Advance();
                    if (Match('=')) return new Token(TokenType.GreaterEqual, ">=");
                    return new Token(TokenType.Greater, ">");
                case '!':
                    Advance();
                    if (Match('=')) return new Token(TokenType.NotEqual, "!=");
                    return new Token(TokenType.Not, "!");
                case '&':
                    Advance();
                    if (Match('&')) return new Token(TokenType.AndAnd, "&&");
                    return new Token(TokenType.BitwiseAnd, "&");
                case '|':
                    Advance();
                    if (Match('|')) return new Token(TokenType.OrOr, "||");
                    return new Token(TokenType.BitwiseOr, "|");
                default:
                    char unknown = Advance();
                    return new Token(TokenType.Unknow, unknown.ToString());
            }
        }

        // =============================================================================================================
        // HELPERS :
        // =============================================================================================================

        /// <summary>
        /// Gets whether lexer has reached the end of input.
        /// </summary>
        private bool IsAtEnd => _position >= _lenght;

        /// <summary>
        /// Gets the current character.
        /// </summary>
        private char Current => _input[_position];

        /// <summary>
        /// Advances the cursor by one character and returns it.
        /// </summary>
        /// <returns>The character at previous position</returns>
        private char Advance() => _input[_position++];

        /// <summary>
        /// If the next character equals <paramref name="expected"/>, consumes it and returns <c>true</c>.
        /// Otherwise returns <c>false</c> without advancing.
        /// </summary>
        /// <param name="expected">Exepected char to match</param>
        /// <returns><c>true</c> if match, <c>false</c> otherwise.</returns>
        private bool Match(char expected)
        {
            if (!IsAtEnd && _input[_position] == expected)
            {
                _position++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skips any whitespace characters.
        /// </summary>
        private void SkipWhiteSpace()
        {
            while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
            {
                _position++;
            }
        }

        // =============================================================================================================
        // SCANNERS :
        // =============================================================================================================

        /// <summary>
        /// Scans an integer or floating-point literaln or a single dot.
        /// </summary>
        /// <returns><see cref="TokenType.Integer"/> or <see cref="TokenType.Float"/>; 
        /// if only a signle dot is read, returns <see cref="TokenType.Dot"/>; 
        /// if multiple dots are found within the same run, returns <see cref="TokenType.LexicalError"/></returns>
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

        /// <summary>
        /// Scans an identifier or a reserved keyword.
        /// </summary>
        /// <returns><see cref="TokenType.Identifier"/> when not a keyword; otherwise the corresponding keyword token type.</returns>
        private Token Identifier()
        {
            int start = _position;
            while(!IsAtEnd && char.IsLetterOrDigit(Current))
            {
                _position++;
            }
            string value = _input.Substring(start, _position - start);
            if (Keywords.TryGetValue(value, out var kwType)) return new Token(kwType, value);
            return new Token(TokenType.Identifier, value);
        }

        private Token ReadString()
        {
            int start = _position;
            _position++; // Skip opening '"'
            var sb = new StringBuilder();
            while (!IsAtEnd && Current != '"')
            {
                if (Current == '\\')
                {
                    _position++;
                    if (IsAtEnd) return new Token(TokenType.LexicalError, _input.Substring(start, _position - start));
                    char esc = Current;
                    switch(esc)
                    {
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case '"':
                            sb.Append('\"');
                            break;
                        default:
                            sb.Append(esc);
                            break;
                    }
                }
                else
                {
                    sb.Append(_input[_position]);
                }
                _position++;
            }
            if (IsAtEnd) return new Token(TokenType.LexicalError, _input.Substring(start, _position - start));
            _position++; // Skip closing '"'
            return new Token(TokenType.StringLiteral, sb.ToString());
        }
    }
}
