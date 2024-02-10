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

        public short this[short i] { get => JackOSProvider.Memory.peek((short)(start + i)); set => JackOSProvider.Memory.poke((short)(start + i), value); }

        protected readonly short start;
    }
}
