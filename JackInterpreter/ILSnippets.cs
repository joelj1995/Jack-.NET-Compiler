using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackInterpreter
{
    internal static class ILSnippets
    {
        public static string RuntimeReference => @".assembly extern System.Runtime
{
	.publickeytoken = (
		b0 3f 5f 7f 11 d5 0a 3a
	)
	.ver 8:0:0:0
}";
    }
}
