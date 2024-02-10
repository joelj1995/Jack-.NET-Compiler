using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackMemory
    {
        short peek(short address);
        void poke(short address, short value);
        JackArrayClass alloc(short size);
        void deAlloc(JackArrayClass o);
    }
}
