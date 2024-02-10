using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackString : IJackString
    {
        public JackString()
        {
            Console.WriteLine("Jack String initialized.");
        }

        public char backSpace()
        {
            return (char)8;
        }

        public char doubleQuote()
        {
            return '"';
        }

        public JackStringClass FromCLRString(string value)
        {
            var result = New((short)value.Length);
            for (short i = 0; i < value.Length; i++)
            {
                result.appendChar(value[i]);
            }
            return result;
        }

        public JackStringClass New(short maxLength)
        {
            var start = JackOSProvider.Memory.alloc(maxLength);
            return new JackStringClassImplementation(maxLength, start.Start);
        }

        public char newLine()
        {
            return '\n';
        }
    }
}
