using Shared;
using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

int __year = 2021;
int __day = 6;
string[] input = await Utils.GetInput(__year, __day);
//string[] input = await Utils.ReadInputFile($@".\InputFiles\input-{__year}-{__day}-exampledata.txt");
//string[] input = new string[] { await Utils.GetExampleInput(__year, __day) };

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    Console.WriteLine("Day 4: Hydrothermal Venture\n");

    Console.WriteLine($"Initial state: {string.Join(',', input)}");

    // 1st task
    Console.WriteLine("\n1st Task:\n");
    {

        List<int> school = Array.ConvertAll(input[0].Split(','), int.Parse).ToList();

        int days = 80;
        int day = 0;

        while (day < days)
        {
            day++;

            List<int> newFish = new();

            for (int i = 0; i < school.Count; i++)
            {
                int fish = school[i];

                if (fish == 0)
                {
                    newFish.Add(8);
                    school[i] = 6;
                }
                else
                {
                    school[i]--;
                }
            }

            school.AddRange(newFish);
            //Console.WriteLine($"After {day.ToString().PadLeft(2)} days: {string.Join(',', school)}");
        }

        Console.WriteLine($"After {days.ToString().PadLeft(2)} days there are: {school.Count} fish.");
    }

    Console.WriteLine("\n" + new string('-', 30));

    // 2nd task
    Console.WriteLine("\n2nd Task:\n");
    {
        //Dictionary<int, long> school = new()
        //{
        //    { 0, 0 },
        //    { 1, 0 },
        //    { 2, 0 },
        //    { 3, 0 },
        //    { 4, 0 },
        //    { 5, 0 },
        //    { 6, 0 },
        //    { 7, 0 },
        //    { 8, 0 },
        //};
        //foreach (int fish in Array.ConvertAll(input[0].Split(','), int.Parse))
        //{
        //    school[fish]++;
        //}
        var school = Array.ConvertAll(input[0].Split(','), int.Parse).GroupBy(x => x).ToDictionary(x => x.Key, x => (long)x.Count());
        for (int i = 0; i <= 8; ++i)
        {
            if (!school.ContainsKey(i))
            {
                school[i] = 0;
            }
        }

        if (school == null)
        {
            Console.WriteLine("[Error]: school == null\n");
            return;
        }

        int days = 256;
        int day = 0;

        while (day < days)
        {
            day++;
            Dictionary<int, long> newSchool = new(school);

            for (int i = 0; i < school.Count; i++)
            {
                long subSchool = school[i];

                if (i == 0)
                {
                    newSchool[8] += subSchool;
                    newSchool[6] += subSchool;
                }
                else
                {
                    newSchool[i - 1] += subSchool;
                }

                newSchool[i] -= subSchool;
            }

            school = newSchool;
        }

        Console.WriteLine($"After {day.ToString().PadLeft(2)} days there are: {school.Sum(x => x.Value)} fish.");
    }
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}
