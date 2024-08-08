using System.Data;
using System.Data.Common;
using System.Globalization;

public class Scanner {
    private string source;
    private List<Token> tokens = new();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private static readonly Dictionary<string,TokenType> keywords = new(){
        {"and",TokenType.AND},
        {"class", TokenType.CLASS},
        {"else",TokenType.ELSE},
        {"false",TokenType.FALSE},
        {"for",TokenType.FOR},
        {"fun",TokenType.FUN},
        {"if",TokenType.IF},
        {"nil",TokenType.NIL},
        {"or",TokenType.OR},
        {"print",TokenType.PRINT},
        {"return",TokenType.RETURN},
        {"super",TokenType.SUPER},
        {"this",TokenType.THIS},
        {"true",TokenType.TRUE},
        {"var",TokenType.VAR},
        {"while",TokenType.WHILE}
    };
    
    public Scanner(String source) {
        this.source = source;
    }

    internal List<Token> scanTokens() {
        while (!isAtEnd()) {
            // We are at the beginning of the next lexeme.
            start = current;
            scanToken();
        }
        tokens.Add(new Token(TokenType.EOF,"", null, line));
        return tokens;
    }

    private void scanToken() {
        char c = advance();
        switch (c) {
            case '(' : addToken(TokenType.LEFT_PAREN); break;
            case ')' : addToken(TokenType.RIGHT_PAREN); break;
            case '{' : addToken(TokenType.LEFT_BRACE); break;
            case '}' : addToken(TokenType.RIGHT_BRACE); break;
            case ',' : addToken(TokenType.COMMA); break;
            case '.' : addToken(TokenType.DOT); break;
            case '-' : addToken(TokenType.MINUS); break;
            case '+' : addToken(TokenType.PLUS); break;
            case ':' : addToken(TokenType.SEMICOLON); break;
            case '*' : addToken(TokenType.STAR); break;
            case '!' :
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=' :
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<' :
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>' :
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            
            case '/' :
                if (match('/')) {
                    // a comment goes until the end of the line.
                    while(peek() != '\n' && !isAtEnd()) advance();
                }
                else if (peek()=='*') {
                    //multiline comment
                    Console.WriteLine("Multiline comment started!");
                    scanMultiLineComment();
                }
                else {
                    addToken(TokenType.SLASH);
                }
                break;

            case ' ' :
            case '\r' :
            case '\t' :
                // Ignore whitespace
                break;
            
            case '\n' :
                line++;
                break;
            
            case '"' : parsestring(); break;

            default: 
                if(isDigit(c))
                {
                    number();
                }
                else if (isAlpha(c))
                {
                    identifier();
                }
                else 
                {
                    Lox.error(line, "Unexpected character.");
                }
                break;
        }
    }

    private void identifier()
    {
        TokenType type;

        while(isAlphaNumeric(peek()))
        {
            advance();
        }
        string text = source[start..current];
        var filteredKeywords = keywords.Where(k=> k.Key.ToLower() == text.ToLower());
        if (filteredKeywords.Count() > 0)
        {
            type = filteredKeywords.Select(f=> f.Value).First();
        }
        else
        {
            type=TokenType.IDENTIFIER;
        }
                
        addToken((TokenType)type,text);
    }

    private void scanMultiLineComment()
    {
        do
        {
            advance();
        }
        while (!(peek() == '*' && peekNext() == '/'));
        advance();
        advance();
        string value = source[start..current];
        addToken(TokenType.MULTILINECOMMENT,value);
    }

    private void number()
    {
        while(isDigit(peek()))
        {
            advance();
        }

        // Look for a fractional part.
        if (peek() == '.' && isDigit(peekNext()))
        {
            // Consume the .
            advance();
            while(isDigit(peek()))
            {
                advance();
            }
        }
        string doubleToParse = source[start..current];
        var dbl = Double.Parse(doubleToParse,NumberStyles.Float, CultureInfo.InvariantCulture);
        addToken(TokenType.NUMBER,dbl);
    }



    private void parsestring() {
        while(peek() != '"' && !isAtEnd()) {
            if(peek() =='\n') {
                line++;
            }
            advance();
        }
        if(isAtEnd()){
            Lox.error(line, "Unterminated string.");
            return;
        }

        // The closing ".
        advance();

        // Trim the surrounding quotes.
        string value = source[(start+1)..(current-1)];
        addToken(TokenType.STRING,value);
    }

    private bool match(char expected) {
        if(isAtEnd()) return false;
        if (source[current] != expected) return false;
        
        current++;
        return true;
    }

    private char peek() {
        if (isAtEnd()) return '\0';
        return source[current];
    }

    private char peekNext()
    {
        if (current +1 >= source.Length) 
        {
            return '\0';
        }
        return source[current+1];
    }

    private bool isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c =='_');
    }
    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private bool isAtEnd() {
        return current >= source.Length;
    }
    
    private char advance() {
        return source[current++];
    }

    private void addToken(TokenType type) {
        addToken(type, null);
    }

    private void addToken(TokenType type, object? literal) {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }
}