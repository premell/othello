using othello_api.Models;


// Instead of a database...
public class GameDataService
{
    // dont exactly know what of this i need
    public Dictionary<string, SessionConnection> sessionPlayerConnections { get; } = new();
    // caching relationship gameid sessionid for quick lookups
    // this is probably not needed, just send session from frontend, oh well todo
    public Dictionary<int, string> gameSessionMap { get; } = new();

    // map of pending player actions in game
    public Dictionary<string, List<PlayerRequest>> sessionPlayerRequests { get; } = new();

    public Dictionary<int, Game> games { get; } = new();

    public Dictionary<string, List<ChatMessage>> messages { get; } = new();


    // array of stringIds of the sesions that should be removed after 10 seconds
    public List<string> cleanupConnections { get; } = new();
}


public struct SessionConnection
{
    public List<PlayerConnection> PlayerConnections;
    public int IncrementTime;
    public int TimeLimitSeconds;
    public int WhiteWins;
    public int BlackWins;
    public int? CurrentGameId;
}

public struct PlayerConnection
{
    public Color PlayerColor;
    public string ConnectionId;
}


public struct PlayerRequest
{
    public Color PlayerColor { get; set; }
    public PlayerAction Action { get; set; }
}
