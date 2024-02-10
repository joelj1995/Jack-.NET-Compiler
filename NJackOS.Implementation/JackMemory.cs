using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackMemory : IJackMemory
    {
        public JackMemory() 
        {
            Console.WriteLine("Jack Memory initialized.");
        }

        public Interface.JackArrayClass alloc(short size)
        {
            short result = free;
            if (free + size > maxMem)
                throw new Exception("Out of memory.");
            free = (short)(free + size);
            return new JackArrayImplementation(size, result);
        }

        public void deAlloc(Interface.JackArrayClass o)
        {
            return;
        }

        public short peek(short address)
        {
            return memory[address];
        }

        public void poke(short address, short value)
        {
            memory[address] = value;
        }

        /*
         * An array of shorts works well enough but something
         * more optimized could be used with a bit more sophistication.
         */
        private short[] memory = new short[maxMem];
        public const int maxMem = 32768;
        public const short pheap = 2048;
        public const short pheapend = 16383;
        public short free = pheap;
    }
}
