namespace AdventOfCode.Solutions.Day06;

public class Day06Tests
{
    private readonly Day06Solver _solver = new();
    [Test] public async Task Step1WithExample() => await _solver.ExecuteExample1("41");
    [Test] public async Task Step2WithExample() => await _solver.ExecuteExample2("6");
    [Test] public void Step1WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle1());
    [Test] public void Step2WithPuzzleInput() => TestContext.Current?.OutputWriter.WriteLine(_solver.ExecutePuzzle2());
}

public class Day06Solver : SolverBase
{
    List<char[]> gameBoard = new();

    protected override void Parse(List<string> data)
    {
        gameBoard = data.Select(line => line.ToCharArray()).ToList();
    }

    /// <summary>
    /// Moves the agent through the game board utilizing the given movement protocol, i.e. walk straight unless blocked by an obstacle,
    /// at which point, turn to the right. The test counts all distinct positions on the game board the agent occupies before walking 
    /// off the board. 
    /// </summary>
    /// <returns></returns>
    protected override object Solve1()
    {
        Agent agent = new Agent(gameBoard);

        while (agent.IsOnBoard())
        {
            agent.Move();
        }

        int distinctPositions = agent.CountDistinctPositions();

        return distinctPositions;
    }

    /// <summary>
    /// Counts how many positions (empty tiles) on the game board can be substituted for an obstacle which, in doing so, cause the 
    /// agent to be trapped in a loop.  
    /// </summary>
    /// <returns></returns>
    protected override object Solve2()
    {
        int boardHeight = gameBoard.Count;
        int boardWidth = gameBoard[0].Length;
        int loopCount = 0; 

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                char currentTile = gameBoard[y][x]; 

                if (!IsEmptyTile(currentTile))
                {
                    continue;
                }

                SetUpBarricadeAtCoordinates(y, x);

                bool loopExists = TestForLoop(gameBoard);

                if (loopExists)
                {
                    loopCount++;
                }
                
                RemoveBarricadeAtCoordinates(y, x);
            }
        }
        
        return loopCount;
    }

    private bool IsEmptyTile(char _tile)
    {
        const char emptyTile = '.';
        return _tile == emptyTile;
    }

    private void SetUpBarricadeAtCoordinates(int _y, int _x)
    {
        const char barricade = '#';
        gameBoard[_y][_x] = barricade;
    }

    private void RemoveBarricadeAtCoordinates(int _y, int _x)
    {
        const char emptyTile = '.';
        gameBoard[_y][_x] = emptyTile;
    }

    private bool TestForLoop(List<char[]> _gameBoard)
    {
        Agent agent = new Agent(_gameBoard);

        while (agent.IsOnBoard())
        {
            agent.Move(); 

            if (agent.IsStuckInLoop())
            {
                return true; 
            }

            agent.LogState();
        }
        return false;
    }
}

public class Agent
{
    private List<char[]> gameBoard;
    private int xCoordinate;
    private int yCoordinate;
    private char currentState;
    private char[] agentStates = ['^', '>', 'v', '<'];
    private (int yMovement, int xMovement)[] agentMovement = [ (-1, 0), (0, 1), (1, 0), (0, -1) ];
    private HashSet<StateLogger> movementLog = new HashSet<StateLogger>();

    public Agent(List<char[]> _gameBoard)
    {
        gameBoard = _gameBoard.Select(row => row.ToArray()).ToList();
        InitializeStartingPosition(gameBoard);
    }

    private void InitializeStartingPosition(List<char[]> _gameBoard)
    {
        int boardHeight = _gameBoard.Count;
        int boardWidth = _gameBoard[0].Length;
        
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                char cell = _gameBoard[y][x];

                if (!IsAgent(cell))
                    continue;
                
                xCoordinate = x;
                yCoordinate = y;
                currentState = cell;

                return;
            }
        }
        
        throw new Exception("No agent could be found on gameboard.");
    }

    private bool IsAgent(char _cell)
    {
        return agentStates.Contains(_cell);
    }

    public bool IsOnBoard()
    {
        int boardHeight = gameBoard.Count;
        int boardWidth = gameBoard[0].Length;
        
        if (xCoordinate < 0 ||
            yCoordinate < 0 ||
            xCoordinate >= boardWidth ||
            yCoordinate >= boardHeight)
        {
            return false;
        }

        return true;
    }

    private bool CoordinatesAreOnBoard(int _y, int _x)
    {
        int boardHeight = gameBoard.Count;
        int boardWidth = gameBoard[0].Length;
        
        if (_x < 0 ||
            _y < 0 ||
            _x >= boardWidth ||
            _y >= boardHeight)
        {
            return false;
        }

        return true;
    }
    
    public void Move()
    {
        const char barricade = '#'; 

        int currentDirection = Array.IndexOf(agentStates, currentState);
        (int yMovement, int xMovement) = agentMovement[currentDirection];

        int nextYCoordinate = yCoordinate + yMovement;
        int nextXCoordinate = xCoordinate + xMovement;

        if (CoordinatesAreOnBoard(nextYCoordinate, nextXCoordinate))
        {
            char nextTile = gameBoard[nextYCoordinate][nextXCoordinate];

            if (nextTile == barricade)
            {
                Turn();
                return;
            }

            AdvanceToNextTile(nextYCoordinate, nextXCoordinate);
        }

        MarkPrecedingTile();

        UpdateAgentCoordinates(nextYCoordinate, nextXCoordinate);
    }

    private void Turn()
    {
        int currentStateIndex = Array.IndexOf(agentStates, currentState);
        
        if (currentStateIndex == 3)
        {
            currentState = agentStates[0];
        } 
        else
        {
            currentState = agentStates[currentStateIndex + 1];
        }
    }

    private void AdvanceToNextTile(int _nextY, int _nextX)
    {
        gameBoard[_nextY][_nextX] = currentState;
    }

    private void MarkPrecedingTile()
    {
        gameBoard[yCoordinate][xCoordinate] = 'X';
    }

    private void UpdateAgentCoordinates(int _y, int _x)
    {
        yCoordinate = _y;
        xCoordinate = _x;
    }

    public int CountDistinctPositions()
    {
        int positionCount = 0;

        foreach (char[] row in gameBoard)
        {
            foreach (char tile in row)
            {
                if (tile == 'X')
                {
                    positionCount += 1;
                }
            }
        }
        
        return positionCount;
    }
    
    public bool IsStuckInLoop()
    {
        StateLogger state = new StateLogger(currentState, yCoordinate, xCoordinate);

        if (movementLog.Contains(state))
        {
            return true;
        }
        
        return false;
    }

    public void LogState()
    {
        StateLogger state = new StateLogger(currentState, yCoordinate, xCoordinate);

        if (!movementLog.Contains(state))
        {
            movementLog.Add(state);
        }
    }
}

public struct StateLogger
{
    public int yCoordinate { get; }
    public int xCoordinate { get; }
    public char agentState { get; }

    public StateLogger(char _agentState, int _yCoordinate, int _xCoordinate)
    {
        yCoordinate = _yCoordinate;
        xCoordinate = _xCoordinate;
        agentState = _agentState;
    }
}