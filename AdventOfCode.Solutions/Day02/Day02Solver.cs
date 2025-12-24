using Mono.CompilerServices.SymbolWriter;

namespace AdventOfCode.Solutions.Day02;

public class Day02Tests
{
    private readonly Day02Solver _solver = new();
    [Test] public async Task Step1WithExample() => await _solver.ExecuteExample1("2");
    [Test] public async Task Step2WithExample() => await _solver.ExecuteExample2("4");
    [Test] public void Step1WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle1());
    [Test] public void Step2WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle2());
}

public class Day02Solver : SolverBase
{
    List<int[]> _data = new();

    protected override void Parse(List<string> data)
    {
        _data = data.Select(line =>
                        line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToArray()
                    ).ToList();
    }

    protected override object Solve1()
    {
        int sum = 0;

        foreach (var report in _data)
        {
            bool safe = TestReport(report);

            if (safe)
            {
                sum++;
            }
        }

        return  sum;
    }

    protected override object Solve2()
    {
        int sum = 0;

        foreach (var report in _data)
        {
            if (TestReport(report))
            {
                sum++;
                continue;
            }

            bool safeWithRemoval = false;

            for (int i = 0; i < report.Length; i++)
            {
                // remove one level from the problematic report and retest for safety
                int[] modifiedReport = report.Where((_,j) => j != i).ToArray();

                if (TestReport(modifiedReport))
                {
                    safeWithRemoval = true;
                    break;
                }
            }

            if (safeWithRemoval)
            {
                sum++;
            }
        }

        return  sum;
    }

    private enum Directionality
    {
        increasing,
        decreasing
    }

    private Boolean TestReport(int[] _report)
    {
        bool safe = true;

        // test for directionality
        Directionality directionality = TestDirectionality(_report[0], _report[1]);

        for (int i = 0; i < _report.Length - 1; i++)
        {
            // test for difference between levels
            safe = TestLevelDifference(_report[i], _report[i+1], safe);

            // test for continual increase/decrease
            safe = TestContinualChange(_report[i], _report[i+1], directionality, safe);
        }

        return safe;
    }

    private Directionality TestDirectionality(int _position1, int _position2)
    {
        Directionality directionality;
        switch (_position1 < _position2)
        {
            case (true):
                directionality = Directionality.increasing;
                break;
            
            case (false):
                directionality = Directionality.decreasing;
                break;
        }

        return directionality;
    }

    private Boolean TestLevelDifference(int _currentPosition, int _nextPosition, bool _safe)
    {
        int difference = Math.Abs(_nextPosition - _currentPosition);

        if (difference < 1 || difference > 3)
        {
            _safe = false;
        }

        return _safe;
    }

    private Boolean TestContinualChange(int _currentPosition, int _nextPosition, Directionality _directionality, bool _safe)
    {
        switch (_directionality)
        {
            case (Directionality.increasing):
                if (_currentPosition > _nextPosition)
                {
                    _safe = false;
                }
                break;

            case (Directionality.decreasing):
                if (_currentPosition < _nextPosition)
                {
                    _safe = false;
                }
                break;
        }

        return _safe;
    }
}