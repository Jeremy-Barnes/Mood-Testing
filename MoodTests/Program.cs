
using MoodTests;

var traits = new List<PersonalityTrait>()
{
    new PersonalityTrait(PersonalityTraitType.Aggression, 3.5),
    new PersonalityTrait(PersonalityTraitType.Toughness, 3.75),
    new PersonalityTrait(PersonalityTraitType.Warmth, 2.6),
    new PersonalityTrait(PersonalityTraitType.Fearfulness, 2.5),
    new PersonalityTrait(PersonalityTraitType.Neuroticism, 1.8),
    new PersonalityTrait(PersonalityTraitType.Openness, 3.25)

};
var person2 = new Person(new Personality(traits));

Console.WriteLine(person2.ToString());
Console.WriteLine("--------- Fear + 3 Joy + 3");
person2.Mood(MoodAxis.Fear).UpdateMood(3);
person2.Mood(MoodAxis.Joy).UpdateMood(3);
Console.WriteLine(person2.ToString());
Console.WriteLine("--------- Anger + 5");
person2.Mood(MoodAxis.Anger).UpdateMood(5);
Console.WriteLine(person2.ToString());
Console.WriteLine("--------- Joy + 4");
person2.Mood(MoodAxis.Joy).UpdateMood(4);
Console.WriteLine(person2.ToString());

for (int i = 0; i < 5; i++)
{
    Console.WriteLine("--------- Ticke");
    person2.Mood(MoodAxis.Joy).UpdateMood(0);
    person2.Mood(MoodAxis.Fear).UpdateMood(0);
    person2.Mood(MoodAxis.Connection).UpdateMood(0);
    person2.Mood(MoodAxis.Anger).UpdateMood(0);
    person2.Mood(MoodAxis.Certainty).UpdateMood(0);
    person2.Mood(MoodAxis.Curiosity).UpdateMood(0);
    Console.WriteLine(person2.ToString());
}

//double lastFear = 6;
//double lastjoy = 6;


//for (int i = 0; i < 6; i++)
//{

//    printMood(person2.Mood(MoodAxis.Joy), lastjoy);
//    printMood(person2.Mood(MoodAxis.Fear), lastFear);
//    lastjoy = person2.Mood(MoodAxis.Joy).Value;
//    lastFear = person2.Mood(MoodAxis.Fear).Value;
//    Console.WriteLine();
//    Console.WriteLine();

//}


//lastFear = person2.Mood(MoodAxis.Fear).Value;
//lastjoy = person2.Mood(MoodAxis.Joy).Value;

//for (int i = 0; i < 6; i++)
//{
//    person2.Mood(MoodAxis.Connection).UpdateMood(-.66);

//    printMood(person2.Mood(MoodAxis.Joy), lastjoy);
//    printMood(person2.Mood(MoodAxis.Connection), lastFear);
//    lastjoy = person2.Mood(MoodAxis.Joy).Value;
//    lastFear = person2.Mood(MoodAxis.Connection).Value;
//    Console.WriteLine();
//    Console.WriteLine();

//}

//for (int i = 0; i < 10; i++)
//{
//    person2.Mood(MoodAxis.Connection).UpdateMood(1);
//    printMood(person2.Mood(MoodAxis.Joy), lastjoy);
//    printMood(person2.Mood(MoodAxis.Connection), lastFear);
//    lastjoy = person2.Mood(MoodAxis.Joy).Value;
//    lastFear = person2.Mood(MoodAxis.Connection).Value;
//    Console.WriteLine();
//    Console.WriteLine();
//}

void printMood(MoodVector mood, double lastValue)
{
    Console.ResetColor();
    var delta = mood.Value - lastValue;
    var CColor = ConsoleColor.White;
    if (delta > 0)
    {
        CColor = ConsoleColor.Green;
    }
    else if (delta < 0)
    {
        CColor = ConsoleColor.Red;
    }
    Console.Write(mood);
    Console.ForegroundColor = CColor;
    Console.Write($"   {delta.ToString("##.##").PadRight(5)} \r\n");
}