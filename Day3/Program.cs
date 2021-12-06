using Shared;
using System.Collections;
using System.Text;

string[] input = await Utils.GetInput(2021, 3);

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    // 1st task
    {
        Console.WriteLine("1st Task:\n");

        var bitArrays = input.Select(x => new BitArray(x.Select(y => y == '1').ToArray()));
        int bitCount = input[0].Length;

        int[] zeroCount = new int[bitCount];
        int[] oneCount = new int[bitCount];

        foreach (var bits in bitArrays)
        {
            for (int x = 0; x < bits.Count; ++x)
            {
                if (bits[x]) oneCount[x]++;
                else zeroCount[x]++;
            }
        }

        char[] gammaBinaryCharArr = new char[zeroCount.Length];
        char[] epsilonBinaryCharArr = new char[zeroCount.Length];

        for (int i = 0; i < bitCount; i++)
        {
            gammaBinaryCharArr[i] = zeroCount[i] > oneCount[i] ? '0' : '1';
            epsilonBinaryCharArr[i] = zeroCount[i] < oneCount[i] ? '0' : '1';
        }

        var gammaBinaryString = new string(gammaBinaryCharArr);
        var epsilonBinaryString = new string(epsilonBinaryCharArr);
        Console.WriteLine("            gammaString: " + gammaBinaryString);
        Console.WriteLine("          epsilonString: " + epsilonBinaryString);
        uint gamma = Convert.ToUInt32(gammaBinaryString, 2);
        uint epsilon = Convert.ToUInt32(epsilonBinaryString, 2);
        uint epsilonBitwise = ((1 << 12) - 1) ^ gamma;
        Console.WriteLine("epsilonString (bitwise): " + Convert.ToString(epsilonBitwise, 2));
        uint powerConsumption = gamma * epsilon;
        uint powerConsumptionBitwise = gamma * epsilonBitwise;
        Console.WriteLine($"powerConsumption: {powerConsumption}");
        Console.WriteLine($"powerConsumption (bitwise): {powerConsumptionBitwise}");
    }

    // 2nd task
    {
        Console.WriteLine("\n2nd Task:\n");

        int oxygenGeneratorRating = Task2FindValue(input, (a, b) => a >= 0 && b == '1' || a < 0 && b == '0');
        int co2ScrubberRating = Task2FindValue(input, (a, b) => a < 0 && b == '1' || a >= 0 && b == '0');

        Console.WriteLine("oxygenGeneratorRating: " + oxygenGeneratorRating);
        Console.WriteLine("co2ScrubberRating: " + co2ScrubberRating);
        Console.WriteLine("life support rating: " + (oxygenGeneratorRating * co2ScrubberRating));
    }
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}

static int Task2FindValue(string[] input, Func<int, char, bool> conditions)
{
    int rowLength = input[0].Length;
    List<string> values = new(input);
    int[] mostLeastCommon = new int[rowLength];

    for (int x = 0; x < rowLength; x++)
    {
        List<string> newValues = new();

        for (int y = 0; y < values.Count; y++)
        {
            char bit = values[y][x];

            mostLeastCommon[x] = bit switch
            {
                '0' => --mostLeastCommon[x],
                '1' => ++mostLeastCommon[x],
                _ => throw new Exception($"Unexpected bit value {values[y][x]} at row {y} col {x}: '{values[y]}'"),
            };
        }

        for (int y = 0; y < values.Count; y++)
        {
            string row = values[y];

            if (conditions.Invoke(mostLeastCommon[x], row[x]))
            {
                newValues.Add(row);
            }
        }

        values = newValues;

        if (values.Count == 1) break;
        if (values.Count < 1) throw new Exception($"{nameof(values)}.Count < 1");
    }

    return Convert.ToInt32(values[0], 2);
}
