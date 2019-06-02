using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CSJT11.AST
{
    public abstract class Type : Node
    {
        protected abstract bool Equal(Type other);
        
        public bool Compatible(Type other)
        {
            if (other == null)
                return false;

            return Equal(other);
        }
    }

    public class ArrayType : Type
    {
        private Type nameType;
        public ArrayType (Type nameType)
        {
            this.nameType = nameType;
        }

        public override void ResolveNames(LexicalScope scope)
        {
        }

        public override void TypeCheck()
        {
        }

        protected override bool Equal(Type other)
        {
            // TODO: check compatabile
            return this.GetType() == other.GetType();
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            this.nameType.GenCode(of, genOption, level);
            Emit(of, 0, "[ ] ");
        }
    }

    public class NameType : Type
    {
        private string name;
        public NameType(string name)
        {
            this.name = name;
        }

        public override void ResolveNames(LexicalScope scope)
        {
        }

        public override void TypeCheck()
        {
        }

        protected override bool Equal(Type other)
        {
            // TODO: check compatabile
            return this.GetType() == other.GetType();
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (genOption == "[[return]]")
                return; // Do nothing..... 

            Emit(of, 0, this.name);
        }
    }

    public class VoidType : Type
    {
        private string name;
        public VoidType(string name)
        {
            this.name = name;
        }

        public override void ResolveNames(LexicalScope scope)
        {
        }

        public override void TypeCheck()
        {
        }

        protected override bool Equal(Type other)
        {
            return this.GetType() == other.GetType();
        }
        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            if (genOption == "[[return]]")
                return; // Do nothing..... 

            Emit(of, 0, this.name);
        }
    }

    public enum UnannPrimitiveType
    {
        Boolean,
        Int,
        Byte,
        Short,
        Long,
        Char,
        Float
    }

    public class PrimitiveType : Type
    {
        private UnannPrimitiveType type;
        public PrimitiveType(UnannPrimitiveType type)
        {
            this.type = type;
        }

        public override void ResolveNames(LexicalScope scope)
        {
        }

        public override void TypeCheck()
        {
        }

        protected override bool Equal(Type other)
        {
            // TODO: check compatabile
            return this.GetType() == other.GetType();
        }

        public override void GenCode(StreamWriter of, string genOption, int level)
        {
            switch (this.type)
            {
                case UnannPrimitiveType.Boolean:
                    Emit(of, 0, "bool ");
                    break;
                case UnannPrimitiveType.Byte:
                case UnannPrimitiveType.Char:
                case UnannPrimitiveType.Int:
                case UnannPrimitiveType.Long:
                case UnannPrimitiveType.Short:
                    Emit(of, 0, "int32 ");
                    break;
            }
        }
    }
}
