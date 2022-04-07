using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodTests
{
    public class Person
    {
        public Person()
        {
            Moods = Enum.GetValues<MoodAxis>().Select(axis => new MoodVector(axis)).ToList();
        }

        public List<MoodVector> Moods { get; set; }
        public MoodVector Mood(MoodAxis axis)
        {
            return Moods.Where(m => m.Axis == axis).FirstOrDefault();
        }

    }
}
