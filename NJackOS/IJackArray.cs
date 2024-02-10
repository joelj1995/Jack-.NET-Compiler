using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackArray
    {
        JackArrayClass FromCLRShort(short value);
        short New(short size);
    }
}
