using Shared;
using System.Collections;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

int day = 5;
string[] input = await Utils.GetInput(2021, day);
//string[] input = await Utils.ReadInputFile($@".\InputFiles\input-2021-{day}-exampledata.txt");

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    Console.WriteLine("Day 5: Hydrothermal Venture\n");

    List<LineSegment> lineSegments = new();

    foreach (var row in input)
    {
        var endPoints = Regex.Split(row, @" * \-\> *").Select(x =>
        {
            int[] coordinates = Array.ConvertAll(x.Split(','), int.Parse);
            return new Vector2(coordinates[0], coordinates[1]);
        }).ToArray();

        lineSegments.Add(new LineSegment(endPoints));
    }

    Console.WriteLine("lineSegments.Count: " + lineSegments.Count);

    int maxY = (int)lineSegments.Max(ls => Math.Max(ls.EndPointA.Y, ls.EndPointB.Y)) + 1;
    int maxX = (int)lineSegments.Max(ls => Math.Max(ls.EndPointA.X, ls.EndPointB.X)) + 1;

    //// 1st task
    Console.WriteLine("\n1st Task:\n");
    {
        int[][] diagram = new int[maxY][];
        int[,] diagram2 = new int[maxY, maxX];

        for (int y = 0; y < diagram.Length; y++)
        {
            diagram[y] = new int[maxX];
        }

        foreach (var ls in lineSegments)
        {
            var d = Vector2.Abs(ls.EndPointA - ls.EndPointB);
            var min = Vector2.Min(ls.EndPointA, ls.EndPointB);
            var max = Vector2.Max(ls.EndPointA, ls.EndPointB);

            // only horizontal and vertical lines
            if (d.X > 0 && d.Y > 0) { continue; }

            float dist = Math.Max(d.X, d.Y);

            for (int i = 0; i <= dist; i++)
            {
                var lerp = Vector2.Lerp(ls.EndPointA, ls.EndPointB, i / dist).Round();

                diagram[(int)lerp.Y][(int)lerp.X]++;
            }
        }

        Console.WriteLine("height: " + diagram.Length + ", width: " + diagram[0].Length);
        Console.WriteLine("Points with 2 overlapping lines: " + diagram.SelectMany(x => x).Count(x => x >= 2));
    }

    Console.WriteLine("\n" + new string('-', 30));

    // 2nd task
    Console.WriteLine("\n2nd Task:\n");
    {
        int[][] diagram = new int[maxY][];

        for (int y = 0; y < diagram.Length; y++)
        {
            diagram[y] = new int[maxX];
        }

        foreach (var ls in lineSegments)
        {
            var d = Vector2.Abs(ls.EndPointA - ls.EndPointB);
            float dist = Math.Max(d.X, d.Y);
            HashSet<Vector2> lerps = new();

            for (int i = 0; i <= dist; i++)
            {
                var lerp = Vector2.Lerp(ls.EndPointA, ls.EndPointB, i / dist).Round();
                lerps.Add(lerp);
            }

            foreach (var lerp in lerps)
            {
                diagram[(int)lerp.Y][(int)lerp.X]++;
            }
        }

        Console.WriteLine("height: " + diagram.Length + ", width: " + diagram[0].Length);
        Console.WriteLine("Points with 2 overlapping lines: " + diagram.SelectMany(x => x).Count(x => x >= 2));
    }
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}

internal static class Extensions
{
    public static Vector2 Round(this Vector2 vector2) => new Vector2(
        (float)Math.Round((decimal)vector2.X),
        (float)Math.Round((decimal)vector2.Y));
}

internal record LineSegment
{

    //public LineSegment(Vector2 endPointA, Vector2 endPointB)
    public LineSegment(Vector2[] endPoints)
    {
        EndPointA = endPoints[0];
        EndPointB = endPoints[1];
    }

    public Vector2 EndPointA { get; set; }
    public Vector2 EndPointB { get; set; }
}
