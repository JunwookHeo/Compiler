using System;
using System.IO;
using System.Collections.Generic;

namespace CSJT11.AST
{
    public class CompilationUnit : Node
    {
        private LexicalScope symboltable;
        private List<string> packageDeclarations;
        private List<Declaration> declarations;

        public CompilationUnit(List<string> packageDeclarations, List<Declaration> declarations)
        {
            this.packageDeclarations = packageDeclarations;
            this.declarations = declarations;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.symboltable = new LexicalScope(scope);

            foreach (Declaration declaration in this.declarations)
            {
                declaration.ResolveNames(this.symboltable);
            }
        }

        public override void TypeCheck()
        {
            foreach (Declaration declaration in this.declarations)
            {
                declaration.TypeCheck();
            }
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            EmitLine(of, level, ".assembly " + genOption);
            EmitLine(of, level, "{{");
            EmitLine(of, level, "}}");
            foreach (Declaration declaration in this.declarations)
            {
                declaration.GenCode(of, "", level) ;
            }
        }

    }
}
