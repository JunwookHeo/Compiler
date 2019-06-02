using System;
using System.IO;
using System.Collections.Generic;

namespace CSJT11.AST
{
    public enum ClassModifier
    {
        Public,
        Private,
        Static
    }

    public class ClassDeclaration : Declaration
    {
        private LexicalScope symboltable;

        private List<ClassModifier> classModifiers;
        private String className;
        private List<Declaration> declarations;

        public ClassDeclaration(List<ClassModifier> classModifiers, string className, List<Declaration> declarations)
        {
            this.classModifiers = classModifiers;
            this.className = className;
            this.declarations = declarations;
        }

        public override void AddToSymbolTable(LexicalScope scope)
        {
            scope.Add(className, this);
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.AddToSymbolTable(scope);
            this.symboltable = new LexicalScope(scope);

            foreach (var declaration in this.declarations)
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

        public override Type GetValType()
        {
            return null;
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            Emit(of, level, ".class ");
            foreach (ClassModifier cm in this.classModifiers)
            {
                Emit(of, 0, cm.ToString().ToLower() + " ");
                
            }
            EmitLine(of, 0, this.className);
            EmitLine(of, level, "{{");
            foreach (Declaration declaration in this.declarations)
            {
                declaration.GenCode(of, "", level+1);
            }
            EmitLine(of, level, "}}");
        }
    }
}
