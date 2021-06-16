using System;

namespace WojnaMrowisk
{
    internal static class rand
    {
        private static readonly Random genX = new Random();

        public static int generate(int a, int b)
        {
            var i = genX.Next(a, b);
            return i;
        }
    }
}