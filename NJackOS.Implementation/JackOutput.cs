using NJackOS.Interface;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NJackOS.Implementation
{
    public class JackOutput : IJackOutput
    {
        

        public JackOutput()
        {
            initMap();
            Console.WriteLine("Jack Output initialized.");
        }

        public void printInt(short i)
        {
            string str = i.ToString();
            printString(str);
        }

        public void moveCursor(short i, short j)
        {
            curRow = i;
            curColumn = j;
            writeBitmapPixels(charMaps[32]);
        }

        public void printChar(char c)
        {
            writeBitmapPixels(getMap(c));
            curColumn = (short)(curColumn + 1);
            if (curColumn > 63)
            {
                curColumn = 0;
                curRow = (short)(curRow + 1);
                if (curRow > 22)
                {
                    curRow = 0;
                }
            }
        }

        public void printString(string s)
        {
            int i = 0;
            while (i < s.Length)
            {
                printChar(s[i]);

                i = i + 1;
            }
        }

        public void println()
        {
            curColumn = 0;
            curRow = (short)(curRow + 1);
            if (curRow > 22)
            {
                curRow = 0;
            }
            return;
        }

        public void backSpace()
        {
            curColumn = (short)(curColumn - 1);
            if (curColumn < 0)
            {
                curColumn = 0;
                if (curRow != 0)
                {
                    curRow = (short)(curRow - 1);
                }
            }
        }

        private void initMap()
        {
            bitMask[0] = 1;
            bitMask[1] = 2;
            bitMask[2] = 4;
            bitMask[3] = 8;
            bitMask[4] = 16;
            bitMask[5] = 32;
            bitMask[6] = 64;
            bitMask[7] = 128;

            #region MAPS
            Create(0, 63, 63, 63, 63, 63, 63, 63, 63, 63, 0, 0);
            Create(32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);          //
            Create(33, 12, 30, 30, 30, 12, 12, 0, 12, 12, 0, 0);  // !
            Create(34, 54, 54, 20, 0, 0, 0, 0, 0, 0, 0, 0);       // "
            Create(35, 0, 18, 18, 63, 18, 18, 63, 18, 18, 0, 0);  // #
            Create(36, 12, 30, 51, 3, 30, 48, 51, 30, 12, 12, 0); // $
            Create(37, 0, 0, 35, 51, 24, 12, 6, 51, 49, 0, 0);    // %
            Create(38, 12, 30, 30, 12, 54, 27, 27, 27, 54, 0, 0); // &
            Create(39, 12, 12, 6, 0, 0, 0, 0, 0, 0, 0, 0);        // '
            Create(40, 24, 12, 6, 6, 6, 6, 6, 12, 24, 0, 0);      // (
            Create(41, 6, 12, 24, 24, 24, 24, 24, 12, 6, 0, 0);   // )
            Create(42, 0, 0, 0, 51, 30, 63, 30, 51, 0, 0, 0);     // *
            Create(43, 0, 0, 0, 12, 12, 63, 12, 12, 0, 0, 0);     // +
            Create(44, 0, 0, 0, 0, 0, 0, 0, 12, 12, 6, 0);        // ,
            Create(45, 0, 0, 0, 0, 0, 63, 0, 0, 0, 0, 0);         // -
            Create(46, 0, 0, 0, 0, 0, 0, 0, 12, 12, 0, 0);        // .    
            Create(47, 0, 0, 32, 48, 24, 12, 6, 3, 1, 0, 0);      // /


            Create(48, 12, 30, 51, 51, 51, 51, 51, 30, 12, 0, 0); // 0
            Create(49, 12, 14, 15, 12, 12, 12, 12, 12, 63, 0, 0); // 1
            Create(50, 30, 51, 48, 24, 12, 6, 3, 51, 63, 0, 0);   // 2
            Create(51, 30, 51, 48, 48, 28, 48, 48, 51, 30, 0, 0); // 3
            Create(52, 16, 24, 28, 26, 25, 63, 24, 24, 60, 0, 0); // 4
            Create(53, 63, 3, 3, 31, 48, 48, 48, 51, 30, 0, 0);   // 5
            Create(54, 28, 6, 3, 3, 31, 51, 51, 51, 30, 0, 0);    // 6
            Create(55, 63, 49, 48, 48, 24, 12, 12, 12, 12, 0, 0); // 7
            Create(56, 30, 51, 51, 51, 30, 51, 51, 51, 30, 0, 0); // 8
            Create(57, 30, 51, 51, 51, 62, 48, 48, 24, 14, 0, 0); // 9


            Create(58, 0, 0, 12, 12, 0, 0, 12, 12, 0, 0, 0);      // :
            Create(59, 0, 0, 12, 12, 0, 0, 12, 12, 6, 0, 0);      // ;
            Create(60, 0, 0, 24, 12, 6, 3, 6, 12, 24, 0, 0);      // <
            Create(61, 0, 0, 0, 63, 0, 0, 63, 0, 0, 0, 0);        // =
            Create(62, 0, 0, 3, 6, 12, 24, 12, 6, 3, 0, 0);       // >
            Create(64, 30, 51, 51, 59, 59, 59, 27, 3, 30, 0, 0);  // @
            Create(63, 30, 51, 51, 24, 12, 12, 0, 12, 12, 0, 0);  // ?

            Create(65, 12, 30, 51, 51, 63, 51, 51, 51, 51, 0, 0);          // A ** TO BE FILLED **
            Create(66, 31, 51, 51, 51, 31, 51, 51, 51, 31, 0, 0); // B
            Create(67, 28, 54, 35, 3, 3, 3, 35, 54, 28, 0, 0);    // C
            Create(68, 15, 27, 51, 51, 51, 51, 51, 27, 15, 0, 0); // D
            Create(69, 63, 51, 35, 11, 15, 11, 35, 51, 63, 0, 0); // E
            Create(70, 63, 51, 35, 11, 15, 11, 3, 3, 3, 0, 0);    // F
            Create(71, 28, 54, 35, 3, 59, 51, 51, 54, 44, 0, 0);  // G
            Create(72, 51, 51, 51, 51, 63, 51, 51, 51, 51, 0, 0); // H
            Create(73, 30, 12, 12, 12, 12, 12, 12, 12, 30, 0, 0); // I
            Create(74, 60, 24, 24, 24, 24, 24, 27, 27, 14, 0, 0); // J
            Create(75, 51, 51, 51, 27, 15, 27, 51, 51, 51, 0, 0); // K
            Create(76, 3, 3, 3, 3, 3, 3, 35, 51, 63, 0, 0);       // L
            Create(77, 33, 51, 63, 63, 51, 51, 51, 51, 51, 0, 0); // M
            Create(78, 51, 51, 55, 55, 63, 59, 59, 51, 51, 0, 0); // N
            Create(79, 30, 51, 51, 51, 51, 51, 51, 51, 30, 0, 0); // O
            Create(80, 31, 51, 51, 51, 31, 3, 3, 3, 3, 0, 0);     // P
            Create(81, 30, 51, 51, 51, 51, 51, 63, 59, 30, 48, 0);// Q
            Create(82, 31, 51, 51, 51, 31, 27, 51, 51, 51, 0, 0); // R
            Create(83, 30, 51, 51, 6, 28, 48, 51, 51, 30, 0, 0);  // S
            Create(84, 63, 63, 45, 12, 12, 12, 12, 12, 30, 0, 0); // T
            Create(85, 51, 51, 51, 51, 51, 51, 51, 51, 30, 0, 0); // U
            Create(86, 51, 51, 51, 51, 51, 30, 30, 12, 12, 0, 0); // V
            Create(87, 51, 51, 51, 51, 51, 63, 63, 63, 18, 0, 0); // W
            Create(88, 51, 51, 30, 30, 12, 30, 30, 51, 51, 0, 0); // X
            Create(89, 51, 51, 51, 51, 30, 12, 12, 12, 30, 0, 0); // Y
            Create(90, 63, 51, 49, 24, 12, 6, 35, 51, 63, 0, 0);  // Z

            Create(91, 30, 6, 6, 6, 6, 6, 6, 6, 30, 0, 0);          // [
            Create(92, 0, 0, 1, 3, 6, 12, 24, 48, 32, 0, 0);        // \
            Create(93, 30, 24, 24, 24, 24, 24, 24, 24, 30, 0, 0);   // ]
            Create(94, 8, 28, 54, 0, 0, 0, 0, 0, 0, 0, 0);          // ^
            Create(95, 0, 0, 0, 0, 0, 0, 0, 0, 0, 63, 0);           // _
            Create(96, 6, 12, 24, 0, 0, 0, 0, 0, 0, 0, 0);          // `

            Create(97, 0, 0, 0, 14, 24, 30, 27, 27, 54, 0, 0);      // a
            Create(98, 3, 3, 3, 15, 27, 51, 51, 51, 30, 0, 0);      // b
            Create(99, 0, 0, 0, 30, 51, 3, 3, 51, 30, 0, 0);        // c
            Create(100, 48, 48, 48, 60, 54, 51, 51, 51, 30, 0, 0);  // d
            Create(101, 0, 0, 0, 30, 51, 63, 3, 51, 30, 0, 0);      // e
            Create(102, 28, 54, 38, 6, 15, 6, 6, 6, 15, 0, 0);      // f
            Create(103, 0, 0, 30, 51, 51, 51, 62, 48, 51, 30, 0);   // g
            Create(104, 3, 3, 3, 27, 55, 51, 51, 51, 51, 0, 0);     // h
            Create(105, 12, 12, 0, 14, 12, 12, 12, 12, 30, 0, 0);   // i
            Create(106, 48, 48, 0, 56, 48, 48, 48, 48, 51, 30, 0);  // j
            Create(107, 3, 3, 3, 51, 27, 15, 15, 27, 51, 0, 0);     // k
            Create(108, 14, 12, 12, 12, 12, 12, 12, 12, 30, 0, 0);  // l
            Create(109, 0, 0, 0, 29, 63, 43, 43, 43, 43, 0, 0);     // m
            Create(110, 0, 0, 0, 29, 51, 51, 51, 51, 51, 0, 0);     // n
            Create(111, 0, 0, 0, 30, 51, 51, 51, 51, 30, 0, 0);     // o
            Create(112, 0, 0, 0, 30, 51, 51, 51, 31, 3, 3, 0);      // p
            Create(113, 0, 0, 0, 30, 51, 51, 51, 62, 48, 48, 0);    // q
            Create(114, 0, 0, 0, 29, 55, 51, 3, 3, 7, 0, 0);        // r
            Create(115, 0, 0, 0, 30, 51, 6, 24, 51, 30, 0, 0);      // s
            Create(116, 4, 6, 6, 15, 6, 6, 6, 54, 28, 0, 0);        // t
            Create(117, 0, 0, 0, 27, 27, 27, 27, 27, 54, 0, 0);     // u
            Create(118, 0, 0, 0, 51, 51, 51, 51, 30, 12, 0, 0);     // v
            Create(119, 0, 0, 0, 51, 51, 51, 63, 63, 18, 0, 0);     // w
            Create(120, 0, 0, 0, 51, 30, 12, 12, 30, 51, 0, 0);     // x
            Create(121, 0, 0, 0, 51, 51, 51, 62, 48, 24, 15, 0);    // y
            Create(122, 0, 0, 0, 63, 27, 12, 6, 51, 63, 0, 0);      // z


            Create(123, 56, 12, 12, 12, 7, 12, 12, 12, 56, 0, 0);   // {
            Create(124, 12, 12, 12, 12, 12, 12, 12, 12, 12, 0, 0);  // |
            Create(125, 7, 12, 12, 12, 56, 12, 12, 12, 7, 0, 0);    // }
            Create(126, 38, 45, 25, 0, 0, 0, 0, 0, 0, 0, 0);        // ~
            #endregion
        }

        private void Create(short index, short a, short b, short c, short d, short e,
                 short f, short g, short h, short i, short j, short k)
        {
            var map = new short[11];
            map[0] = a;
            map[1] = b;
            map[2] = c;
            map[3] = d;
            map[4] = e;
            map[5] = f;
            map[6] = g;
            map[7] = h;
            map[8] = i;
            map[9] = j;
            map[10] = k;
            charMaps[index] = map;
        }

        private short[] getMap(char c)
        {
            if ((c < 32) | (c > 126))
            {
                c = (char)0;
            }

            return charMaps[c];
        }

        private void writeBitmapPixels(short[] bitmap)
        {
            short i; // rows 0 - 10
            short j; // columns 0-7
            short x;
            short y;
            i = 0;
            while (i < 11)
            {
                j = 0;
                while (j < 7)
                {
                    JackOSProvider.Screen.setColor((bitmap[i] & bitMask[j]) > 0);

                    x = (short)((curColumn * 8) + j);
                    y = (short)((curRow * 11) + i);
                    JackOSProvider.Screen.drawPixel(x, y);

                    j = (short)(j + 1);
                }
                i = (short)(i + 1);
            }
        }

        private JackOSProvider os;
        private short[] bitMask = new short[8];
        private short curRow = 0;
        private short curColumn = 0;
        short[][] charMaps = new short[127][];
    }
}
