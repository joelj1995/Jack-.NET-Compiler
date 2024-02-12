using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackSys
    {
        void halt();
        void error(short errorCode);
        void wait(short duration);
    }
}
