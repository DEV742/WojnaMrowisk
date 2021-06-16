using System;

namespace WojnaMrowisk
{
    internal static class rand
    {
        private static readonly Random genX = new Random();

        //A method to randomly generate a number within a certain range. Is used when local Random.Next is called too often
        public static int generate(int a, int b)
        {
            var i = genX.Next(a, b);
            return i;
        }
    }
}