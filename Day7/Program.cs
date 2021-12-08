using Shared;
using System.Collections;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

int __year = 2021;
int __day = 7;
string __title = "The Treachery of Whales";
string[] input = await Utils.GetInput(__year, __day);
//string[] input = await Utils.ReadInputFile($@".\InputFiles\input-{__year}-{__day}-exampledata.txt");
//string[] input = new string[] { await Utils.GetExampleInput(__year, __day) };

int[] crabPositions = Array.ConvertAll(input[0].Split(","), int.Parse);

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    Console.WriteLine($"Day {__day}: {__year}\n");

    // 1st task
    Console.WriteLine("\n1st Task:\n");
    {
        int min = crabPositions.Min();
        int max = crabPositions.Max();

        int[] distancesCosts = new int[max - min + 1];

        for (int i = min; i <= max; i++)
        {
            for (int pos = 0; pos < crabPositions.Length; pos++)
            {
                int dist = Math.Abs(crabPositions[pos] - i);
                distancesCosts[i] += dist;
            }
        }
        int leastFuel = distancesCosts.Min();
        Console.WriteLine("(task1) least fuel: " + leastFuel);
    }

    Console.WriteLine("\n" + new string('-', 30));

    // 2nd task
    Console.WriteLine("\n2nd Task:\n");
    {
        int min = crabPositions.Min();
        int max = crabPositions.Max();

        int[] distancesCosts = new int[max - min + 1];

        for (int i = min; i <= max; i++)
        {
            for (int pos = 0; pos < crabPositions.Length; pos++)
            {
                int dist = Math.Abs(crabPositions[pos] - i);
                distancesCosts[i] += (dist * (dist + 1)) / 2;
            }
        }

        int leastFuel = distancesCosts.Min();
        Console.WriteLine("(task2) least fuel: " + leastFuel);
    }
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}
