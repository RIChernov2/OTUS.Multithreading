using System;
using System.Diagnostics;
using System.Linq;

namespace MultiThreads
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array100K = Helper.GenerateIntArray(100_000);
            int[] array1M = Helper.GenerateIntArray(1_000_000);
            int[] array10M = Helper.GenerateIntArray(10_000_000);

            ArrayCalculator calculator100K = new(array100K);
            ArrayCalculator calculator1M = new(array1M);
            ArrayCalculator calculator10M = new(array10M);

            calculator100K.Calulate();
            calculator1M.Calulate();
            calculator10M.Calulate();


            calculator100K.Print();
            calculator1M.Print();
            calculator10M.Print();

            Console.WriteLine("Press Enter to finish");
            Console.ReadLine();
        }
    }
}
