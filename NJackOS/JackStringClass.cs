using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public abstract class JackStringClass : JackObject
    {
        public JackStringClass(short maxLength, short start) : base(start)
        {
            this.maxLength = maxLength;
        }

        public override string ToString()
        {
            return ToCLRString();
        }

        public abstract void dispose();
        public abstract short length();
        public abstract char charAt(short i);
        public abstract void setCharAt(short i, char c);
        public abstract JackStringClass appendChar(char c);
        public abstract void eraseLastChar();
        public abstract short intValue();
        public abstract void setInt(short j);
        public abstract string ToCLRString();

        protected readonly short maxLength;
    }
}
