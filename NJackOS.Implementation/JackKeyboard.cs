using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackKeyboard : IJackKeyboard
    {
        public char keyPressed()
        {
            return Key;
        }

        public char readChar()
        {
            var key = Key;
            while (key == '\0')
            {
                key = Key;
                Thread.Sleep(1);
            }
            var key2 = Key;
            while (key2 > '\0')
            {
                key2 = Key;
                Thread.Sleep(1);
            }
            JackOSProvider.Output.printChar(key);
            return key;
        }

        public short readInt(JackStringClass message)
        {
            var str = readLine(message);
            return str.intValue();
        }

        public JackStringClass readLine(JackStringClass message)
        {
            var str = JackOSProvider.String.New(256);
            JackOSProvider.Output.printString(message);
            while (true)
            {
                var c = readChar();
                if (c == JackOSProvider.String.newLine())
                {
                    JackOSProvider.Output.printString(str);
                    return str;
                }
                if (c == JackOSProvider.String.backSpace())
                {
                    if (str.length() > 0)
                    {
                        str.eraseLastChar();
                        JackOSProvider.Output.backSpace();
                    }
                }
                else
                {
                    if (str.length() < 256)
                    {
                        str.appendChar(c);
                    }
                }
            }
            throw new Exception();
        }

        private char Key => JackOSProvider.CurrentKey;
    }
}
