using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal class PopulateSymbolTableJackListener : JackBaseListener
    {
        public PopulateSymbolTableJackListener(SymbolTable symbolTable)
        {
            this.symbolTable = symbolTable;
        }

        private readonly SymbolTable symbolTable;
    }
}
