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
        }
    }
    public struct Rates
    {
        public DateTime Time;
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public Signal Signal;

        public Rates(string[] data)
        {
            Time = Convert.ToDateTime(data[0]);

            Open = float.Parse(data[1]);
            High = float.Parse(data[2]);
            Low = float.Parse(data[3]);
            Close = float.Parse(data[4]);

            Signal = new Signal(data);
        }
        public float[] ToFloat()
        {
            return new float[] { Open, High, Low, Close};
        }
    }
    public class Signal
    {
        public SignalEnum Value;
        public Signal(string[] data)
        {
            if (data[5] != "0")
                Value = SignalEnum.FastValley;
            else if (data[6] != "0")
                Value = SignalEnum.SlowValley;
            else if (data[7] != "0")
                Value = SignalEnum.FastPeak;
            else if (data[8] != "0")
                Value = SignalEnum.SlowPeak;
            else
                Value = SignalEnum.Neutral;
        }
    }
    public enum SignalEnum
    {
        Neutral = 0,
        FastValley = 1,
        SlowValley = 2,
        FastPeak = 3,
        SlowPeak = 4,
        Count
    }
    public class MLTrader
    {
        #region Private fields
        private Rates[] _rates;
        private readonly int _rewardLength = 1;
        private readonly int _observationLength = 50;
        private int _index;
        #endregion
        #region Public properties
        public bool IsLastStep => _index == MaximumRates;
        public int MaximumRates => _rates.Length - _rewardLength;
        public int MaximumRewards => MaximumRates - _observationLength;
        #endregion
        private static MLTrader _instance;        
        public static MLTrader Instance => _instance ?? (_instance = new MLTrader());
        public MLTrader()
        {
            using (var streamReader = new StreamReader(@"C:\Users\MikeSifanele\OneDrive - Optimi\Documents\Data\rates_rates.csv"))
            {
                List<Rates> rates = new List<Rates>();

                _ = streamReader.ReadLine();

                while (!streamReader.EndOfStream)
                {
                    rates.Add(new Rates(streamReader.ReadLine().Split(',')));
                }

                _rates = rates.ToArray();
            }

            Reset();
        }
        public float[] GetObservation()
        {
            List<float> observation = new List<float>();

            for (int i = _index; i >= _index - _observationLength; i--)
                observation.AddRange(_rates[i].ToFloat());

            _index++;

            return observation.ToArray();
        }

        public int GetReward(int action) => action == (int)_rates[_index].Signal.Value ? 1 : -1;
        public void Reset() => _index = _observationLength;
    }
}
