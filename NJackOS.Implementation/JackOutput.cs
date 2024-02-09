using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Implementation
{
    public class JackOutput : IJackOutput
    {
        public JackOutput()
        {
            Console.WriteLine("Jack Output initialized.");
        }

        public void printInt(short i)
        {
            Console.Write(i);
        }
    }
}
