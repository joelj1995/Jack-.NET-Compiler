using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NJackOS.Implementation
{
    internal class JackSys : IJackSys
    {
        public JackSys()
        {
            Console.WriteLine("Jack Sys initialized.");
        }

        public void error(short errorCode)
        {
            throw new Exception(errorCode.ToString());
        }

        public void halt()
        {
            System.Environment.Exit(4);
        }

        public void wait(short duration)
        {
            Thread.Sleep(duration);
        }
    }
}
