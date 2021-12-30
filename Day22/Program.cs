using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Shared;
using Shared.Extensions;

//Stopwatch swTotal = new();
//swTotal.Restart();

Console.WriteLine("Day 22: Reactor Reboot");

string[] input = (await Utils.GetInput(2021, 22));
//string[] input = (await Utils.GetExampleInput(2021, 22, 2)).Trim(new char[] { ' ', '\n' }).Split('\n', StringSplitOptions.TrimEntries);

var steps = input.Select(row =>
{
    var rowSplit = row.Split(' ');
    var axis = Regex.Match(rowSplit[1], @"x\=(?<x1>-?\d+)..(?<x2>-?\d+),y\=(?<y1>-?\d+)..(?<y2>-?\d+),z\=(?<z1>-?\d+)..(?<z2>-?\d+)");
    return new Cube(
        rowSplit[0] == "on",
        new(axis.Groups["x1"].Value.ToInt(), axis.Groups["y1"].Value.ToInt(), axis.Groups["z1"].Value.ToInt()),
        new(axis.Groups["x2"].Value.ToInt(), axis.Groups["y2"].Value.ToInt(), axis.Groups["z2"].Value.ToInt())
    );
});

/* --- Task 1 ----------------------------------------------------------------------------------------------*/

{
    //Stopwatch swTask2 = new();
    //swTask2.Restart();

    Console.WriteLine();
    Console.WriteLine("Task 1:");

    var task1_steps = steps.Where(step =>
    {
        if (step.Start.X < -50 || step.Start.X > 50 || step.Start.Y < -50 || step.Start.Y > 50 ||
            step.Start.Z < -50 || step.Start.Z > 50 || step.End.X < -50 || step.End.X > 50 ||
            step.End.Y < -50 || step.End.Y > 50 || step.End.Z < -50 || step.End.Z > 50)
        {
            return false;
        }

        return true;
    }).ToArray();

    var minStart = new Vector3(task1_steps.Min(step => step.Start.X), task1_steps.Min(step => step.Start.Y), task1_steps.Min(step => step.Start.Z));
    var minEnd = new Vector3(task1_steps.Min(step => step.End.X), task1_steps.Min(step => step.End.Y), task1_steps.Min(step => step.End.Z));
    var offset = Vector3.Min(minStart, minEnd);
    var offsetLong = new Vector3Long(offset.X.ToInt(), offset.Y.ToInt(), offset.Z.ToInt());

    for (int i = 0; i < task1_steps.Length; i++)
    {
        Cube? step = task1_steps[i];
        task1_steps[i].Start -= offsetLong;
        task1_steps[i].End -= offsetLong;
    }

    var maxEnd2 = new Vector3(task1_steps.Max(step => step.End.X), task1_steps.Max(step => step.End.Y), task1_steps.Max(step => step.End.Z));
    var upperBound = (int)Math.Round(Math.Max(Math.Max(maxEnd2.X, maxEnd2.Y), maxEnd2.Z));

    var reactorCore = new ReactorCore(upperBound + 1, offset);

    foreach (var step in task1_steps)
    {
        long toggledCount = reactorCore.ToggleCuboids(step);
    }

    Console.WriteLine("Toggled cuboids: " + reactorCore.CountToggled());

    //swTask2.Stop();
    //Console.WriteLine("Task 2: " + swTask2.Elapsed + " ms.");

}

Console.WriteLine();

/* --- Task 2 ----------------------------------------------------------------------------------------------*/

{
    //Stopwatch swTask2 = new();
    //swTask2.Restart();

    Console.WriteLine("Task 2:");

    HashSet<Cube> cubes = new() { steps.First() };

    for (int i = 1; i < steps.Count(); i++)
    {
        var step = steps.ElementAt(i);
        HashSet<Cube> newCubes = new();

        foreach (Cube cube in cubes)
        {
            // intersect curent cube ('step') with all previous cubes and split if necessary
            newCubes.UnionWith(cube.GetAllPossibleCubes(step));
        }

        if (step.Mode)
        {
            // Add intersecting cube.
            newCubes.Add(step);
        }

        cubes = newCubes;
    }

    Console.WriteLine("Toggled cuboids: " + cubes.Aggregate(0, (long sum, Cube i) => sum + i.Volume));

    //swTask2.Stop();
    //Console.WriteLine("Task 2: " + swTask2.Elapsed + " ms.");
}

//swTotal.Stop();
//Console.WriteLine("Total: " + swTotal.Elapsed + " ms.");

/* --- functions, classes & Utils --------------------------------------------------------------------------*/

record Cube
{
    public bool Mode { get; set; }
    public Vector3Long Start { get; set; }
    public Vector3Long End { get; set; }
    public long Volume { get; set; }


    public Cube(bool mode, Vector3Long start, Vector3Long end)
    {
        Mode = mode;
        Start = start;
        End = end;
        CalculateVolume();
    }


    public void CalculateVolume()
    {
        //Stopwatch sw = new();
        //sw.Restart();

        Volume = (Math.Abs(End.X - Start.X) + 1) * (Math.Abs(End.Y - Start.Y) + 1) * (Math.Abs(End.Z - Start.Z) + 1);

        //sw.Stop();
        //Console.WriteLine("CalculateVolume: " + sw.Elapsed + " ms.");
    }

    /// <summary>
    /// Find where two cubes intersect, and return a cube which represents the intersection, or return 'null' if the cubes do not intersect.
    /// </summary>
    /// <param name="cubeB"></param>
    /// <returns>The intersection cube, or null if the cubes do not intersect.</returns>
    public Cube? Intersect(Cube cubeB)
    {
        //Stopwatch sw = new();
        //sw.Restart();

        if (cubeB.Start.X > End.X || cubeB.End.X < Start.X ||
            cubeB.Start.Y > End.Y || cubeB.End.Y < Start.Y ||
            cubeB.Start.Z > End.Z || cubeB.End.Z < Start.Z)
        { return null; }

        Cube newCube = new(
            true,
            start: new(
               Math.Max(Start.X, cubeB.Start.X),
               Math.Max(Start.Y, cubeB.Start.Y),
               Math.Max(Start.Z, cubeB.Start.Z)),
            end: new(
                Math.Min(End.X, cubeB.End.X),
                Math.Min(End.Y, cubeB.End.Y),
                Math.Min(End.Z, cubeB.End.Z))
        );

        //sw.Stop();
        //Console.WriteLine("Intersect: " + sw.Elapsed + " ms.");
        return newCube;
    }

    /// <summary>
    /// Splits a cube into multiple smaller cubes where it intersects with another cube ('intersect').
    /// </summary>
    /// <param name="intersect"></param>
    /// <returns></returns>
    public HashSet<Cube> Split(Cube intersect)
    {
        //Stopwatch sw = new();
        //sw.Restart();

        HashSet<Cube> cubes = new();

        #region Create cubes.
        // front
        if (intersect.Start.Z > this.Start.Z)
        {
            // top
            if (intersect.Start.Y > this.Start.Y)
            {
                // left
                if (intersect.Start.X > this.Start.X)
                {
                    cubes.Add(new Cube(true, new(this.Start.X, this.Start.Y, this.Start.Z), new(intersect.Start.X - 1, intersect.Start.Y - 1, intersect.Start.Z - 1)));
                }

                // center
                cubes.Add(new Cube(true, new(intersect.Start.X, this.Start.Y, this.Start.Z), new(intersect.End.X, intersect.Start.Y - 1, intersect.Start.Z - 1)));

                // right
                if (intersect.End.X < this.End.X)
                {
                    cubes.Add(new Cube(true, new(intersect.End.X + 1, this.Start.Y, this.Start.Z), new(this.End.X, intersect.Start.Y - 1, intersect.Start.Z - 1)));
                }
            }

            // middle y
            // left
            if (intersect.Start.X > this.Start.X)
            {
                cubes.Add(new Cube(true, new(this.Start.X, intersect.Start.Y, this.Start.Z), new(intersect.Start.X - 1, intersect.End.Y, intersect.Start.Z - 1)));
            }

            // middle y
            // center
            cubes.Add(new Cube(true, new(intersect.Start.X, intersect.Start.Y, this.Start.Z), new(intersect.End.X, intersect.End.Y, intersect.Start.Z - 1)));

            // middle y
            // right
            if (intersect.End.X < this.End.X)
            {
                cubes.Add(new Cube(true, new(intersect.End.X + 1, intersect.Start.Y, this.Start.Z), new(this.End.X, intersect.End.Y, intersect.Start.Z - 1)));
            }

            // bottom
            if (intersect.End.Y < this.End.Y)
            {
                // left
                if (intersect.Start.X > this.Start.X)
                {
                    cubes.Add(new Cube(true, new(this.Start.X, intersect.End.Y + 1, this.Start.Z), new(intersect.Start.X - 1, this.End.Y, intersect.Start.Z - 1)));
                }

                // center
                cubes.Add(new Cube(true, new(intersect.Start.X, intersect.End.Y + 1, this.Start.Z), new(intersect.End.X, this.End.Y, intersect.Start.Z - 1)));

                // right
                if (intersect.End.X < this.End.X)
                {
                    cubes.Add(new Cube(true, new(intersect.End.X + 1, intersect.End.Y + 1, this.Start.Z), new(this.End.X, this.End.Y, intersect.Start.Z - 1)));
                }
            }
        }

        // middle z
        // top
        if (intersect.Start.Y > this.Start.Y)
        {
            // left
            if (intersect.Start.X > this.Start.X)
            {
                cubes.Add(new Cube(true, new(this.Start.X, this.Start.Y, intersect.Start.Z), new(intersect.Start.X - 1, intersect.Start.Y - 1, intersect.End.Z)));
            }

            // center
            cubes.Add(new Cube(true, new(intersect.Start.X, this.Start.Y, intersect.Start.Z), new(intersect.End.X, intersect.Start.Y - 1, intersect.End.Z)));

            // right
            if (intersect.End.X < this.End.X)
            {
                cubes.Add(new Cube(true, new(intersect.End.X + 1, this.Start.Y, intersect.Start.Z), new(this.End.X, intersect.Start.Y - 1, intersect.End.Z)));
            }
        }

        // left
        if (intersect.Start.X > this.Start.X)
        {
            cubes.Add(new Cube(true, new(this.Start.X, intersect.Start.Y, intersect.Start.Z), new(intersect.Start.X - 1, intersect.End.Y, intersect.End.Z)));
        }

        // Skip intersect cube.

        // right
        if (intersect.End.X < this.End.X)
        {
            cubes.Add(new Cube(true, new(intersect.End.X + 1, intersect.Start.Y, intersect.Start.Z), new(this.End.X, intersect.End.Y, intersect.End.Z)));
        }

        // bottom
        if (intersect.End.Y < this.End.Y)
        {
            // left
            if (intersect.Start.X > this.Start.X)
            {
                cubes.Add(new Cube(true, new(this.Start.X, intersect.End.Y + 1, intersect.Start.Z), new(intersect.Start.X - 1, this.End.Y, intersect.End.Z)));
            }

            // center
            cubes.Add(new Cube(true, new(intersect.Start.X, intersect.End.Y + 1, intersect.Start.Z), new(intersect.End.X, this.End.Y, intersect.End.Z)));

            // right
            if (intersect.End.X < this.End.X)
            {
                cubes.Add(new Cube(true, new(intersect.End.X + 1, intersect.End.Y + 1, intersect.Start.Z), new(this.End.X, this.End.Y, intersect.End.Z)));
            }
        }

        // rear
        if (intersect.End.Z < this.End.Z)
        {
            // top
            if (intersect.Start.Y > this.Start.Y)
            {
                // left
                if (intersect.Start.X > this.Start.X)
                {
                    cubes.Add(new Cube(true, new(this.Start.X, this.Start.Y, intersect.End.Z + 1), new(intersect.Start.X - 1, intersect.Start.Y - 1, this.End.Z)));
                }

                // center
                cubes.Add(new Cube(true, new(intersect.Start.X, this.Start.Y, intersect.End.Z + 1), new(intersect.End.X, intersect.Start.Y - 1, this.End.Z)));

                // right
                if (intersect.End.X < this.End.X)
                {
                    cubes.Add(new Cube(true, new(intersect.End.X + 1, this.Start.Y, intersect.End.Z + 1), new(this.End.X, intersect.Start.Y - 1, this.End.Z)));
                }
            }

            // left
            if (intersect.Start.X > this.Start.X)
            {
                cubes.Add(new Cube(true, new(this.Start.X, intersect.Start.Y, intersect.End.Z + 1), new(intersect.Start.X - 1, intersect.End.Y, this.End.Z)));
            }

            // center
            cubes.Add(new Cube(true, new(intersect.Start.X, intersect.Start.Y, intersect.End.Z + 1), new(intersect.End.X, intersect.End.Y, this.End.Z)));

            // right
            if (intersect.End.X < this.End.X)
            {
                cubes.Add(new Cube(true, new(intersect.End.X + 1, intersect.Start.Y, intersect.End.Z + 1), new(this.End.X, intersect.End.Y, this.End.Z)));
            }

            // bottom
            if (intersect.End.Y < this.End.Y)
            {
                // left
                if (intersect.Start.X > this.Start.X)
                {
                    cubes.Add(new Cube(true, new(this.Start.X, intersect.End.Y + 1, intersect.End.Z + 1), new(intersect.Start.X - 1, this.End.Y, this.End.Z)));
                }

                // center
                cubes.Add(new Cube(true, new(intersect.Start.X, intersect.End.Y + 1, intersect.End.Z + 1), new(intersect.End.X, this.End.Y, this.End.Z)));

                // right
                if (intersect.End.X < this.End.X)
                {
                    cubes.Add(new Cube(true, new(intersect.End.X + 1, intersect.End.Y + 1, intersect.End.Z + 1), new(this.End.X, this.End.Y, this.End.Z)));
                }
            }
        }
        #endregion

        foreach (var cube in cubes)
        {
            cube.CalculateVolume();
        }

        //sw.Stop();
        //Console.WriteLine("GetAllPossibleCubes: " + sw.Elapsed + " ms.");
        return cubes;
    }

    public HashSet<Cube> GetAllPossibleCubes(Cube cubeB)
    {
        var newCubes = new HashSet<Cube>();
        var intersect = this.Intersect(cubeB);

        // No intersect, return the original cube.
        if (intersect == null)
        {
            newCubes.Add(this);
            return newCubes;
        }

        newCubes = Split(intersect);

        return newCubes;
    }
}

/// <summary>
/// For step 1.
/// </summary>
class ReactorCore
{
    public bool[][][] Cube { get; set; }
    public Vector3 Start { get; set; }
    public Vector3 End { get; set; }

    public ReactorCore(int upperBound, Vector3 offset)
    {
        Start = new(0, 0, 0);
        End = new(upperBound, upperBound, upperBound);

        Cube = new bool[upperBound][][];
        for (int iZ = 0; iZ < Cube.Length; iZ++)
        {
            Cube[iZ] = new bool[upperBound][];

            for (int iY = 0; iY < Cube[iZ].Length; iY++)
            {
                Cube[iZ][iY] = new bool[upperBound];
            }
        }
    }

    public long ToggleCuboids(Cube step)
    {
        long toggledCount = 0;

        for (long iZ = step.Start.Z; iZ <= step.End.Z; iZ++)
        {
            for (long iY = step.Start.Y; iY <= step.End.Y; iY++)
            {
                for (long iX = step.Start.X; iX <= step.End.X; iX++)
                {
                    if (Cube[iZ][iY][iX] != step.Mode)
                    {
                        toggledCount++;
                        Cube[iZ][iY][iX] = step.Mode;
                    }
                }
            }
        }

        return toggledCount;
    }

    internal long CountToggled()
    {
        long count = 0;
        for (long iZ = 0; iZ < Cube.Length; iZ++)
        {
            for (long iY = 0; iY < Cube[iZ].Length; iY++)
            {
                for (long iX = 0; iX < Cube[iZ][iY].Length; iX++)
                {
                    count += Cube[iZ][iY][iX] ? 1 : 0;
                }
            }
        }
        return count;
    }
}
