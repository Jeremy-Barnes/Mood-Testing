// See https://aka.ms/new-console-template for more information

using MoodTests;

var person1 = new Person();
person1.Mood(MoodAxis.Joy).LinkMood(person1.Mood(MoodAxis.Fear), .15, (MoodVector.MidPositiveRange, MoodVector.Half), -.75);


for (int i = 0; i < 6; i++)
{
    Console.WriteLine($"{person1.Mood(MoodAxis.Joy)}  {person1.Mood(MoodAxis.Fear)}");
    person1.Mood(MoodAxis.Joy).UpdateMood(1);
    person1.Mood(MoodAxis.Fear).UpdateMood(1);

}

Console.WriteLine("\r\n\r\n\r\n");

//var person2 = new Person();
//person2.Mood(MoodAxis.Joy).LinkMood(person2.Mood(MoodAxis.Fear), .15, (MoodVector.MidPositiveRange, MoodVector.Half), -.75);

//for (int i = 0; i < 10; i++)
//{
//    Console.WriteLine($"{person2.Mood(MoodAxis.Joy)}  {person2.Mood(MoodAxis.Fear)}");
//    person2.Mood(MoodAxis.Fear).UpdateMood(-1);
//    person2.Mood(MoodAxis.Joy).UpdateMood(0);
//}


var person2 = new Person();
person2.Mood(MoodAxis.Joy).LinkMood(person2.Mood(MoodAxis.Fear), .15, new Correlation(MoodVector.MidPositiveRange, MoodVector.Half), -.75);

for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"{person2.Mood(MoodAxis.Joy)}  {person2.Mood(MoodAxis.Fear)}");
    person2.Mood(MoodAxis.Fear).UpdateMood(-1);
    person2.Mood(MoodAxis.Joy).UpdateMood(0);
}