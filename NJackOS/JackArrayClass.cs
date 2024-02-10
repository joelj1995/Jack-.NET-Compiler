using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public abstract class JackArrayClass : JackObject
    {
        public short Start => start;

        public JackArrayClass(short start) : base(start)
        {
        }

    }
}
