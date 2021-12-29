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

            using (var streamReader = new StreamReader(@"C:\Users\MikeSifanele\OneDrive - Optimi\Documents\Data\rates_vp.csv"))
            {
                List<Rates> rates = new List<Rates>();

                _ = streamReader.ReadLine();

                while (!streamReader.EndOfStream)
                {
                    rates.Add(new Rates(streamReader.ReadLine().Split(',')));
                }

                _rates = rates.ToArray();
            }


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
        SlowPeak = 4
    }
    public class MLTrader
    {
        #region Private fields
        private Rates[] _rates;
        private int _rewardLength = 1;
        private int _ratesLength;
        #endregion
        #region Public fields
        public int Index;
        #endregion
        #region Public properties
        public bool IsLastStep
        {
            get { return Index == _ratesLength; }
        }
        #endregion
        private static MLTrader _instance;        
        public static MLTrader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MLTrader();

                return _instance;
            }
            set { _instance = value; }
        }
        public MLTrader()
        {
            _rates = new Rates[0];

            _ratesLength = _rates.Length - _rewardLength;
            Reset();
        }
        public float[] GetObservation()
        {
            return _rates[Index].ToFloat();
        }
        public float GetReward(int[] action)
        {
            int profit = GetPoints(action: action[0], _rates[Index + 1].Open, _rates[Index + 1].Close);

            int drawDown = GetPoints(action[0], _rates[Index+1].Open, action[0] == 0? _rates[Index+1].Low: _rates[Index+1].High);

            int risk = action[1] - drawDown;

            return risk < 1? -(drawDown + profit): drawDown + profit;
        }
        public void Reset()
        {
            Index = 0;
        }        
        private int GetPoints(int action, float openPrice, float closePrice)
        {
            if(action == 0)
                return (int)((closePrice - openPrice) * 10);
            else
                return (int)((openPrice - closePrice) * 10);
        }
    }
}
