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
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //joy -curiosity
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //curiosity +joy

            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //joy  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //curiosity ~+joy

            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-joy

            curiosity.LinkMoodsBidirectional(joy,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //joy --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //curiosity -joy

            //curious - fear
            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //fear -curiosity
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //curiosity +fear

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //fear  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //curiosity ~+fear

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //fear +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-fear

            curiosity.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //fear --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //curiosity -fear

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
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //connection -fear
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //fear +connection

            fear.LinkMoodsBidirectional(connection,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //connection  +fear
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //fear ~+connection

            fear.LinkMoodsBidirectional(connection,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //connection +fear
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear ~-connection

            fear.LinkMoodsBidirectional(connection,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //connection --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //fear -connection

            //fear - joy
            fear.LinkMoodsBidirectional(joy,
             new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //joy -fear
             new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //fear +joy

            fear.LinkMoodsBidirectional(joy,
             new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //joy  +fear
             new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //fear ~+joy

            fear.LinkMoodsBidirectional(joy,
             new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //joy +fear
             new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear ~-joy

            fear.LinkMoodsBidirectional(joy,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //joy --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //fear -joy

            //curiosity - fear
            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //curiosity -fear
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //fear +curiosity

            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //curiosity  +fear
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //fear ~+curiosity

            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //curiosity +fear
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear ~-curiosity

            fear.LinkMoodsBidirectional(curiosity,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //curiosity --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //fear -curiosity

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
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //anger -curiosity
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //curiosity +anger

            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //anger  +curiosity
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //curiosity ~+anger

            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //anger +curiosity
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //curiosity ~-anger

            curiosity.LinkMoodsBidirectional(anger,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //anger --curiosity
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //curiosity -anger
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
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //fear -connection
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //connection +fear

            connection.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //fear  +connection
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //connection ~+fear

            connection.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //fear +connection
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //connection ~-fear

            connection.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //fear --connection
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //connection -fear

            //certainty - fear
            fear.LinkMoodsBidirectional(certainty,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //certainty -fear
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //fear +certainty

            fear.LinkMoodsBidirectional(certainty,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //certainty  +fear
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //fear ~+certainty

            fear.LinkMoodsBidirectional(certainty,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //certainty +fear
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //fear ~-certainty

            fear.LinkMoodsBidirectional(certainty,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //certainty --fear
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //fear -certainty

            //fear - joy
            joy.LinkMoodsBidirectional(fear,
                new Correlation(weakNegativeCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25)), //boredom -joy
                new Correlation(strongPositiveCorrelation, MoodVector.Min, MoodVector.GetPercentOfRange(25))); //despair +fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half), //security  +joy
                new Correlation(weakPositiveCorrelation, MoodVector.GetPercentOfRange(25), MoodVector.Half)); //sadness ~+fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(mediumPositiveCorrelation, MoodVector.Half, MoodVector.MidPositiveRange), //thrill +joy
                new Correlation(mediumNegativeCorrelation, MoodVector.Half, MoodVector.MidPositiveRange)); //happiness ~-fear

            joy.LinkMoodsBidirectional(fear,
                new Correlation(strongNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max), //terror --joy
                new Correlation(weakNegativeCorrelation, MoodVector.MidPositiveRange, MoodVector.Max)); //joy -fear

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
