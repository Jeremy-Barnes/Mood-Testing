
using MoodTests;


var person2 = new Person(new Personality());
Console.WriteLine($"{person2.Mood(MoodAxis.Joy)}  {person2.Mood(MoodAxis.Fear)}");


for (int i = 0; i < 6; i++)
{
    person2.Mood(MoodAxis.Fear).UpdateMood(.66);
    Console.WriteLine($"{person2.Mood(MoodAxis.Joy)}  {person2.Mood(MoodAxis.Fear)}");
}

for (int i = 0; i < 10; i++)
{
    person2.Mood(MoodAxis.Fear).UpdateMood(-1);
    Console.WriteLine($"{person2.Mood(MoodAxis.Joy)}  {person2.Mood(MoodAxis.Fear)}");
}