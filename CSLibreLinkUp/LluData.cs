using System;

namespace CSLibreLinkUp
{
    public sealed class LluData
    {
        public enum Trend
        {
            None = 0,
            Down = 1,
            DiagonalDown = 2,
            Across = 3,
            DiagonalUp = 4,
            Up = 5,
        }

        public string PatientId { get; private set; }
        public double Value { get; private set; }
        public int ValueInMgPerDl { get; private set; }
        public bool IsHigh { get; private set; }
        public bool IsLow { get; private set; }
        public Trend TrendArrow { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public LluData(string patientId, double value, int valueInMgPerDl, bool isHigh, bool isLow, Trend trendArrow, DateTime timeStamp)
        {
            PatientId = patientId;
            Value = value;
            ValueInMgPerDl = valueInMgPerDl;
            IsHigh = isHigh;
            IsLow = isLow;
            TrendArrow = trendArrow;
            TimeStamp = timeStamp;
        }
    }
}