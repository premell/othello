using Microsoft.AspNetCore.SignalR;
using othello_api.Models;

namespace othello_api.Services;

public class OthelloService
{
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly GameDataService _gameDataService;

    public OthelloService(IHubContext<GameHub> gameHubContext, GameDataService gameDataService)
    {
        this._gameHubContext = gameHubContext;
        this._gameDataService = gameDataService;
    }

    public Game? GetGameById(int gameId)
    {
        if (_gameDataService.games.TryGetValue(gameId, out var game))
            return game;
        else return null;
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


        if (_gameDataService.messages.TryGetValue(sessionId, out var messages))
            messages.Add(chatMessage);
        else
        {
            var newMessages = new List<ChatMessage>(){
            chatMessage
          };
            _gameDataService.messages.Add(sessionId, newMessages);
        }

        return chatMessage;
    }

    public Game MakeMove(int gameId, int targetSquare, Color playerColor)
    {
        var game = GetGameById(gameId);
        ValidateGameAndPlayerTurn(game, gameId, playerColor);

        var gameState = GameStateUtil.PlaceMark(game.State, targetSquare, playerColor);

        game.Moves.Add(new GameMove()
        {
            PlayerColor = playerColor,
            Square = targetSquare,
            MoveNumber = game.Moves.Count + 1,
            ResultingState = gameState
        });

        game.State = gameState;
        game.PlayerTurn = Utils.GetOppositeColor(playerColor);

        return game;
    }

    public Game Takeback(int gameId)
    {
        var game = GetGameById(gameId);
        if (game?.Moves.Count >= 1)
        {
            game.PlayerTurn = game.Moves.Last().PlayerColor;
            game.Moves.Remove(game.Moves.Last());
            game.State = game.Moves.Count > 0 ? game.Moves.Last().ResultingState : ",,,,,,,,,,,,,,,,,,,,,,,,,,,1,0,,,,,,,0,1,,,,,,,,,,,,,,,,,,,,,,,,,,,";
        }
        return game;
    }

    public Game NewGame(int timeLimitSeconds, int timeIncrementSeconds, string sessionId)
    {
        //, Color playerColor
        var newGame = new Game(timeLimitSeconds, timeIncrementSeconds, sessionId);
        _gameDataService.games.Add(newGame.Id, newGame);

        _gameDataService.gameSessionMap.Add(newGame.Id, sessionId);
        GameTimerManager.CreateGameTimers(
            newGame.Id,
            newGame.TimeIncrementSeconds,
            newGame.TimeLimitSeconds,
            newGame.GameStarttime,
            async (winner) => await EndGame(newGame.Id, winner, GameStatus.WON_TIME)
        );

        return newGame;
    }

    public async Task EndGame(int gameId, Color winner, GameStatus status)
    {
        var game = GetGameById(gameId);

        ValidateGameExists(game, gameId);

        game.Status = status;
        game.Winner = winner;

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

            if (_gameDataService.games.ContainsKey(gameId))
            {
                _gameDataService.games.Remove(gameId);
            }

            if (_gameDataService.gameSessionMap.ContainsKey(gameId))
            {
                _gameDataService.gameSessionMap.Remove(gameId);
            }

        }
    }

    public Game SetPlayersTurn(int gameId, Color playerTurn)
    {
        var game = GetGameById(gameId);
        ValidateGameExists(game, gameId);
        game.PlayerTurn = playerTurn;

        return game;
    }


    public Game ChangeGameStatusAsync(int gameId, GameStatus status)
    {
        var game = GetGameById(gameId);
        ValidateGameExists(game, gameId);
        game.Status = status;

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

    public List<ChatMessage>? GetMessages(string sessionId)
    {
        if (_gameDataService.messages.TryGetValue(sessionId, out var messages))
            return messages;
        else return new List<ChatMessage>();
    }
}
