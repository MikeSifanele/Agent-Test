using System;
using System.Collections.Generic;
using System.IO;
using Enums;

namespace AgentHelper
{
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
            return new float[] { Open, High, Low, Close };
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
    public class MLTrader
    {
        #region Private fields
        private Rates[] _rates;
        private readonly int _rewardLength = 1;
        private readonly int _observationLength = 50;
        private int _index;
        #endregion
        #region Public properties
        public int CurrentStepIndex => _index - _observationLength;
        public bool IsLastStep => _index == MaximumRates;
        public int MaximumRates => _rates.Length - _rewardLength;
        public int MaximumRewards => MaximumRates - _observationLength;
        public int Target => (int)_rates[_index].Signal.Value;
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

        public int GetReward(int action)
        {
            SignalEnum signal = _rates[_index].Signal.Value;

            switch((SignalEnum)action)
            {
                case SignalEnum.FastPeak:
                    if (signal == SignalEnum.FastValley)
                        return -10;
                    else if (signal == SignalEnum.SlowValley)
                        return -100;
                    break;
                case SignalEnum.FastValley:
                    if (signal == SignalEnum.FastPeak)
                        return -10;
                    else if (signal == SignalEnum.SlowPeak)
                        return -100;
                    break;
                case SignalEnum.SlowPeak:
                    if (signal == SignalEnum.FastValley)
                        return -10;
                    else if (signal == SignalEnum.SlowValley)
                        return -100;
                    break;
                case SignalEnum.SlowValley:
                    if (signal == SignalEnum.FastPeak)
                        return -10;
                    else if (signal == SignalEnum.SlowPeak)
                        return -100;
                    break;
                case SignalEnum.Neutral:
                    if (signal == SignalEnum.FastPeak)
                        return -10;
                    else if (signal == SignalEnum.SlowPeak)
                        return -100;
                    else if (signal == SignalEnum.FastValley)
                        return -10;
                    else if (signal == SignalEnum.SlowValley)
                        return -100;
                    break;
            }

            return 1;
        }

        public void Reset() => _index = _observationLength;
    }
}
