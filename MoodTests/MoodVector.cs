using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodTests
{
    public class MoodVector
    {
        public MoodVector(MoodAxis axis)
        {
            Axis = axis;
        }

        public const double Max = 11;
        public const double Min = 1;
        public const double Half = (Max + Min) / 2;
        public const double MidPositiveRange = (Max + Half) / 2;
        public const double MidNegativeRange = (Min + Half) / 2;

        private double moodValue = Half;
        public double Value
        {
            get { return moodValue; }
            set
            {
                if (value > Max)
                {
                    value = Max;
                }
                if (value < Min)
                {
                    value = Min;
                }
                moodValue = value;
            }
        }

        public MoodAxis Axis { get; set; }
        public List<AxisLink> LinkedRelationships { get; set; } = new List<AxisLink>();

        public void LinkMood(MoodVector mood, params Correlation[] correlations)
        {
            if (mood.Axis == Axis || LinkedRelationships.Any(link => link.LinkedVector.Axis == mood.Axis)) return;

            LinkedRelationships.Add(new AxisLink
            {
                //InvertedCorrelationFactor = invertedCorrelationFactor,
                LinkedVector = mood,
                CorrelationRanges = correlations.ToList()
                //StandardCorrelationRange = standardRange,
                //StandardCorrelationFactor = correlationStrength
            });
        }


        public void UpdateMood(double delta)
        {
            var modifiedDelta = delta;
            foreach (var link in LinkedRelationships)
            {
                var correlation = link.CorrelationFactor2 * (link.LinkedVector.Value - Half); //(link.LinkedVector.Value - Half) / Max;
                modifiedDelta = modifiedDelta + correlation;
            }
            Value += modifiedDelta;
        }

        public override string ToString()
        {
            return $"{Axis} - {Value}";
        }
    }

    public enum MoodAxis
    {
        Fear,
        Joy,
        Curiosity,
        Anger,
        Certainty,
        Connection
    }

    public class AxisLink
    {
        //public double StandardCorrelationFactor { get; set; }
        //public double? InvertedCorrelationFactor { get; set; }
        //public (double HighBound, double LowBound) StandardCorrelationRange { get; set; }
        public List<Correlation> CorrelationRanges { get; set; } = new List<Correlation>();
        public MoodVector LinkedVector { get; set; }
        //public bool IsReverseCorrelated
        //{
        //    get
        //    {
        //        return StandardCorrelationRange.HighBound >= LinkedVector.Value || StandardCorrelationRange.LowBound <= LinkedVector.Value;
        //    }
        //}

        //public double CorrelationFactor => IsReverseCorrelated ? InvertedCorrelationFactor.Value : StandardCorrelationFactor;
        public double CorrelationFactor2 => CorrelationRanges.FirstOrDefault(range => range.ContainsValue(LinkedVector.Value)).Factor;
        public override string ToString()
        {
            return LinkedVector.ToString();
        }
    }

    public class Correlation
    {
        public Correlation(double factor, double upperLimitValue, double lowerLimitValue)
        {
            Factor = factor;
            ValueLowerLimit = lowerLimitValue;
            ValueUpperLimit = upperLimitValue;
        }

        public double Factor { get; set; }
        public double ValueUpperLimit { get; set; }
        public double ValueLowerLimit { get; set; }
        public bool ContainsValue(double value)
        {
            return ValueLowerLimit <= value && value <= ValueUpperLimit;
        }
    }
}
