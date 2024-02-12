using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackMath : IJackMath
    {
        public short abs(short x)
        {
            return Math.Abs(x);
        }

        public short max(short x, short y)
        {
            return Math.Max(x, y);
        }

        public short min(short x, short y)
        {
            return Math.Min(x, y);
        }

        public short sqrt(short x)
        {
            return (short)Math.Sqrt(x);
        }
    }
}
