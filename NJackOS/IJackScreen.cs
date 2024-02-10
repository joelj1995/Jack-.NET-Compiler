using System;
using System.Collections.Generic;
using System.Text;

namespace NJackOS.Interface
{
    public interface IJackScreen
    {
        void clearScreen();
        void setColor(bool b);
        void drawPixel(Int16 x, Int16 y);
        void drawLine(Int16 x1, Int16 x2, Int16 y1, Int16 y2);
        void drawRectangle(Int16 x1, Int16 y1, Int16 x2, Int16 y2);
        void drawCircle(Int16 cx, Int16 cy, Int16 r);
    }
}
