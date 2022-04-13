using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodTests
{
    public class Person
    {
        public List<MoodVector> Moods { get; set; }
        public Personality Personality { get; set; }

        public Person(Personality personality)
        {
            Moods = Enum.GetValues<MoodAxis>().Select(axis => new MoodVector(axis)).ToList();
            Personality = personality;
            Personality.LinkMoods(this);
        }


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

}
