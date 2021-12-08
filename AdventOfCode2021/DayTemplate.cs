using Shared;
using System.Collections;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2021;

internal class Day6 : DayBase
{
    public Day6(int year, int day, string title) : base(day) { }

    //public async Task<DayBase> Init()
    //{
    //    //string[] input = await Utils.GetInput(2021, day);
    //    string[] input = await Utils.ReadInputFile($@".\InputFiles\input-2021-{day}-exampledata.txt");

    //    if (input == null)
    //    {
    //        Console.WriteLine("[Error]: input == null\n");
    //        return;
    //    }

    //    try
    //    {
    //        Console.WriteLine("Day 4: Hydrothermal Venture\n");

    //        // 1st task
    //        Console.WriteLine("\n1st Task:\n");
    //        {
    //        }

    //        Console.WriteLine("\n" + new string('-', 30));

    //        // 2nd task
    //        Console.WriteLine("\n2nd Task:\n");
    //        {
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine($"\n{new string('=', 30)}");
    //        Console.WriteLine(e);
    //        Console.WriteLine($"{new string('=', 30)}");
    //        return;
    //    }

    //    return this;
    //}
}

internal interface IDay
{
}

internal abstract class DayBase
{
    public int Day { get; set; }

    public DayBase(int day)
    {
        Day = day;
    }
}
