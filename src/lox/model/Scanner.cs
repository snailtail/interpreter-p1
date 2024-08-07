public class Scanner {
    private string source;
    private List<Token> tokens = new();
    private int start = 0;
    private int current = 0;
    private int line = 1;

    
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
                Lox.error(line, "Unexpected character.");
                break;
        }
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
        string text = source.Substring(start,current-start);
        tokens.Add(new Token(type, text, literal, line));
    }
}