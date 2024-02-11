using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackKeyboard
    {
        char keyPressed();
        char readChar();
        JackStringClass readLine(JackStringClass message);
        short readInt(JackStringClass message);
    }
}
