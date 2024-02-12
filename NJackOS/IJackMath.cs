using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackMath
    {
        short abs(short x);
        short min(short x, short y);
        short max(short x, short y);
        short sqrt(short x);
    }
}
