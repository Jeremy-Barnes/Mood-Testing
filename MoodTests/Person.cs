using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodTests
{
    public class Person
    {
        public Person(List<(TraitType type, double value)> personalityTraits)
        {
            Moods = Enum.GetValues<MoodAxis>().Select(axis => new MoodVector(axis)).ToList();
            Traits = Enum.GetValues<TraitType>().Select(traitType =>
                personalityTraits.Where(pts => pts.type == traitType)
                                 .Select(traitValue => new Trait(traitValue.type, this, traitValue.value))
                                 .FirstOrDefault(defaultValue: null) ??
                new Trait(traitType, this, Trait.Half))
            .ToList();
        }

        private List<MoodVector> Moods { get; set; }
        private List<Trait> Traits { get; set; }
        public MoodVector Mood(MoodAxis axis)
        {
            return Moods.Where(m => m.Axis == axis).FirstOrDefault();
        }


        public override string ToString()
        {
            string ret = "";
            foreach (var mood in Moods)
            {
                ret += mood.ToString() + "\r\n";
            }
            return ret;
        }
    }

    public class Trait
    {
        public const double Max = 5;
        public const double Min = 1;
        public const double Half = (Max + Min) / 2;
        public const double MidPositiveRange = (Max + Half) / 2;
        public const double MidNegativeRange = (Min + Half) / 2;

        public TraitType TraitType { get; set; }
        private double invertStrength(double strength)
        {
            return Half - strength + Half;
        }

    }
    public enum TraitType
    {
        Aggression,
        Warmth,
        Openness,
        Toughness,
        Neuroticism,
        Fearfulness

    }

    public enum CorrelationStrength
    {
        Weak,
        Strong
    }
}
