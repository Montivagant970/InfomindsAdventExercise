namespace AdventOfCode.Solutions.Day03;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

public class Day03Tests
{
    private readonly Day03Solver _solver = new();
    [Test] public async Task Step1WithExample() => await _solver.ExecuteExample1("161");
    [Test] public async Task Step2WithExample() => await _solver.ExecuteExample2("48");
    [Test] public void Step1WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle1());
    [Test] public void Step2WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle2());
}

public class Day03Solver : SolverBase
{
    string _data = "";

    protected override void Parse(List<string> data)
    {
        _data = string.Join("", data);
    }

    protected override object Solve1()
    {
        int sum = 0;

        foreach (Match match in Regex.Matches(_data, @"mul\((\d{1,3}),(\d{1,3})\)"))
        {
            sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
        }

        return sum;
    }

    protected override object Solve2()
    {
        bool doEnabled = true;
        int sum = 0;

        foreach (Match match in Regex.Matches(_data, @"do\(\)|don't\(\)|mul\((\d{1,3}),(\d{1,3})\)"))
        {
            if (match.Value == "do()")
            {
                doEnabled = true;
            }
            else if (match.Value == "don't()")
            {
                doEnabled = false;
            }
            else if (match.Value.StartsWith("mul(") && doEnabled)
            {
                sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
            }
        }
        return sum;
    }

}