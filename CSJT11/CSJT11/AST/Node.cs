using System;
using System.IO;
using System.Collections.Generic;

namespace CSJT11.AST
{

    public abstract class Node 
    {
        public abstract void ResolveNames(LexicalScope scope);
        public abstract void TypeCheck();
        public abstract void GenCode(StreamWriter of, string genOption, int level);

        void Indent(int n) 
        {
            for (int i = 0; i < n; i++) 
                Console.Write(" ");
        }

        public void DumpValue(int indent) 
        {
            Indent(indent);
            Console.WriteLine("{0}", GetType().ToString());
            Indent(indent); Console.WriteLine("{");

            foreach (var field in GetType().GetFields(System.Reflection.BindingFlags.NonPublic |
                                                      System.Reflection.BindingFlags.Instance))
            {
                object value = field.GetValue(this);
                Indent(indent + 1);

                // Is this value something we can iterate through?
                // We test that it is a generic type, this way we don't treat strings as IEnumerables.
                if (value is System.Collections.IEnumerable && value.GetType().IsGenericType)
                {
                    Console.WriteLine("{0}:", field.Name);
                    Indent(indent + 1);
                    Console.WriteLine("{");

                    foreach (object item in (System.Collections.IEnumerable)value)
                    {
                        if (item is Node)
                        {
                            ((Node)item).DumpValue(indent + 2);
                        }
                        else
                        {
                            Indent(indent + 2);
                            Console.WriteLine("{0}", item);
                        }
                    }

                    Indent(indent + 1);
                    Console.WriteLine("}");
                }
                else if (value is Node)
                {
                    Console.WriteLine("{0}:", field.Name);
                    ((Node)value).DumpValue(indent + 2);
                }
                else
                {
                    Console.WriteLine("{0}: {1}", field.Name, value);
                }
            }

            Indent(indent);
            Console.WriteLine("}"); 
        }

        public Node FindClass(System.Type t)
        {
            if (t.Equals(this.GetType()))
            {
                return (Node)this;
            }

            foreach (var field in GetType().GetFields(System.Reflection.BindingFlags.NonPublic |
                                                      System.Reflection.BindingFlags.Instance))
            {
                object value = field.GetValue(this);
                if (value is System.Collections.IEnumerable && value.GetType().IsGenericType)
                {
                    foreach (object item in (System.Collections.IEnumerable)value)
                    {
                        if (item is Node)
                        {
                            Node obj = ((Node)item).FindClass(t);
                            if (obj != null && t.Equals(obj.GetType()))
                            {
                                return (Node)obj;
                            }
                        }
                    }
                }
                else if (value is Node)
                {
                    Node obj = ((Node)value).FindClass(t);
                    if (obj != null && t.Equals(obj.GetType()))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        protected void Emit(StreamWriter of, int level, string fmt, params object[] args)
        {
            for(int i=0; i < level;  i++)
                of.Write("\t");
            of.Write(fmt, args);
        }

        protected void EmitLine(StreamWriter of, int level, string fmt, params object[] args)
        {
            for (int i = 0; i < level; i++)
                of.Write("\t");
            of.WriteLine(fmt, args);
        }
    }
}
