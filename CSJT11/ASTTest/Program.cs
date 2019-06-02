using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSJT11.AST;

namespace ASTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAST();
            Console.ReadKey();
        }

        static void TestAST()
        {
            CompilationUnit root = new CompilationUnit
              ( 
                null,
                new List<Declaration>()
                {
                  new ClassDeclaration
                  (
                    new List<ClassModifier>() { ClassModifier.Public },
                       "HelloWorld",
                        new List<Declaration>()
                        {
                            new MethodDeclaration
                            (
                                new List<MethodModifier>() { MethodModifier.Public, MethodModifier.Static },
                                new NameType("void"),
                                "main",
                                new List<FormalParameter>()
                                {
                                    new FormalParameter(new ArrayType(new NameType ("String")), "args")

                                },
                                new BlockStatements(
                                    new List<Statement>()
                                    {
                                        // Localvariabledeclarationstatement
                                        new LocalVariableDeclarationStatement
                                        (
                                            new LocalVariableDeclaration(new NameType("int"), new List<string> () {"x"} )
                                        ),
                                        // expressionStatement
                                        new ExpressionStatement
                                        (
                                            new AssignmentExpression(
                                                new ExpressionName("x"),
                                                new Literal(42)
                                            )
                                        )
                                    }
                                )
                           )

                       }
                  )

                }

            );
            root.DumpValue(0);
        }
    }
}
