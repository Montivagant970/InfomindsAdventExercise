namespace AdventOfCode.Solutions.Day04;

public class Day04Tests
{
    private readonly Day04Solver _solver = new();
    [Test] public async Task Step1WithExample() => await _solver.ExecuteExample1("18");
    [Test] public async Task Step2WithExample() => await _solver.ExecuteExample2("9");
    [Test] public void Step1WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle1());
    [Test] public void Step2WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle2());
}

public class Day04Solver : SolverBase
{
    List<char[]> _data = new();

    protected override void Parse(List<string> data)
    {
        _data = data.Select(line => line.ToCharArray()).ToList();
    }

    /// <summary>
    /// Counts instances of the target string "XMAS" in any direction (horizontals, verticals, diagonals) in the given word puzzle.
    /// </summary>
    /// <returns></returns>
    protected override object Solve1()
    {
        const string target = "XMAS";
        int sum = 0;
        int height = _data.Count;
        int width = _data[0].Length;

        (int xDirection, int yDirection)[] directionality =
        {
            (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (0, -1), (1, -1)
        };
        
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (_data[y][x] != 'X')
                    continue;
                
                foreach (var (xDirection, yDirection) in directionality)
                {
                    if(CheckForMatch(x, y, xDirection, yDirection, target, width, height))
                    {
                        sum++;
                    }
                }
            }
        }
        
        return sum;
    }

    private bool CheckForMatch(int _x, int _y, int _xDirection, int _yDirection, string _target, int _width, int _height)
    {
        for (int i = 0; i < _target.Length; i++)
        {
            int xCheck = _x + _xDirection * i;
            int yCheck = _y + _yDirection * i;

            // respect the board boundaries
            if (xCheck < 0 || xCheck >= _width ||
                yCheck < 0 || yCheck >= _height)
            {
                return false;
            }

            // check for match at the letter level
            if (_data[yCheck][xCheck] != _target[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks for an X pattern of the string "MAS" on each of the two diagonals. If both diagonals spell out "MAS," the solution instance 
    /// is added to a running total.
    /// </summary>
    /// <returns></returns>
    protected override object Solve2()
    {
        int sum = 0;
        int height = _data.Count;
        int width = _data[0].Length;
        
        (int xHigh, int yHigh, int xLow, int yLow)[] diagonals = { (-1, 1, 1, -1), (1, 1, -1, -1) };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isMatch = true;
                
                if (_data[y][x] != 'A')
                    continue;
                
                foreach (var (xHigh, yHigh, xLow, yLow) in diagonals)
                {
                    if (!CheckForMatchDiagonals(x, y, xHigh, yHigh, xLow, yLow, height, width))
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    sum++;
                }
            }
        }

        return sum;
    }

    private bool CheckForMatchDiagonals(int _x, int _y, int _xHigh, int _yHigh, int _xLow, int _yLow, int _height, int _width)
    {
        int xHighCheck = _x + _xHigh;
        int yHighCheck = _y + _yHigh;
        int xLowCheck = _x + _xLow;
        int yLowCheck = _y + _yLow;

        // respect the board boundaries
        if (xHighCheck < 0 || xHighCheck >= _width || yHighCheck < 0 || yHighCheck >= _height)
            return false;
        if (xLowCheck < 0 || xLowCheck >= _width || yLowCheck < 0 || yLowCheck >= _height)
            return false;

        char highCharacter = _data[yHighCheck][xHighCheck];
        char lowCharacter = _data[yLowCheck][xLowCheck];

        // ensure the diagonals spell the desired word
        if ((highCharacter == 'M' && lowCharacter == 'S') ||
            (highCharacter == 'S' && lowCharacter == 'M'))
        {
            return true;
        }
        
        return false;
    }
}