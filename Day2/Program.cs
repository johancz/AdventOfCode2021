using Shared;

string[] input = await Utils.GetInput(2021, 2);

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    IEnumerable<(string command, int value)> commands = input.Select(x =>
    {
        var command = x.Split(' ');
        return (command[0], int.Parse(command[1]));
    });

    Console.WriteLine($"# of commands: {commands.Count()}");

    // 1st task
    {
        Console.WriteLine("\n1st Task:");

        int finalHorizontal = 0;
        int finalDepth = 0;

        foreach (var command in commands)
        {
            switch (command.command)
            {
                case "forward": finalHorizontal += command.value; break;
                case "up": finalDepth -= command.value; break;
                case "down": finalDepth += command.value; break;
                default:
                    throw new Exception($"Unexpected command \"{command.command}\" with value: {command.value}");
            }
        }

        Console.WriteLine($"Final Horizontal: {finalHorizontal}");
        Console.WriteLine($"Final Depth: {finalDepth}");
        Console.WriteLine($"Multiplied: {finalHorizontal * finalDepth}");
    }

    // 2nd task
    {
        Console.WriteLine("\n2nd Task:");

        int finalHorizontal = 0;
        int finalDepth = 0;
        int aim = 0;

        foreach (var command in commands)
        {
            switch (command.command)
            {
                case "forward": finalHorizontal += command.value; finalDepth += aim * command.value; break;
                case "up": aim -= command.value; break;
                case "down": aim += command.value; break;
                default:
                    throw new Exception($"Unexpected command \"{command.command}\" with value: {command.value}");
            }
        }

        Console.WriteLine($"Final Horizontal: {finalHorizontal}");
        Console.WriteLine($"Final Depth: {finalDepth}");
        Console.WriteLine($"Multiplied: {finalHorizontal * finalDepth}");
    }

    // timeToComplete: 18min
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}
