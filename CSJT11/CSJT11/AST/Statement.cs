using System;
using System.IO;
using System.Collections.Generic;

namespace CSJT11.AST
{
    public abstract class Statement : Node
    {
        //cannot contain value
        public abstract void SetVariable(System.Type type, LocalVariableDeclaration lvd, Type val);
    }

    public class BlockStatements : Statement
    {
        private LexicalScope symboltable;

        private List<Statement> statements;
        public BlockStatements(List<Statement> statements)
        {
            this.statements = statements;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.symboltable = new LexicalScope(scope);

            foreach (Statement statement in this.statements)
            {
                statement.ResolveNames(this.symboltable);
            }
        }

        public override void TypeCheck()
        {
            foreach (Statement statement in this.statements)
            {
                statement.TypeCheck();
            }
        }

        public override void SetVariable(System.Type type, LocalVariableDeclaration lvd, Type val)
        {
            foreach (Statement statement in this.statements)
            {
                statement.SetVariable(type, lvd, val);
            }
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            foreach (Statement st in this.statements)
            {
                st.GenCode(of, genOption, level);
            }
        }
    }

    public class ExpressionStatement : Statement
    {
        private Expression exp;
        public ExpressionStatement(Expression exp)
        {
            this.exp = exp;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.exp.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
            this.exp.TypeCheck();
        }
        public override void SetVariable(System.Type type, LocalVariableDeclaration lvd, Type val)
        {
            // Nothing
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.exp.GenCode(of, genOption, level);
        }
    }

    public class LocalVariableDeclarationStatement : Statement
    {
        private Declaration declaration;
        public LocalVariableDeclarationStatement(Declaration declaration)
        {
            this.declaration = declaration;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.declaration.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
            this.declaration.TypeCheck();
        }
        public override void SetVariable(System.Type type, LocalVariableDeclaration lvd, Type val)
        {
            // Nothing
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.declaration.GenCode(of, genOption, level);
        }
    }

    public class ReturnStatement : Statement
    {
        private Expression exp;
        private LocalVariable vt;
        public ReturnStatement(Expression exp)
        {
            this.exp = exp;
            this.vt = null;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            if (this.exp != null)
                this.exp.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
            if (this.exp != null)
                this.exp.TypeCheck();
        }
        public override void SetVariable(System.Type type, LocalVariableDeclaration lvd, Type val)
        {
            if (type == typeof(ReturnStatement) && this.exp != null)
                this.vt = lvd.AddLocalVariableForReturn(val);
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (this.exp != null && this.vt != null)
            {
                this.exp.GenCode(of, "[[load]]", level);
                Emit(of, level, "stloc ");
                this.vt.GenCode(of, "[[nameonly]]", level);
                EmitLine(of, level, "");
            }

            EmitLine(of, level, "br \t\t\t {0}_RET", genOption);
        }

        public void GenCodePost(StreamWriter of, string genOption, int level)
        {
            if (this.exp == null)
            {
                Emit(of, level - 1, "{0}_RET: ", genOption);
            }
            else if (this.vt != null)
            {
                EmitLine(of, level - 1, "{0}_RET:", genOption);
                Emit(of, level, "ldloc ", genOption);
                this.vt.GenCode(of, "[[nameonly]]", level);
                EmitLine(of, level, "");
            }
        }
    }

    public class IfThenStatement : Statement
    {
        private Expression exp;
        private Statement statement;
        private LocalVariable vt;

        public IfThenStatement(Expression exp, Statement statement)
        {
            this.exp = exp;
            this.statement = statement;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            if (this.exp != null)
                this.exp.ResolveNames(scope);
            if (this.statement != null)
                this.statement.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
            if (this.exp != null)
                this.exp.TypeCheck();
            if (this.statement != null)
                this.statement.TypeCheck();
        }

        public override void SetVariable(System.Type type, LocalVariableDeclaration lvd, Type val)
        {
            if (type == typeof(ReturnStatement) && this.statement != null)
                this.statement.SetVariable(type, lvd, val);

            if (type == typeof(IfThenStatement))
                this.vt = lvd.AddLocalVariableForIfThen(val);
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (this.exp != null)
            {
                this.exp.GenCode(of, "[[load]]", level);
            }

            Emit(of, level, "stloc ");
            this.vt.GenCode(of, "[[nameonly]]", level);
            EmitLine(of, 0, "");
            Emit(of, level, "ldloc ");
            this.vt.GenCode(of, "[[nameonly]]", level);
            EmitLine(of, 0, "");
            EmitLine(of, level, "brfalse \t {0}_IF", genOption);

            if (this.statement != null)
            {
                this.statement.GenCode(of, genOption, level);
            }
            EmitLine(of, level-1, "{0}_IF: ", genOption);
        }
    }
}

