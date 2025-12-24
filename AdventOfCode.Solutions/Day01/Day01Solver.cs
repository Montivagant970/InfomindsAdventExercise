namespace AdventOfCode.Solutions.Day01;

public class Day01Tests
{
    private readonly Day01Solver _solver = new();
    [Test] public async Task Step1WithExample() => await _solver.ExecuteExample1("11");
    [Test] public async Task Step2WithExample() => await _solver.ExecuteExample2("31");
    [Test] public void Step1WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle1());
    [Test] public void Step2WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle2());
}

public class Day01Solver : SolverBase
{
    List<(int Left, int Right)> _data = new();

    protected override void Parse(List<string> data)
    {
        _data = data.Select(line =>
        {
            var sides = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return (Left: int.Parse(sides[0]), Right: int.Parse(sides[1]));
        }).ToList();
    }
    
    /// <summary>
    /// Orders the two lists of numbers from the smallest to largest value, calculates the distance between the two numbers of the same line, and adds 
    /// their distance to a running total.
    /// </summary>
    /// <returns></returns>
    protected override object Solve1()
    {
        var (leftListNumbers, rightListNumbers) = SplitAndOrderData(_data);
        int sum = 0;

        for (int i = 0; i < leftListNumbers.Count; i++)
        {
            sum += Math.Abs(leftListNumbers[i] - rightListNumbers[i]);
        }

        return sum;
    }

    /// <summary>
    /// Multiplies the numbers in the left list by the times that they occur in the right list, adding the result to a running total. This is achieved
    /// through a dictionary which holds a count of the instances of each unique number in its list. 
    /// </summary>
    /// <returns></returns>
    protected override object Solve2()
    {
        var (leftListNumbers, rightListNumbers) = SplitAndOrderData(_data);
        Dictionary<int, int> rightListCounts = new Dictionary<int, int>();
        int sum = 0;
        
        for (int i = 0; i < rightListNumbers.Count; i++)
        {
            if (rightListCounts.ContainsKey(rightListNumbers[i]))
            {
                rightListCounts[rightListNumbers[i]] += 1;
            }
            else
            {
                rightListCounts[rightListNumbers[i]] = 1;
            }
        }

        foreach (int number in leftListNumbers)
        {
            if (rightListCounts.ContainsKey(number))
            {
                sum += number * rightListCounts[number];
            }
        }

        return sum;
    }

    private (List<int> left, List<int> right) SplitAndOrderData(List<(int Left, int Right)> _data)
    {
        var leftListNumbers = _data.Select(p => p.Left).OrderBy(x => x).ToList();
        var rightListNumbers = _data.Select(p => p.Right).OrderBy(x => x).ToList();

        return (leftListNumbers, rightListNumbers);
    }
}