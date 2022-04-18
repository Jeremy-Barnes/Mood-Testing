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

        public MoodAxis Axis { get; }
        private Dictionary<MoodAxis, AxisLink> LinkedMoods { get; set; } = new Dictionary<MoodAxis, AxisLink>();

        public void LinkMoodsBidirectional(MoodVector linkMood, Correlation correlation, Correlation inverseCorrelation)
        {
            if (correlation.ValueUpperLimit == correlation.ValueLowerLimit) correlation = null;
            if (inverseCorrelation.ValueUpperLimit == inverseCorrelation.ValueLowerLimit) inverseCorrelation = null;
            LinkMoodsUnidirectional(linkMood, correlation);
            linkMood.LinkMoodsUnidirectional(this, inverseCorrelation ?? correlation);

        }

        public void LinkMoodsUnidirectional(MoodVector linkMood, Correlation correlation)
        {
            if (correlation == null) return;
            if (linkMood.Axis == Axis) throw new InvalidOperationException("Cant link a mood with itself. You can't land on a fraction!");

            correlation.Name = $"{Axis} --> {linkMood.Axis}";
            var link = LinkedMoods.GetValueOrDefault(linkMood.Axis, new AxisLink(linkMood));
            link.CorrelationRanges.Add(correlation);

            LinkedMoods[linkMood.Axis] = link;

        }

        public void UpdateMood(double delta)
        {
            var originalValue = this.Value;
            var modifiedDelta = delta;
            foreach (var link in LinkedMoods.Values)
            {
                //limit correlation's impact based on the size of the impulse
                var correlation = Math.Abs(delta / MoodVector.Max) * link.CorrelationFactor * link.Correlation.CalculateCorrelationMagnitude(link.LinkedVector.Value); //Math.Max(.01, ); //(link.LinkedVector.Value - Half) / Max;
                modifiedDelta = modifiedDelta + correlation;
            }
            Value += modifiedDelta;

            foreach (var link in LinkedMoods.Values)
            {
                link.LinkedVector.UpdateMoodForPropagation(this);
            }
        }

        protected void UpdateMoodForPropagation(MoodVector originatingMood)
        {
            var moodLink = LinkedMoods[originatingMood.Axis];
            var modifiedDelta = (moodLink.Correlation.CalculateCorrelationMagnitude(moodLink.LinkedVector.Value) * moodLink.CorrelationFactor);// (moodLink.LinkedVector.Value - Half); // / delta

            if (Math.Sign(modifiedDelta) != Math.Sign(moodLink.CorrelationFactor)) //correlation is stronger forwards than it is in reverse
            {
                modifiedDelta /= 2;
            }

            foreach (var link in LinkedMoods.Values)
            {
                double correlation;
                if (link == moodLink)
                {
                    continue;
                }
                //else
                //{
                correlation = (moodLink.Correlation.CalculateCorrelationMagnitude(moodLink.LinkedVector.Value) * moodLink.CorrelationFactor);
                //}
                modifiedDelta = modifiedDelta + correlation;

            }
            Value += modifiedDelta;
        }

        public override string ToString()
        {
            return $"{Axis.ToString().PadRight(10)} - {Value.ToString("##.##").PadRight(5)}";
        }

        public static double GetPercentOfRange(int percent)
        {
            if (percent == 0) return Min;
            if (percent == 100) return Max;

            double percentDecimal = percent / 100.0;
            return (Max * percentDecimal) + (Min * (1 - percentDecimal));
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
        public List<Correlation> CorrelationRanges { get; set; } = new List<Correlation>();
        public MoodVector LinkedVector { get; set; }
        public double CorrelationFactor => CorrelationRanges.FirstOrDefault(range => range.ContainsValue(LinkedVector.Value)).Factor;
        public Correlation Correlation => CorrelationRanges.FirstOrDefault(range => range.ContainsValue(LinkedVector.Value));

        public AxisLink(MoodVector link)
        {
            LinkedVector = link;
        }

        public override string ToString()
        {
            return LinkedVector.ToString();
        }
    }

    public class Correlation
    {
        public Correlation(double factor, double lowerLimitValue, double upperLimitValue)
        {
            Factor = factor;
            ValueLowerLimit = lowerLimitValue;
            ValueUpperLimit = upperLimitValue;

            if (ValueLowerLimit > upperLimitValue)
            {
                ValueLowerLimit = upperLimitValue;
            }

        }

        public string Name { get; set; }
        public double Factor { get; set; }
        public double ValueUpperLimit { get; set; }
        public double ValueLowerLimit { get; set; }
        public double CalculateCorrelationMagnitude(double value)
        {
            if (ValueUpperLimit < MoodVector.Half)
            {
                return (ValueUpperLimit - value) / ValueUpperLimit;
            }
            else
            {
                return value / ValueUpperLimit;
            }
        }
        public bool ContainsValue(double value)
        {
            return ValueLowerLimit <= value && value <= ValueUpperLimit;
        }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
