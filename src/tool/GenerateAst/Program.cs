namespace tool.GenerateAst;

public class Program
{
    public static void Main(string[] args)
    {
        if(args.Length!=1)
        {
            Console.WriteLine("Usage: GenerateAst <output directory>");
            System.Environment.Exit(64);
        }
        string outputDir = args[0];

        defineAst(outputDir, "Expr", new List<string>()
        {
              "Binary   : Expr left, Token operator, Expr right",
              "Grouping : Expr expression",
              "Literal  : Object value",
              "Unary    : Token operator, Expr right"
        });

    }

    private static void defineAst(string outputDir, string baseName, List<string> types)
    {
        string path = Path.Combine(outputDir, baseName + ".cs");

        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
        {
            writer.WriteLine("abstract class " + baseName + "{");
            writer.WriteLine();
            foreach (string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields= type.Split(':')[1].Trim();
                defineType(writer, baseName, className, fields);
                writer.WriteLine();
            }

            writer.WriteLine("}");
        }
    }

    private static void defineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("    static class " + className + " : " + baseName + " {");


        // Fields
        string[] fields = fieldList.Split(", ");
        
        writer.WriteLine();
        foreach(string field in fields)
        {
            writer.WriteLine("        readonly " + field + ";");
        }
        writer.WriteLine();
        // Constructor
        writer.WriteLine("        public " + className + "(" + fieldList + ") {");
        
        // Store parameters in fields.
        foreach (string field in fields)
        {
            string name = field.Split(" ")[1];
            writer.WriteLine("            this." + name + " = " + name + ";");
        }

        writer.WriteLine("        }");


        writer.WriteLine("    }");
    }
}