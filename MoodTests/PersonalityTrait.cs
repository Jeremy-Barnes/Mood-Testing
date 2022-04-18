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
        public double Value { get; set; } = 1.5;

        public void LinkMoods(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
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
                        int thrillTerrorBoundary = (int)Math.Round(13 * (Value - Half));
                        var thrillTerrorThreshold = Math.Max(MoodVector.Half, MoodVector.GetPercentOfRange(70 + thrillTerrorBoundary));

                        joy.LinkMoodsBidirectional(fear,
                            new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //boredom -joy
                            new Correlation(mediumNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //despair ++fear

                        joy.LinkMoodsBidirectional(fear,
                            new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //security  +joy
                            new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //sadness ~+fear

                        joy.LinkMoodsBidirectional(fear,
                            new Correlation(mediumPositiveCorrelation, MoodVector.Half, thrillTerrorThreshold), //thrill +joy
                            new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //happiness ~-fear

                        joy.LinkMoodsBidirectional(fear,
                            new Correlation(strongNegativeCorrelation, thrillTerrorThreshold, MoodVector.Max), //terror --joy
                            new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //joy -fear
                        break;
                    }
                case PersonalityTraitType.Warmth:
                    {
                        int introvertBoundary = (int)Math.Round(7 * (Max - Value)) + 5; //because everyone likes a little alone time
                        var introvertThreshold = MoodVector.GetPercentOfRange(50 - introvertBoundary);

                        joy.LinkMoodsBidirectional(connection,
                          new Correlation(strongNegativeCorrelation, MoodVector.Min, introvertThreshold), //isolation ---joy
                          new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //despair -connection

                        joy.LinkMoodsBidirectional(connection,
                            new Correlation(weakPositiveCorrelation, introvertThreshold, MoodVector.Half), //alone  ~+joy
                            new Correlation(weakNegativeCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //sadness ~-connection

                        joy.LinkMoodsBidirectional(connection,
                            new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //friends  +joy
                            new Correlation(weakPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //joy ~+connection


                        joy.LinkMoodsBidirectional(connection,
                            new Correlation(strongPositiveCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //bestfriends  ++joy
                            new Correlation(strongPositiveCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //mania +connection
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

        private double ValueCorrelation => Value >= Half ? (Value / Max) : (Half / Value);

        public double strongPositiveCorrelation { get { return .4 / (ValueCorrelation); } }
        public double mediumPositiveCorrelation { get { return .2 / (ValueCorrelation); } }
        public double weakPositiveCorrelation { get { return .025 / (ValueCorrelation); } } //V/M

        public double strongNegativeCorrelation { get { return -.475 * (ValueCorrelation); } } // M/V
        public double mediumNegativeCorrelation { get { return -.075 * (ValueCorrelation); } }
        public double weakNegativeCorrelation { get { return -.0125 * (ValueCorrelation); } }
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
