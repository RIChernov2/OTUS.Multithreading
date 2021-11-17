using System;
using System.Linq;


namespace MultiThreads
{
    public static class Helper
    {
        public static int[] GenerateIntArray(int arraySise)
        {
            Random random = new Random();
            //return new int[arraySise].Select(x => x = random.Next(1, 20)).ToArray();
            return new int[arraySise].Select(x => x = random.Next(int.MinValue, int.MaxValue)).ToArray();
        }
    }
}
