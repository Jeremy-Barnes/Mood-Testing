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

        private double ValueCorrelation => Value >= Half ? (Value / Max) : (Half / Value);

        public double strongPositiveCorrelation { get { return .4 / (ValueCorrelation); } }
        public double mediumPositiveCorrelation { get { return .2 / (ValueCorrelation); } }
        public double weakPositiveCorrelation { get { return .025 / (ValueCorrelation); } } //V/M

        public double strongNegativeCorrelation { get { return -.475 * (ValueCorrelation); } } // M/V
        public double mediumNegativeCorrelation { get { return -.075 * (ValueCorrelation); } }
        public double weakNegativeCorrelation { get { return -.0125 * (ValueCorrelation); } }

        public PersonalityTrait(PersonalityTraitType type)
        {
            TraitType = type;
        }

        public PersonalityTraitType TraitType { get; }
        public double Value { get; set; } = 3.5;

        public void LinkMoods(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);
            switch (TraitType)
            {
                case PersonalityTraitType.Aggression:
                    {
                        linkAggressionWithAnger(person);
                        break;
                    }
                case PersonalityTraitType.Toughness:
                    {
                        linkToughnessWithFear(person);
                        break;
                    }
                case PersonalityTraitType.Warmth:
                    {
                        linkWarmthWithJoy(person);
                        break;
                    }
                case PersonalityTraitType.Fearfulness:
                    {
                        linkFearfulness(person);
                        break;
                    }
                case PersonalityTraitType.Neuroticism:
                    {
                        linkNeuroticismWithAnger(person);
                        linkNeuroticismWithFear(person);
                        break;
                    }
                case PersonalityTraitType.Openness:
                    {
                        linkOpennnessWithCuriosity(person);
                        break;
                    }
            }
        }

        private void linkAggressionWithAnger(Person person)
        {
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);

            //anger - certainty
            anger.LinkMoodsBidirectional(certainty,
                new Correlation(mediumPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //chaos +anger
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //passivity ~-certainty

            anger.LinkMoodsBidirectional(certainty,
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //uncertainty  ~+anger
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //calm ~+certainty

            anger.LinkMoodsBidirectional(certainty,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //certainty ~+anger
                new Correlation(Value >= Half ? mediumPositiveCorrelation : mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //anger +certainty (for some)

            anger.LinkMoodsBidirectional(certainty,
                new Correlation(Value >= Half ? mediumPositiveCorrelation : weakPositiveCorrelation, MidPositiveRange, MoodVector.Max), //conviction +anger
                new Correlation(Value >= Half ? strongPositiveCorrelation : weakPositiveCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //rage +certainty

            //anger - joy
            anger.LinkMoodsBidirectional(joy,
                new Correlation(mediumPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //despair +anger
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //passivity ~-joy

            anger.LinkMoodsBidirectional(joy,
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //sadness  ~+anger
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //calm ~+joy

            anger.LinkMoodsBidirectional(joy,
                new Correlation(Value >= Half ? mediumNegativeCorrelation : weakNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy -anger
                new Correlation(Value >= Half ? mediumPositiveCorrelation : mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //anger +joy (for some)

            anger.LinkMoodsBidirectional(joy,
                new Correlation(Value >= Half ? strongNegativeCorrelation : mediumNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max),//++joy --anger
                new Correlation(Value >= Half ? weakPositiveCorrelation : strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //rage +joy


            //fear - anger
            anger.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //despair +anger
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //passivity ~-joy

            anger.LinkMoodsBidirectional(fear,
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //sadness  ~+anger
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //calm ~+joy

            anger.LinkMoodsBidirectional(fear,
                new Correlation(Value >= Half ? mediumNegativeCorrelation : weakNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy -anger
                new Correlation(Value >= Half ? mediumPositiveCorrelation : mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //anger +joy (for some)

            anger.LinkMoodsBidirectional(fear,
                new Correlation(Value >= Half ? strongNegativeCorrelation : mediumNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max),//++joy --anger
                new Correlation(Value >= Half ? weakPositiveCorrelation : strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //rage +~joy (for some)

        }

        private void linkWarmthWithJoy(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);

            //connect - joy
            int introvertBoundary = (int)Math.Round(7 * (Max - Value)) + 5; //because everyone likes a little alone time
            var introvertThreshold = MoodVector.GetPercentOfRange(50 - introvertBoundary);

            joy.LinkMoodsBidirectional(connection,
              new Correlation(strongNegativeCorrelation, MoodVector.Min, introvertThreshold), //isolation ---joy
              new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //despair -connection

            joy.LinkMoodsBidirectional(connection,
                new Correlation(weakPositiveCorrelation, introvertThreshold, MoodVector.Half), //alone  ~+joy
                new Correlation(weakNegativeCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //sadness ~-connection

            joy.LinkMoodsBidirectional(connection,
                new Correlation(Value < Half ? weakNegativeCorrelation : mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //friends  +joy (for some)
                new Correlation(weakPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //joy ~+connection


            joy.LinkMoodsBidirectional(connection,
                new Correlation(strongPositiveCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //bestfriends  ++joy
                new Correlation(strongPositiveCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //mania +connection


            // joy - anger
            anger.LinkMoodsBidirectional(joy,
                new Correlation(weakPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //despair +~anger
                new Correlation(weakPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //passivity ~+joy

            anger.LinkMoodsBidirectional(joy,
                new Correlation(weakNegativeCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //sadness  -~anger maybe null this one
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //calm +joy 

            anger.LinkMoodsBidirectional(joy,
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy -anger
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //anger -joy

            anger.LinkMoodsBidirectional(joy,
                new Correlation(Value >= Half ? mediumPositiveCorrelation : weakPositiveCorrelation, MidPositiveRange, MoodVector.Max), //rage -anger
                new Correlation(Value >= Half ? strongPositiveCorrelation : weakPositiveCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //rage -joy
        }

        private void linkToughnessWithFear(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);

            int thrillTerrorBoundary = (int)Math.Round(13 * (Value - Half));
            var thrillTerrorThreshold = Math.Max(MoodVector.Half, MoodVector.GetPercentOfRange(70 + thrillTerrorBoundary));

            //fear - certainty
            certainty.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //chaos +fear
                new Correlation(weakPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +certainty

            certainty.LinkMoodsBidirectional(fear,
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //uncertainty  +~fear
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //security +certainty

            certainty.LinkMoodsBidirectional(fear,
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //certainty -fear
                new Correlation(weakPositiveCorrelation, MoodVector.Half, thrillTerrorThreshold)); //thrill ~+certainty

            certainty.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, thrillTerrorThreshold, MoodVector.Max), //conviction --fear
                new Correlation(mediumNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //terror +-certainty


            //fear - joy
            joy.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //boredom -joy
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //despair +fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //security  +joy
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //sadness ~+fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, thrillTerrorThreshold), //thrill +joy
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //happiness ~-fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, thrillTerrorThreshold, MoodVector.Max), //terror --joy
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //joy -fear
        }









        private void linkOpennnessWithCuriosity(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);

            //curious - certainty
            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //chaos -curiosity
                new Correlation(mediumPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +certainty

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //uncertainty  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //disinterest ~+certainty

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //certainty +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-certainty

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //conviction --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //intense curiosity -certainty

            //curious - joy
            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //despair -curiosity
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +joy

            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //sadness  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //curiosity ~+joy

            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-joy

            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //ecstasy --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //manic curiosity -joy

            //curious - fear
            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //boredom -curiosity
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //complete disinterest +fear

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //security  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //disinterest ~+fear

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //fear/thrill +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-fear

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //terror --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //manic curiosity -fear

        }

        private void linkNeuroticismWithFear(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);
            //fear - connection
            fear.LinkMoodsBidirectional(connection,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //isolation -fear
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +connection

            fear.LinkMoodsBidirectional(connection,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //aloneness  +fear
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //security ~+connection

            fear.LinkMoodsBidirectional(connection,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //connection +fear
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear/thrill ~-connection

            fear.LinkMoodsBidirectional(connection,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //intense connection --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //terror -connection

            //fear - joy
            fear.LinkMoodsBidirectional(joy,
             new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //despair -fear
             new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +joy

            fear.LinkMoodsBidirectional(joy,
             new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //sadness  +fear
             new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //security ~+joy

            fear.LinkMoodsBidirectional(joy,
             new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy +fear
             new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear/thrill ~-joy

            fear.LinkMoodsBidirectional(joy,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //mania --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //terror -joy

            //curiosity - fear
            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //complete disinterest -fear
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +curiosity

            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //disinterest  +fear
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //security ~+curiosity

            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //curiosity +fear
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear/thrill ~-curiosity

            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //mania --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //terror -curiosity

        }

        private void linkNeuroticismWithAnger(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);

            //curiosity - anger
            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //zen -curiosity
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +anger

            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //calm  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //disinterest ~+anger

            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //anger +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-anger

            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //rage --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //mania -anger
        }

        private void linkFearfulness(Person person)
        {
            var connection = person.Mood(MoodAxis.Connection);
            var fear = person.Mood(MoodAxis.Fear);
            var joy = person.Mood(MoodAxis.Joy);
            var anger = person.Mood(MoodAxis.Anger);
            var certainty = person.Mood(MoodAxis.Certainty);
            var curiosity = person.Mood(MoodAxis.Curiosity);

            //connection - fear
            connection.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //boredom -connection
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //isolation +fear

            connection.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //security  +connection
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //aloneness ~+fear

            connection.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //fear/thrill +connection
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //connection ~-fear

            connection.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //terror --connection
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //MDMA -fear

            //certainty - fear
            fear.LinkMoodsBidirectional(certainty,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //chaos -fear
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //boredom +certainty

            fear.LinkMoodsBidirectional(certainty,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //uncertainty  +fear
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //security ~+certainty

            fear.LinkMoodsBidirectional(certainty,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //certainty +fear
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear/thrill ~-certainty

            fear.LinkMoodsBidirectional(certainty,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //conviction --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //terror -certainty

            //fear - joy
            joy.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //boredom -joy
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //despair +fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //security  +joy
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //sadness ~+fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //fear/thrill +joy
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //happiness ~-fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //terror --joy
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //mania -fear

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
