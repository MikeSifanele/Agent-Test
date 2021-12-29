using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Agent_Test
{
    internal class Program
    {
        private static Rates[] _rates;
        static void Main(string[] args)
        {
            Console.Title = "ML Trader";

            while (!MLTrader.Instance.IsLastStep)
            {

            }

            Console.WriteLine($"Randomly scored: {reward}/{MLTrader.Instance.MaximumRewards}");

            Console.ReadKey();
        }
    }
}
