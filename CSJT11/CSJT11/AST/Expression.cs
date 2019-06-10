using System;
using System.IO;

namespace CSJT11.AST
{
    //public enum OperatorType
    //{
    //    EQ = 0,
    //    PL = 1
    //}
    public abstract class Expression : Node
    {
        //contain value
        public Type type;
    }

    public class AssignmentExpression : Expression
    {
        protected Expression lhs;
        protected Expression rhs;

        public AssignmentExpression(Expression lhs, Expression rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public void SetExpressions(Expression lhs, Expression rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.lhs.ResolveNames(scope);
            this.rhs.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
            this.lhs.TypeCheck();
            this.rhs.TypeCheck();

            if (this.rhs.type.Compatible(this.lhs.type) == false)
            {
                throw new Exception("Mismatch Type");
            }
            this.type = lhs.type;
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.rhs.GenCode(of, "[[load]]", level);
            if (genOption == "[[load]]")
                EmitLine(of, level, "dup");
            this.lhs.GenCode(of, "[[store]]", level);
        }
    }


    public class AndAssignmentExpression : AssignmentExpression
    {
        public AndAssignmentExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.lhs.GenCode(of, "[[load]]", level);
            this.rhs.GenCode(of, "[[load]]", level);
            EmitLine(of, level, "and");
            this.lhs.GenCode(of, "[[store]]", level);

        }
    }


    public class ExpressionName : Expression
    {
        private string name;
        private Declaration declaration; // This is for checking type not generating code

        public ExpressionName(string name)
        {
            this.name = name;
            this.declaration = null;
        }

        public override void ResolveNames(LexicalScope scope)
        {

            if (scope != null)
            {
                this.declaration = scope.Resolve(this.name);
            }

            if (this.declaration == null)
            {
                Console.WriteLine("Error : Undefined identifier {0}\n", this.name);
                throw new Exception("Undefined identifier");
            }
        }

        public override void TypeCheck()
        {
            if (this.declaration != null)
            {
                this.type = this.declaration.GetValType();
            }
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (declaration is LocalVariable)
            {
                if (genOption == "[[store]]")
                    EmitLine(of, level, "stloc V_{0}", ((LocalVariable)declaration).GetNameToIdx(this.name));
                else if (genOption == "[[load]]")
                    EmitLine(of, level, "ldloc V_{0}", ((LocalVariable)declaration).GetNameToIdx(this.name));
            }
            else if (declaration is FormalParameter)
            {
                if (genOption == "[[store]]")
                    EmitLine(of, level, "starg " + this.name);
                else if (genOption == "[[load]]")
                    EmitLine(of, level, "ldarg " + this.name);
            }
            else
            {
                Console.WriteLine("Error : Undefined Variable {0}\n", this.name);
                throw new Exception("Undefined Variable");
            }
        }
    }

    public class IntegerLiteral : Expression
    {
        private int value;
        public IntegerLiteral(int value)
        {
            this.value = value;
        }

        public override void ResolveNames(LexicalScope scope)
        {
        }

        public override void TypeCheck()
        {
            this.type = new PrimitiveType(UnannPrimitiveType.Int);
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (genOption == "[[load]]")
                EmitLine(of, level, "ldc.i4 {0}", this.value);
        }
    }

        public class FloatingPointLiteral : Expression
    {
        private float value;
        public FloatingPointLiteral(float value)
        {
            this.value = value;
        }

        public override void ResolveNames(LexicalScope scope)
        {
        }

        public override void TypeCheck()
        {
            this.type = new PrimitiveType(UnannPrimitiveType.Float);
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            //if (genOption == "[[load]]")
            //    EmitLine(of, level, "ldc.i4 {0}", this.value);
        }
    }

    public class BinaryExpression : Expression
    {
        protected Expression lhs;
        protected Expression rhs;

        public BinaryExpression(Expression lhs, Expression rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public override void ResolveNames(LexicalScope scope)
        {
            this.lhs.ResolveNames(scope);
            this.rhs.ResolveNames(scope);
        }

        public override void TypeCheck()
        {
            this.lhs.TypeCheck();
            this.rhs.TypeCheck();

            if (this.rhs.type.Compatible(this.lhs.type) == false)
            {
                Console.WriteLine("Error : Mismatched type for the operator\n");
                throw new Exception("Mismatched type");
            }
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (genOption == "[[load]]")
            {
                this.lhs.GenCode(of, "[[load]]", level);
                this.rhs.GenCode(of, "[[load]]", level);
            }
        }
    }
    public class AddBinaryExpression : BinaryExpression
    {
        public AddBinaryExpression(Expression lhs, Expression rhs)
            : base(lhs, rhs) { }

        public override void TypeCheck()
        {
            base.TypeCheck();
            this.type = this.rhs.type;
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            base.GenCode(of, genOption, level);
            EmitLine(of, level, "add");
        }
    }

    public class AndBinaryExpression : BinaryExpression
    {
        public AndBinaryExpression(Expression lhs, Expression rhs)
            : base(lhs, rhs) { }

        public override void TypeCheck()
        {
            base.TypeCheck();
            this.type = this.rhs.type;
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            base.GenCode(of, genOption, level);
            EmitLine(of, level, "and");
        }
    }

    public class SubBinaryExpression : BinaryExpression
    {
        public SubBinaryExpression(Expression lhs, Expression rhs)
            : base(lhs, rhs) { }

        public override void TypeCheck()
        {
            base.TypeCheck();
            this.type = this.rhs.type;
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            base.GenCode(of, genOption, level);
            EmitLine(of, level, "sub");
        }
    }

    public class EqualBinaryExpression : BinaryExpression
    {
        public EqualBinaryExpression(Expression lhs, Expression rhs)
            : base(lhs, rhs) { }

        public override void TypeCheck()
        {
            base.TypeCheck();
            this.type = this.rhs.type;
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            base.GenCode(of, genOption, level);
            EmitLine(of, level, "ceq");
        }
    }

    public class UnaryExpression : Expression
    {
        protected Expression exp;

        public UnaryExpression(Expression exp)
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
            this.type = exp.type;
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            EmitLine(of, level, "TODO.....");
        }
    }
    public class PreIncUnaryExpression : UnaryExpression
    {

        public PreIncUnaryExpression(Expression exp) : base(exp) { }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.exp.GenCode(of, "[[load]]", level);
            EmitLine(of, level, "ldc.i4 1");
            EmitLine(of, level, "add");
            if (genOption == "[[load]]")
                EmitLine(of, level, "dup");
            this.exp.GenCode(of, "[[store]]", level);
        }
    }
    public class PostIncUnaryExpression : UnaryExpression
    {

        public PostIncUnaryExpression(Expression exp) : base(exp) { }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.exp.GenCode(of, "[[load]]", level);
            if(genOption == "[[load]]")
                EmitLine(of, level, "dup");
            EmitLine(of, level, "ldc.i4 1");
            EmitLine(of, level, "add");
            this.exp.GenCode(of, "[[store]]", level);
        }
    }
}


