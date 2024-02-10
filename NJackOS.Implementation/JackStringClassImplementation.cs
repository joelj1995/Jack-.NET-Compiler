using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackStringClassImplementation : JackStringClass
    {
        public JackStringClassImplementation(short maxLength, short start) : base(maxLength, start) 
        { 

        }

        public override JackStringClass appendChar(char c)
        {
            setCharAt(curLength, c);
            curLength = (short)(curLength + 1);
            return this;
        }

        public override char charAt(short i)
        {
            return (char)JackOSProvider.Memory.peek((short)(start + i));
        }

        public override void dispose()
        {
            throw new NotImplementedException();
        }

        public override void eraseLastChar()
        {
            throw new NotImplementedException();
        }

        public override short intValue()
        {
            throw new NotImplementedException();
        }

        public override short length()
        {
            return curLength;
        }

        public override void setCharAt(short i, char c)
        {
            JackOSProvider.Memory.poke((short)(start + i), (short)c);
        }

        public override void setInt(short j)
        {
            throw new NotImplementedException();
        }

        public override string ToCLRString()
        {
            var result = string.Empty;
            short i = 0;
            while (i < length())
            {
                result += charAt(i);
                i = (short)(i + 1);
            }
            return result;
        }

        private short curLength = 0;
    }
}
