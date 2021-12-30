using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentHelper;

namespace Agent_Test
{
    internal class Program
    {
        private static Random _random = new Random();
        static void Main(string[] args)
        {
            Console.Title = "ML Trader";
            int reward = 0;

            while (!MLTrader.Instance.IsLastStep)
            {
                _ = MLTrader.Instance.GetObservation();
                reward += MLTrader.Instance.GetReward(action: _random.Next((int)SignalEnum.Count));
            }

            Console.WriteLine($"Randomly scored: {reward}/{MLTrader.Instance.MaximumRewards}");

            Console.ReadKey();
        }
        private static void Evolver_WriteToConsole(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
