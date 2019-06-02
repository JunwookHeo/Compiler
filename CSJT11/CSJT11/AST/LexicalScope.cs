using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSJT11.AST
{
    public class LexicalScope
    {
        private readonly LexicalScope parent;
        private readonly Dictionary<string, Declaration> symbol_table;
        public LexicalScope(LexicalScope parent)
        {
            this.symbol_table = new Dictionary<string, Declaration>();
            this.parent = parent;
        }
                
        public Declaration Resolve(string symbol)
        {
            Declaration pd = null;
            if (symbol_table != null)
            {
                symbol_table.TryGetValue(symbol, out pd);//TO check if symbol already exist in the table
            }
            //If not exist, return null for pd

            if (pd == null && parent != null)
            {
                pd = parent.Resolve(symbol);//To search its parent for the symbol
            }
            return pd;
        }

        public void Add(string symbol, Declaration declaration)
        {
            symbol_table.Add(symbol, declaration); 
        }
    }
}

