using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackOutput
    {
        void moveCursor(short i, short j);
        void printChar(char c);
        void printString(string s);
        void printInt(short i);
        void println();
        void backSpace();
    }
}
