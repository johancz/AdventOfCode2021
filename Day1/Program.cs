using Shared;

string[] input = await Utils.GetInput(2021, 1);

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    int[] numbers = Array.ConvertAll(input, int.Parse);
    Console.WriteLine($"numbers.Length: {numbers.Length}\n");

    // part 1
    { 
        int largerCount = 0;

        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > numbers[i - 1])
            {
                largerCount++;
            }
        }

        Console.WriteLine($"[Part 1] Measurements larger than previous: {largerCount}");
    }

    // part 2
    {
        int slidingWindowSize = 3;
        int largerCount = 0;
        int[] previousSlidingWindow = new int[slidingWindowSize];

        Array.Copy(numbers, previousSlidingWindow, slidingWindowSize);

        for (int i = 1; i < numbers.Length - slidingWindowSize + 1; i++)
        {
            //Console.WriteLine($"i: {i}");

            int[] slidingWindow = new int[slidingWindowSize];
            Array.Copy(numbers, i, slidingWindow, 0, slidingWindowSize);

            if (slidingWindow.Sum() > previousSlidingWindow.Sum())
            {
                largerCount++;
            }

            previousSlidingWindow = slidingWindow;
        }

        Console.WriteLine($"[Part 2] Measurements larger than previous: {largerCount}");
    }

    // timeToComplete: ~25min
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}
