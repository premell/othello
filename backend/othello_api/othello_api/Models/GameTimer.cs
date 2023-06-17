public static class GameTimerManager
{
    public static Dictionary<int, GameTimers> GameTimersDictionary = new Dictionary<int, GameTimers>();

    public static void CreateGameTimers(
        int gameId,
        int incrementTime,
        int timeLimit,
        DateTime startTime,
        Action<Color> timeRanOut
    )
    {
        var gameTimers = new GameTimers
        {
            // add 0.5 to avoid some annoying bugs
            BlackTime = new AdvancedTimer(TimeSpan.FromSeconds(timeLimit + 0.5), startTime),
            WhiteTime = new AdvancedTimer(TimeSpan.FromSeconds(timeLimit + 0.5), startTime),
            IncrementTime = incrementTime,
            StartTime = startTime,
            TimerStarted = false
        };

        gameTimers.WhiteTime.TimeRanOut += (sender, e) =>
        {
            timeRanOut(Color.BLACK);
        };
        gameTimers.BlackTime.TimeRanOut += (sender, e) =>
        {
            timeRanOut(Color.WHITE);
        };

        GameTimersDictionary[gameId] = gameTimers;
    }

    public static void IncrementTime(int gameId, Color playerColor, int customIncrementAmount = 15,bool useCustomIncrementAmount = false)
    {
        if (GameTimersDictionary.TryGetValue(gameId, out var gameTimers))
        {
            var timer = playerColor == Color.WHITE ? gameTimers.WhiteTime : gameTimers.BlackTime;
            timer.Increment(useCustomIncrementAmount ? customIncrementAmount : gameTimers.IncrementTime);
        }
        else
        {
            throw new Exception("Game not found");
        }
    }

    public static double GetTimeInSeconds(int gameId, Color playerColor)
    {
        if (GameTimersDictionary.TryGetValue(gameId, out var gameTimers))
        {
            var timer = playerColor == Color.WHITE ? gameTimers.WhiteTime : gameTimers.BlackTime;
            return timer.RemainingTime.TotalSeconds;
        }
        else
        {
            throw new Exception("Game not found");
        }
    }

    public static void ChangeTimerState(int gameId, Color playerColor, TimerAction action)
    {
        if (GameTimersDictionary.TryGetValue(gameId, out var gameTimers))
        {
            var timer = playerColor == Color.WHITE ? gameTimers.WhiteTime : gameTimers.BlackTime;
            switch (action)
            {
                case TimerAction.Pause:
                    timer.Pause();
                    break;
                case TimerAction.Resume:
                    timer.Resume();
                    break;
                default:
                    throw new ArgumentException("Invalid timer action");
            }
        }
        else
        {
            throw new Exception("Game not found");
        }
    }

    public static bool TimerHasStarted(int gameId)
    {
        if (GameTimersDictionary.TryGetValue(gameId, out var gameTimers))
        {
          return gameTimers.TimerStarted;
        }
        else
        {
            throw new Exception("Game not found");
        }
    }

    public static void SetTimerStarted(int gameId)
    {
        if (GameTimersDictionary.TryGetValue(gameId, out var gameTimers))
        {
          gameTimers.TimerStarted = true;
          GameTimersDictionary[gameId] = gameTimers;
        }
        else
        {
            throw new Exception("Game not found");
        }
    }


    public struct GameTimers
    {
        public AdvancedTimer BlackTime;
        public AdvancedTimer WhiteTime;
        public int IncrementTime;
        public DateTime StartTime;
        public bool TimerStarted;
    }

    public enum TimerAction
    {
        Pause,
        Resume
    }
}
