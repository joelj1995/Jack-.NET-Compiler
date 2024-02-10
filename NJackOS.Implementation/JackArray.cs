using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackArray : IJackArray
    {
        public JackArrayClass FromCLRShort(short value)
        {
            var result = this.New(1);
            result[0] = value;
            return result;
        }

        public JackArrayClass New(short size)
        {
            return JackOSProvider.Memory.alloc(size);
        }
    }
}
