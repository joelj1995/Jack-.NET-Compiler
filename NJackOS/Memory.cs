using System;

namespace NJackOS
{
    public static class Memory
    {


        private const int HACK_MEMORY_SIZE_BYTES = 24576;
        private static byte[] hackMemory = new byte[HACK_MEMORY_SIZE_BYTES];
    }
}
