using System;
using System.IO;
using System.Collections.Generic;

namespace CSJT11.AST
{
    public abstract class Declaration : Node
    {
         public abstract void AddToSymbolTable(LexicalScope scope);
         public abstract Type GetValType();
    }

    public enum MethodModifier
    {
        Public,
        Protected,
        Private,
        Abstract,
        Static,
        Final,
        Synchronized,
        Native,
        Strictfp
    }

    public class LocalVariableDeclaration : Declaration
    {
        private List<LocalVariable> localVariables;
        bool isSetfromReturn;

        public LocalVariableDeclaration(Type type, List<string> names)
        {
            this.isSetfromReturn = false;
            localVariables = new List<LocalVariable>();
            int idx = 0;
            foreach (string n in names)
            {
                LocalVariable f = new LocalVariable(type, n, idx++);
                localVariables.Add(f);
            }
        }

        public override void AddToSymbolTable(LexicalScope scope)
        {
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.AddToSymbolTable(scope);

            foreach (LocalVariable localVariable in this.localVariables)
            {
                localVariable.ResolveNames(scope);
            }
        }

        public override void TypeCheck()
        {

        }

        public override Type GetValType()
        {
            return this.localVariables[0].GetValType();
        }

        public int GetVariableCount()
        {
            if (this.localVariables == null)
                return -1;
            return localVariables.Count;
        }

        public LocalVariable AddLocalVariableForReturn(Type type)
        {
            int order = 0;

            if (localVariables != null)
                order = localVariables.Count;
            if (this.isSetfromReturn == false)
            {
                localVariables.Add(new LocalVariable(type, String.Format("V_{0}", order), order));
                this.isSetfromReturn = true;
            }

            return localVariables[localVariables.Count - 1];
        }

        public LocalVariable AddLocalVariableForIfThen(Type type)
        {
            int order = 0;
            if (localVariables != null)
                order = localVariables.Count;
            localVariables.Add(new LocalVariable(type, String.Format("V_{0}", order), order));
            return localVariables[localVariables.Count - 1];
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            Emit(of, level, ".locals init (");
            Boolean isFirst = true;
            foreach (LocalVariable fp in this.localVariables)
            {
                if (isFirst == true)
                    isFirst = false;
                else
                    Emit(of, 0, ", ");

                fp.GenCode(of, "", level);
                
            }
            EmitLine(of, 0, ")");
        }

    }

    public class FormalParameter : Declaration
    {
        private Type type;
        private string name;

        public FormalParameter(Type type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.AddToSymbolTable(scope);

            this.type.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
        }

        public override void AddToSymbolTable(LexicalScope scope)
        {
            scope.Add(name, this);
        }

        public override Type GetValType()
        {
            return this.type;
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.type.GenCode(of, "", level);
            Emit(of, 0, this.name);
        }
    }

    public class LocalVariable : Declaration
    {
        private Type type;
        private string name;
        private int order;

        public LocalVariable(Type type, string name, int order)
        {
            this.type = type;
            this.name = name;
            this.order = order;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.AddToSymbolTable(scope);

            this.type.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
        }

        public override void AddToSymbolTable(LexicalScope scope)
        {
            scope.Add(name, this);
        }

        public override Type GetValType()
        {
            return this.type;
        }

        public int GetNameToIdx(string name)
        {
            return this.order;
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if(genOption != "[[nameonly]]")
                this.type.GenCode(of, "", level);
            Emit(of, 0, "V_{0}", this.order);
        }
    }

    public class MethodDeclaration : Declaration
    {
        private LexicalScope symboltable;

        private List<MethodModifier> methodModifiers;
        private Type result;
        private string methodName;
        private List<FormalParameter> formalParameters;
        private Statement statement;

        public MethodDeclaration(
                List<MethodModifier> methodModifiers,
                Type result,
                string methodName,
                List<FormalParameter> formalParameters,
                Statement statement)
        {
            this.methodModifiers = methodModifiers;
            this.result = result;
            this.methodName = methodName;
            this.formalParameters = formalParameters;
            this.statement = statement;

        }

        public void SetMethodModifiers(List<MethodModifier> methodModifiers)
        {
            this.methodModifiers = methodModifiers;
        }
        public void SetResult(Type result)
        {
            this.result = result;
        }
        public void SetStatement(Statement statement)
        {
            this.statement = statement;
        }

        public override void AddToSymbolTable(LexicalScope scope)
        {
            scope.Add(this.methodName, this);
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.AddToSymbolTable(scope);
            this.symboltable = new LexicalScope(scope);

            this.result.ResolveNames(this.symboltable);

            foreach (FormalParameter formalParameter in this.formalParameters)
            {
                formalParameter.ResolveNames(this.symboltable);
            }

            this.statement.ResolveNames(this.symboltable);
        }

        public override void TypeCheck()
        {
            this.statement.TypeCheck();
        }

        public override Type GetValType()
        {
            return this.result;
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            Emit(of, level, ".method ");
            foreach (MethodModifier mm in this.methodModifiers)
            {
                Emit(of, 0, mm.ToString().ToLower() + " ");
            }
            this.result.GenCode(of, "", level);
            Emit(of, 0, " ");
            Emit(of, 0, this.methodName);
            
            Emit(of, 0, "(");
            Boolean isFirst = true;
            foreach (FormalParameter fp in this.formalParameters)
            {
                if (isFirst == true)
                    isFirst = false;
                else
                    Emit(of, 0, ", ");

                fp.GenCode(of, "", level);
            }
            EmitLine(of, 0, ")");
            EmitLine(of, level, "{{");
            if(this.methodName == "main")
                EmitLine(of, level+1, ".entrypoint");

            // Check IfThenStatement and then add bool variable
            Node lvd = FindClass(typeof(LocalVariableDeclaration));
            if (lvd != null)
            {
                this.statement.SetVariable(typeof(IfThenStatement), (LocalVariableDeclaration)lvd, new PrimitiveType(UnannPrimitiveType.Boolean));
            }
            else
            {
                EmitLine(of, level + 1, ".locals init(bool V_0)");
            }

            // Check ReturnStatement, check LocalVariableDeclaration.
            // Then, if LocalVariableDeclaration, add tmp value for return
            // else, Write code for tmp value.
            if (lvd != null)
            {
                if(this.result is VoidType)
                    this.statement.SetVariable(typeof(ReturnStatement), (LocalVariableDeclaration)lvd, new PrimitiveType(UnannPrimitiveType.Int));
                else
                    this.statement.SetVariable(typeof(ReturnStatement), (LocalVariableDeclaration)lvd, this.result);

            }
            else
            {
                EmitLine(of, level + 1, ".locals init(int32 V_0)");
            }

            this.statement.GenCode(of, this.methodName, level+1);
            Node rtn = FindClass(typeof(ReturnStatement));
            if (rtn != null)
                ((ReturnStatement)rtn).GenCodePost(of, this.methodName, level + 1);
            EmitLine(of, level + 1, "ret");
            EmitLine(of, level, "}}");


        }
    }
}

