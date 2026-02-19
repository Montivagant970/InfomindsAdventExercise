namespace AdventOfCode.Solutions.Day05;

public class Day05Tests
{
    private readonly Day05Solver _solver = new();
    [Test] public async Task Step1WithExample() => await _solver.ExecuteExample1("143");
    [Test] public async Task Step2WithExample() => await _solver.ExecuteExample2("123");
    [Test] public void Step1WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle1());
    [Test] public void Step2WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle2());
}

public class Day05Solver : SolverBase
{
    List<int[]> pageOrderingRules = new();
    List<int[]> pageUpdates = new();
    Dictionary<int, List<int>> unacceptableNumbers = new();

    protected override void Parse(List<string> data)
    {
        foreach (string line in data)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            if (line.Contains('|'))
            {
                int[] orderingRule = line.Split('|').Select(int.Parse).ToArray();
                pageOrderingRules.Add(orderingRule);
            }
            else
            {
                int[] pagesToUpdate = line.Split(',').Select(int.Parse).ToArray();
                pageUpdates.Add(pagesToUpdate);
            }
        }

        unacceptableNumbers = loadUnacceptableOrderingReference();
    }

    private Dictionary<int, List<int>> loadUnacceptableOrderingReference()
    {
        Dictionary<int, List<int>> unacceptableNumbers = new Dictionary<int, List<int>>();

        foreach (int[] ruleSet in pageOrderingRules)
        {
            if (unacceptableNumbers.ContainsKey(ruleSet[0]))
            {
                unacceptableNumbers[ruleSet[0]].Add(ruleSet[1]);
            }
            else
            {
                unacceptableNumbers[ruleSet[0]] = [ruleSet[1]];
            }
        }
        return unacceptableNumbers;
    }

    /// <summary>
    /// Checks the ordering of each set of proposed page updates in the elf sleigh launch safety manuel, adding correctly ordered page
    /// sets to a list. The page number at the mid-point index of each set is added to a running total.
    /// </summary>
    /// <returns></returns>
    protected override object Solve1()
    {
        List<int[]> correctlyOrderedPageSets = new List<int[]>();

        foreach (int[] pageSet in pageUpdates)
        {
            if (isCorrectlyOrdered(pageSet))
            {
                correctlyOrderedPageSets.Add(pageSet);
            }
        }

        int middleIndexSum = addMiddleIndices(correctlyOrderedPageSets);
        
        return middleIndexSum;
    }

    /// <summary>
    /// Checks the ordering of each set of proposed page updates in the elf sleigh launch safety manuel, correcting the order only of 
    /// those not already in the correct order. These newly corrected page sets are then added to a list, where the page number at the 
    /// mid-point index of each set is added to a running total.
    /// </summary>
    /// <returns></returns>
    protected override object Solve2()
    {
        List<int[]> correctedPageSets = new List<int[]>();

        foreach (int[] pageSet in pageUpdates)
        {
            if (!isCorrectlyOrdered(pageSet))
            {
                correctSetOrdering(pageSet);
                correctedPageSets.Add(pageSet);
            }
        }
        
        int middleIndexSum = addMiddleIndices(correctedPageSets);
        
        return middleIndexSum;
    }

    private bool isCorrectlyOrdered(int[] _pageSet)
    {
        for (int currentPagePosition = _pageSet.Length - 1; currentPagePosition > 0; currentPagePosition--)
        {                
            for (int precedingPagePosition = currentPagePosition - 1; precedingPagePosition >= 0; precedingPagePosition--)
            {
                int currentPageNumber = _pageSet[currentPagePosition];
                int precedingPageNumber = _pageSet[precedingPagePosition];

                bool precedingPageNotInOrder = unacceptableNumbers.ContainsKey(currentPageNumber) && 
                                               unacceptableNumbers[currentPageNumber].Contains(precedingPageNumber);

                if (precedingPageNotInOrder)
                {                
                    return false;
                }
            }
        }
        return true;
    }

    private int addMiddleIndices(List<int[]> _pageUpdates)
    {
        int middleIndexSum = 0; 

        foreach (int[] pageSet in _pageUpdates)
        {
            int middlePagePosition = pageSet.Length / 2;

            middleIndexSum += pageSet[middlePagePosition];
        }

        return middleIndexSum;
    }
    
    private void correctSetOrdering(int[] _pageSet)
    {
        for (int currentPagePosition = _pageSet.Length - 1; currentPagePosition > 0; currentPagePosition--)
        {                
            for (int precedingPagePosition = currentPagePosition - 1; precedingPagePosition >= 0; precedingPagePosition--)
            {
                int currentPageNumber = _pageSet[currentPagePosition];
                int precedingPageNumber = _pageSet[precedingPagePosition];

                bool precedingPageNotInOrder = unacceptableNumbers.ContainsKey(currentPageNumber) && 
                                               unacceptableNumbers[currentPageNumber].Contains(precedingPageNumber);

                if (precedingPageNotInOrder)
                {
                    int holdPageNumber = currentPageNumber;
                    _pageSet[currentPagePosition] = precedingPageNumber;
                    _pageSet[precedingPagePosition] = holdPageNumber;
                }
            }
        }
    }
}