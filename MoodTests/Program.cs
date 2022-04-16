
using MoodTests;


var person2 = new Person(new Personality());

double lastFear = 6;
double lastjoy = 6;

for (int i = 0; i < 10; i++)
{
    person2.Mood(MoodAxis.Joy).UpdateMood(-1.66);

    printMood(person2.Mood(MoodAxis.Joy), lastjoy);
    printMood(person2.Mood(MoodAxis.Fear), lastFear);
    lastjoy = person2.Mood(MoodAxis.Joy).Value;
    lastFear = person2.Mood(MoodAxis.Fear).Value;
    Console.WriteLine();
    Console.WriteLine();

}

for (int i = 0; i < 10; i++)
{
    person2.Mood(MoodAxis.Fear).UpdateMood(-1);
    printMood(person2.Mood(MoodAxis.Joy), lastjoy);
    printMood(person2.Mood(MoodAxis.Fear), lastFear);
    lastjoy = person2.Mood(MoodAxis.Joy).Value;
    lastFear = person2.Mood(MoodAxis.Fear).Value;
    Console.WriteLine();
    Console.WriteLine();
}

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