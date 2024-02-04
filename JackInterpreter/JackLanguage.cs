using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal enum Segment
    {
        CONST, ARG, LOCAL, STATIC, THIS, THAT, POINTER, TEMP
    }

    internal enum Command
    {
        ADD, SUB, NEG, EQ, GT, LT, AND, OR, NOT
    }
}
