using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackString
    {
        JackStringClass FromCLRString(string value);
        JackStringClass New(short maxLength);
        char backSpace();
        char doubleQuote();
        char newLine();
    }
}
