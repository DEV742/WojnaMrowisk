using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    static class rand
    {
        static Random genX = new Random();

        public static int generate(int a, int b)
        {
            int i = genX.Next(a, b);
            return i;
        }
    }
}
