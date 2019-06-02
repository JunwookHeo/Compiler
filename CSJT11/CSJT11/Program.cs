using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CSJT11.AST;

namespace CSJT11
{
   
    class Program
    {
        static void SemanticAnalysis(Node root)
        {
            //root.DumpValue(0);
            root.ResolveNames(null);
            root.TypeCheck();
        }

        static void CodeGeneration(string inputfile, Node root)
        {
            string outputfile = inputfile + ".il";
            using (StreamWriter of = new StreamWriter(outputfile))
            {
                string name = Path.GetFileNameWithoutExtension(inputfile);
                root.GenCode(of, name, 0);
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0) args =  new string[1] { "../../test/test.java" } ;

            Scanner scanner = new Scanner(
                new FileStream(args[0], FileMode.Open));
            Parser parser = new Parser(scanner);
            parser.Parse();

            if(Parser.Root != null)
            {
                SemanticAnalysis(Parser.Root);
                CodeGeneration(args[0], Parser.Root);
            }

            Console.WriteLine("......End Parsing!!!"); 
            Console.ReadKey();
        }

    }
}
