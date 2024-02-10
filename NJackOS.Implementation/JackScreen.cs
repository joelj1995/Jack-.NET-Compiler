using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Implementation
{
    internal class JackScreen : IJackScreen
    {
        public JackScreen() 
        {
            Console.WriteLine("Jack Screen initialized.");
        }

        public void clearScreen()
        {
            throw new NotImplementedException();
        }

        public void drawCircle(short cx, short cy, short r)
        {
            throw new NotImplementedException();
        }

        public void drawLine(short x1, short x2, short y1, short y2)
        {
            throw new NotImplementedException();
        }

        public void drawPixel(short x, short y)
        {
            throw new NotImplementedException();
        }

        public void drawRectangle(short x1, short x2, short y1, short y2)
        {
            throw new NotImplementedException();
        }

        public void setColor(bool b)
        {
            throw new NotImplementedException();
        }
    }
}
