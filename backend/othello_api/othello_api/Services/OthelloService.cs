using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using othello_api.Contexts;
using othello_api.Models;

namespace othello_api.Services;

/* public interface IOthelloService */
/* { */
/*     List<Game> GetAllGames(); */
/*     Task<Game> GetGameByIdAsync(int gameId); */
/*     ChatMessage SaveChatMessage(string sessionId, Color senderColor, string content); */
/*     Task<Game> MakeMoveAsync(int gameId, int targetSquare, Color playerColor); */
/*     Task<Game> Takeback(int gameId); */
/*     Game NewGame(int timeLimitSeconds, int timeIncrementSeconds, string sessionId); */
/*     Task<Game> EndGameAsync(int gameId, Color winner, GameStatus status); */
/*     Task<Game> SetPlayersTurnAsync(int gameId, Color playerTurn); */
/*     Task<Game> ChangeGameStatusAsync(int gameId, GameStatus status); */
/*     void ValidateGameExists(Game game, int gameId); */
/*     void ValidateGameAndPlayerTurn(Game game, int gameId, Color playerColor); */
/*     Task<List<ChatMessage>> GetMessagesAsync(string sessionId); */
/* } */
/**/

/* : IOthelloService */
public class OthelloService
{
    private readonly OthelloContext _context;
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly GameDataService _gameDataService;
    private readonly IServiceScopeFactory _scopeFactory;

    public OthelloService(OthelloContext othelloContext, IHubContext<GameHub> gameHubContext, GameDataService gameDataService, IServiceScopeFactory scopeFactory)
    {
        this._context = othelloContext;
        this._gameHubContext = gameHubContext;
        this._gameDataService = gameDataService;
        this._scopeFactory = scopeFactory;
    }

    public List<Game> GetAllGames()
    {
        return _context.Games.ToList();
    }

    public async Task<Game> GetGameByIdAsync(int gameId)
    {
        return await _context.Games.FindAsync(gameId);
    }

    public ChatMessage SaveChatMessage(string sessionId, Color senderColor, string content)
    {
        var chatMessage = new ChatMessage
        {
            SesssionId = sessionId,
            SenderColor = senderColor,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        _context.SaveChanges();

        return chatMessage;
    }

    public async Task<Game> MakeMoveAsync(int gameId, int targetSquare, Color playerColor)
    {
        var game = await GetGameByIdAsync(gameId);
        ValidateGameAndPlayerTurn(game, gameId, playerColor);

        var gameState = GameStateUtil.PlaceMark(game.State, targetSquare, playerColor);

        game.Moves.Add(new GameMove()
        {
            GameId = game.Id,
            PlayerColor = playerColor,
            Square = targetSquare,
            MoveNumber = game.Moves.Count + 1,
            Timestamp = DateTime.UtcNow,
            ResultingState = gameState
        });

        game.State = gameState;
        game.PlayerTurn = Utils.GetOppositeColor(playerColor);

        await _context.SaveChangesAsync();

        return game;
    }

    public async Task<Game> Takeback(int gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game.Moves.Count >= 1)
        {
            game.PlayerTurn = game.Moves.Last().PlayerColor;
            game.Moves.Remove(game.Moves.Last());
            game.State = game.Moves.Count > 0 ? game.Moves.Last().ResultingState : ",,,,,,,,,,,,,,,,,,,,,,,,,,,1,0,,,,,,,0,1,,,,,,,,,,,,,,,,,,,,,,,,,,,";
            await _context.SaveChangesAsync();
        }
        return game;
    }


    /* public Game MakeMove(int gameId, int targetSquare, Color playerColor) */
    /* { */
    /*     var game = _context.Games.FirstOrDefault(game => game.Id == gameId); */
    /*     if (game == null) */
    /*         throw new Exception("game does not exist"); */
    /*     if (game.PlayerTurn != playerColor) */
    /*         throw new Exception("Not players turn"); */
    /**/
    /*     game.State.State = game.State.PlaceMark(targetSquare, playerColor); */
    /**/
    /*     _context.SaveChanges(); */
    /**/
    /*     //StateUpdated(gameId, game.State.State); */
    /**/
    /*     return game; */
    /* } */
    /**/

    public Game NewGame(int timeLimitSeconds, int timeIncrementSeconds, string sessionId)
    {
        //, Color playerColor
        var newGame = new Game(timeLimitSeconds, timeIncrementSeconds, sessionId);
        _context.Games.Add(newGame);
        _context.SaveChanges();

        _gameDataService.gameSessionMap.Add(newGame.Id, sessionId);
        GameTimerManager.CreateGameTimers(
            newGame.Id,
            newGame.TimeIncrementSeconds,
            newGame.TimeLimitSeconds,
            newGame.GameStarttime,
            async (winner) => await EndGameAsync(newGame.Id, winner, GameStatus.WON_TIME)
        );

        return newGame;
    }

    /* public async Task<Game> EndGameAsync(int gameId, Color winner, GameStatus status) */
    /* { */
    /*     var game = await GetGameByIdAsync(gameId); */
    /**/
    /*     Console.WriteLine(game); */
    /**/
    /*     ValidateGameExists(game, gameId); */
    /**/
    /**/
    /*     /* game.Status = GameStatus.DRAW; */
    /*     /* game.Winner = Color.NONE; */
    /*     game.Status = status; */
    /*     game.Winner = winner; */
    /**/
    /*     await _context.SaveChangesAsync(); */
    /**/
    /*     return game; */
    /* } */

    public async Task EndGameAsync(int gameId, Color winner, GameStatus status)
    {
        // need to use _scopeFactory since this function can be called from timer timeout
        using (var scope = _scopeFactory.CreateScope())
        {
            var tmpContext = scope.ServiceProvider.GetRequiredService<OthelloContext>();
            var game = await tmpContext.Games.FindAsync(gameId);

            ValidateGameExists(game, gameId);

            game.Status = status;
            game.Winner = winner;

            await tmpContext.SaveChangesAsync();

            // Dispose of old objects
            /* var sessionId = _gameDataService.gameSessionMap[gameId]; */
            /* var session = _gameDataService.sessionPlayerConnections[sessionId]; */
            /* session.CurrentGameId = null; */
            /* var gameTimers = GameTimerManager.GameTimersDictionary[gameId]; */
            /* gameTimers.BlackTime.Dispose(); */
            /* gameTimers.WhiteTime.Dispose(); */
            /* GameTimerManager.GameTimersDictionary.Remove(gameId); */
            /* _gameDataService.sessionPlayerRequests[sessionId] = new List<PlayerRequest>(); */

            // I hope this a safer version
            if (_gameDataService.gameSessionMap.TryGetValue(gameId, out var sessionId))
            {
                if (_gameDataService.sessionPlayerConnections.TryGetValue(sessionId, out var session))
                {
                    session.CurrentGameId = null;

                    if (_gameDataService.sessionPlayerRequests.ContainsKey(sessionId))
                    {
                        _gameDataService.sessionPlayerRequests[sessionId] = new List<PlayerRequest>();
                    }

                    // Update game stats
                    if (winner == Color.WHITE) session.WhiteWins++;
                    else if (winner == Color.BLACK) session.BlackWins++;
                    _gameDataService.sessionPlayerConnections[sessionId] = session;


                    // Send messages to clients
                    string message = "";
                    if (status == GameStatus.SURRENDERED)
                    {
                        message = $"{Utils.FormatPlayerColor(Utils.GetOppositeColor(winner))} surrendered, {Utils.FormatPlayerColor(winner)} won ";
                    }
                    else if (status == GameStatus.WON_TIME)
                    {
                        message = $"{Utils.FormatPlayerColor(winner)} won on time";
                    }
                    else if (status == GameStatus.WON_BY_MARKS)
                    {
                        message = $"{Utils.FormatPlayerColor(winner)} won by marks";
                    }

                    await _gameHubContext.Clients.Group(sessionId).SendAsync("receiveMessage", Color.NONE, message, DateTime.UtcNow);
                    await _gameHubContext.Clients.Group(sessionId).SendAsync("gameEnded", winner, status, session.BlackWins, session.WhiteWins);

                }

                if (GameTimerManager.GameTimersDictionary.TryGetValue(gameId, out var gameTimers))
                {
                    gameTimers.BlackTime?.Dispose();
                    gameTimers.WhiteTime?.Dispose();
                    GameTimerManager.GameTimersDictionary.Remove(gameId);
                }
            }



        }
    }

    /**/
    /* public Game EndGame(int gameId, Color winner, GameStatus status) */
    /* { */
    /*     var game = _context.Games.FirstOrDefault(game => game.Id == gameId); */
    /*     if (game == null) */
    /*         throw new Exception("game does not exist"); */
    /**/
    /*     game.Status = status; */
    /*     game.Winner = winner; */
    /**/
    /*     _context.SaveChanges(); */
    /**/
    /*     return game; */
    /* } */


    public async Task<Game> SetPlayersTurnAsync(int gameId, Color playerTurn)
    {
        var game = await GetGameByIdAsync(gameId);
        ValidateGameExists(game, gameId);

        game.PlayerTurn = playerTurn;
        await _context.SaveChangesAsync();

        return game;
    }


    /* public Game SetPlayersTurn(int gameId, Color playerTurn) */
    /* { */
    /*     var game = _context.Games.FirstOrDefault(game => game.Id == gameId); */
    /*     if (game == null) */
    /*         throw new Exception("game does not exist"); */
    /**/
    /*     game.PlayerTurn = playerTurn; */
    /*     _context.SaveChanges(); */
    /**/
    /*     return game; */
    /* } */


    public async Task<Game> ChangeGameStatusAsync(int gameId, GameStatus status)
    {
        var game = await GetGameByIdAsync(gameId);
        ValidateGameExists(game, gameId);

        game.Status = status;
        _context.SaveChanges();

        return game;
    }

    public void ValidateGameExists(Game game, int gameId)
    {
        if (game == null)
            throw new Exception($"Game with ID {gameId} does not exist");
    }

    public void ValidateGameAndPlayerTurn(Game game, int gameId, Color playerColor)
    {
        ValidateGameExists(game, gameId);

        if (game.PlayerTurn != playerColor)
            throw new Exception("Not player's turn");
    }

    public async Task<List<ChatMessage>> GetMessagesAsync(string sessionId)
    {
        return await _context.ChatMessages.Where(message => message.SesssionId == sessionId).ToListAsync();
    }

    /* internal async Task<List<GameMoves>> GetMovesAsync(int gameId) */
    /* { */
    /*     return await _context.GameMoves.Where(move => move.GameId == gameId).ToListAsync(); */
    /* } */

}
