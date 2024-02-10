using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public abstract class JackObject
    {
        public JackObject(short start)
        {
            this.start = start;
        }

        public short this[short i] { get => JackOSProvider.Memory.peek(i); set => JackOSProvider.Memory.poke(start, value); }

        protected readonly short start;
    }
}
