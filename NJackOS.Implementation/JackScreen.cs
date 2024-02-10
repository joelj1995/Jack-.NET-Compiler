using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NJackOS.Implementation
{
    internal class JackScreen : IJackScreen
    {
        public JackScreen() 
        {
            internals = new JackStreenInternal();
            Thread t = new Thread(new ThreadStart(internals.StartNewThread));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            while (!internals.Started)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("Jack Screen initialized.");
        }

        public void clearScreen()
        {
            internals.clearScreen();
        }

        public void drawCircle(short cx, short cy, short r)
        {
            internals.drawCircle(cx, cy, r, color);
        }

        public void drawLine(short x1, short x2, short y1, short y2)
        {
            internals.drawLine(x1, x2, y1, y2, color);
        }

        public void drawPixel(short x, short y)
        {
            internals.drawPixel(x, y, color);
        }

        public void drawRectangle(short x1, short x2, short y1, short y2)
        {
            internals.drawRectangle(x1, x2, y1, y2, color);
        }

        public void setColor(bool b)
        {
            this.color = b;
        }

        private JackStreenInternal internals;
        private bool color = true;
    }
}
