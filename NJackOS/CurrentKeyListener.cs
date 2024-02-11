using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    internal class CurrentKeyListener : ICurrentKeyObserver
    {
        public char CurrentKey { get; private set; } = '\0';

        public void OnCurrentKeyChanged(char key)
        {
            Console.WriteLine($"New key: {key}");
            this.CurrentKey = key;
        }
    }
}
