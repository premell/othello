using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// using System.Linq.Dynamic;

namespace othello_api.Models;

public class GameMove
{
    public Color PlayerColor { get; set; }
    public int Square { get; set; }
    public int MoveNumber { get; set; }
    public string ResultingState { get; set; }
}


public class Game
{
    public int Id { get; set; }
    public string State { get; set; }
    public List<GameMove> Moves { get; set; } = new List<GameMove>();
    public int TimeLimitSeconds { get; set; }
    public int TimeIncrementSeconds { get; set; }
    public DateTime GameStarttime { get; set; } = DateTime.Now;
    public GameStatus? Status { get; set; } = GameStatus.PLAYING;
    public Color? Winner { get; set; } = Color.NONE;
    public Color PlayerTurn { get; set; }
    public string SessionId { get; set; }

    public Game(int timeLimitSeconds, int timeIncrementSeconds, string sessionId)
    {
        TimeLimitSeconds = timeLimitSeconds;
        TimeIncrementSeconds = timeIncrementSeconds;
        SessionId = sessionId;
        State = ",,,,,,,,,,,,,,,,,,,,,,,,,,,1,0,,,,,,,0,1,,,,,,,,,,,,,,,,,,,,,,,,,,,";

        Random rnd = new Random();
        Id = rnd.Next(1, 1_000_000_000);
    }
}

