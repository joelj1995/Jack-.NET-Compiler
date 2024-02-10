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
        public JackStringClassImplementation(short maxLength, short start) : base(maxLength, start) { }

        public override JackStringClass appendChar(char c)
        {
            throw new NotImplementedException();
        }

        public override char charAt(short i)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void setChatAt(short i, char c)
        {
            throw new NotImplementedException();
        }

        public override void setInt(short j)
        {
            throw new NotImplementedException();
        }
    }
}
