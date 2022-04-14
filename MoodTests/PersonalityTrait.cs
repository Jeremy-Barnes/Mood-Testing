using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodTests
{
    public class Personality
    {
        public List<PersonalityTrait> Traits { get; set; } = new List<PersonalityTrait>();
        public Personality()
        {
            Traits = Enum.GetValues<PersonalityTraitType>().Select(type => new PersonalityTrait(type)).ToList();
        }
        public PersonalityTrait Trait(PersonalityTraitType type)
        {
            return Traits.Where(m => m.TraitType == type).FirstOrDefault();
        }

        public void LinkMoods(Person person)
        {
            foreach (var trait in Traits)
            {
                trait.LinkMoods(person);
            }
        }
    }

    public class PersonalityTrait
    {
        public const double Max = 5;
        public const double Min = 1;
        public const double Half = (Max + Min) / 2;
        public const double MidPositiveRange = (Max + Half) / 2;
        public const double MidNegativeRange = (Min + Half) / 2;

        public PersonalityTrait(PersonalityTraitType type)
        {
            TraitType = type;
        }

        public PersonalityTraitType TraitType { get; }
        public double Value { get; set; } = 2.1;
        //public double ValuePercentOfScale => (Value - Half) / Max;

        public void LinkMoods(Person person)
        {
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            switch (TraitType)
            {
                case PersonalityTraitType.Aggression:
                    {
                        break;
                    }
                case PersonalityTraitType.Toughness:
                    {
                        int thrillTerrorBoundary = (int)Math.Round(10 * (Value - Half));
                        //boredom -fear -joy
                        joy.LinkMoodsBidirectional(fear, new Correlation(-.005/(Value/Max), MoodVector.GetPercentOfRange(0), MoodVector.GetPercentOfRange(25)), null);
                        //security -fear +joy
                        joy.LinkMoodsBidirectional(fear, new Correlation(.10/(Value / Max), MoodVector.GetPercentOfRange(25), MoodVector.Half), null);
                        //thrill +fear +joy
                        joy.LinkMoodsBidirectional(fear, new Correlation(.125/ (Value / Max), MoodVector.Half, MoodVector.GetPercentOfRange(60 + thrillTerrorBoundary)), null);
                        //terror +fear -joy
                        joy.LinkMoodsBidirectional(fear, new Correlation(-.175 / (Value / Max), MoodVector.GetPercentOfRange(60 + thrillTerrorBoundary), MoodVector.Max), null);
                        break;
                    }
                case PersonalityTraitType.Warmth:
                    {
                        break;
                    }
                case PersonalityTraitType.Fearfulness:
                    {
                        break;
                    }
                case PersonalityTraitType.Neuroticism:
                    {
                        break;
                    }
                case PersonalityTraitType.Openness:
                    {
                        break;
                    }
            }
        }

        private double convertValueToVector()
        {
            return ((MoodVector.Max + MoodVector.Min) / (PersonalityTrait.Max + PersonalityTrait.Min)) * Value;
        }
    }

    public enum PersonalityTraitType
    {
        Aggression,
        Toughness,
        Warmth,
        Fearfulness,
        Neuroticism,
        Openness
    }

}
