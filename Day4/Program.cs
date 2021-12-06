using Shared;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

string[] input = await Utils.GetInput(2021, 4);
//string[] input = await Utils.ReadInputFile(@".\InputFiles\input-2021-4-exampledata.txt");

if (input == null)
{
    Console.WriteLine("[Error]: input == null\n");
    return;
}

try
{
    Console.WriteLine("Day 4: Giant Squid\n");
    Console.WriteLine("input.Length: " + input.Length);

    int[] randomNumbers = Array.ConvertAll(input[0].Split(','), int.Parse);
    Console.WriteLine($"randomNumbers.Length: {string.Join(',', randomNumbers.Length)}");
    Console.WriteLine($"randomNumbers: {string.Join(',', randomNumbers)}");

    // Join all rows (except the first 2) to a string (with '\n' as the separator).
    // Next split the string into a string[] (with @"\n{2,}" as the separator (two or more '\n')) to get a string[] of all boards.
    // Next split each board's string into a string[] (array of the board's rows) and ...
    // Finally split each board's (row) string at any whitespace and parse each element to an 'int' and create a Cell object.
    Board[] boards = Regex.Split(string.Join('\n', input.Skip(2)).Trim(), @"\n{2,}")
        .Select(x => new Board(x))
        .ToArray();
    Console.WriteLine("boardCount: " + boards.Length);

    // 1st task
    {
        Console.WriteLine("1st Task:\n");

        List<(int number, Board board)> winningBoards = new();
        int numbersDrawn = 0;

        foreach (int number in randomNumbers)
        {
            numbersDrawn++;

            foreach (var board in boards)
            {
                for (int y = 0; y < board.Length; y++)
                {
                    Cell[] boardRow = board[y];

                    for (int x = 0; x < boardRow.Length; x++)
                    {
                        Cell cell = boardRow[x];
                        if (cell.Number == number)
                        {
                            cell.Match = true;

                            if (numbersDrawn < 5) continue;

                            // Check for victory
                            if (boardRow.All(cell => cell.Match) ||
                                board.All(row => row[x].Match))
                            {

                                winningBoards.Add((number, board));
                            }
                        }
                    }
                }
            }

            if (winningBoards.Count > 0)
            {
                Victory(winningBoards);
                break;
            }

            //await Task.Yield();
        }
    }

    Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

    // 2nd task
    {
        Console.WriteLine("2nd Task:\n");

        List<(int number, Board board)> winningBoards = new();
        int numbersDrawn = 0;

        foreach (int number in randomNumbers)
        {

            numbersDrawn++;

            foreach (var board in boards)
            {
                if (winningBoards.Any(wboard => wboard.board == board)) continue;

                for (int y = 0; y < board.Length; y++)
                {
                    Cell[] boardRow = board[y];

                    for (int x = 0; x < boardRow.Length; x++)
                    {
                        Cell cell = boardRow[x];

                        if (cell.Number != number) continue;

                        cell.Match = true;

                        if (numbersDrawn < 5) continue;

                        // Check for victory
                        if (boardRow.All(cell => cell.Match) ||
                            board.All(row => row[x].Match))
                        {

                            winningBoards.Add((number, board));
                        }
                    }
                }
            }

            if (winningBoards.Count == boards.Length)
            {
                // "Victory"
                Victory2(winningBoards);
                break;
            }

            //await Task.Yield();
        }
    }
}
catch (Exception e)
{
    Console.WriteLine($"\n{new string('=', 30)}");
    Console.WriteLine(e);
    Console.WriteLine($"{new string('=', 30)}");
    return;
}

void RenderBoard(Board board)
{
    foreach (Cell[] row in board)
    {
        foreach (Cell cell in row)
        {
            Console.ForegroundColor = cell.Match ? ConsoleColor.Red : ConsoleColor.White;
            Console.Write(cell.Number.ToString().PadLeft(3, ' '));
        }
        Console.WriteLine();
    }
    Console.ResetColor();
}

void Victory(List<(int number, Board board)> winningBoards)
{
    foreach (var winningBoard in winningBoards)
    {
        foreach (Cell[] row in winningBoard.board)
        {
            Console.WriteLine(string.Join(' ', row.Select(x => (x.Match ? "" : x.Number.ToString()).PadLeft(2, ' '))));
        }

        var a = winningBoard.board.SelectMany(x => x.Where(y => !y.Match));
        var b = a.Sum(x => x.Number);
        Console.WriteLine(b * winningBoard.number);
    }
}

void Victory2(List<(int number, Board board)> boards)
{
    Console.WriteLine("winningBoards.Count " + boards.Count);
    var lastBoard = boards.Last();

    RenderBoard(lastBoard.board);

    var a = lastBoard.board.SelectMany(x => x.Where(y => !y.Match));
    var b = a.Sum(x => x.Number);
    Console.WriteLine(b * lastBoard.number);
}

internal class Cell
{
    public int Number { get; set; }
    public bool Match { get; set; }

    public Cell(int number)
    {
        Number = number;
    }
}

internal class Board : IEnumerable<Cell[]>
{
    public Cell[][] Cells { get; set; }

    public int Length => Cells.Length;

    public Cell[] this[int index]
    {
        get
        {
            return Cells[index];
        }
    }

    public Board(string x)
    {
        Cells = x.Split('\n')
                 .Select((y, i) => Array.ConvertAll(Regex.Split(y.Trim(), @"\s+"), int.Parse)
                                        .Select(x => new Cell(x))
                                        .ToArray())
                 .ToArray();
    }

    public IEnumerator<Cell[]> GetEnumerator()
    {
        return ((IEnumerable<Cell[]>)Cells).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Cells.GetEnumerator();
    }
}
