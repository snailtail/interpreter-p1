using System.Text;

public class Lox
{
    public static bool hadError = false;
    public static void Main(string[] args)
    {
        if(args.Length > 1)
        {
            Console.WriteLine("Usage: jlox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1) {
            runFile(args[0]);
        }
        else
        {
            runPrompt();
        }
    }

    private static void runFile(String path)
    {
        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        run(Encoding.Default.GetString(bytes));
        
        //Indicate an error in the exit code.
        if(hadError) Environment.Exit(65);
    }

    private static void runPrompt() {
        

        for(;;)
        {
            Console.WriteLine("> ");
            string? line = Console.ReadLine();
            if (line == null) break;
            run(line);
            hadError=false;
        }
    }

    private static void run(String source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        // For now, just print the tokens.
        foreach(Token token in tokens) {
            Console.WriteLine(token);
        }
    }

    public static void error(int line, String message) {
        report(line, "", message);
    }

    private static void report(int line, String where, String message) {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }
}

