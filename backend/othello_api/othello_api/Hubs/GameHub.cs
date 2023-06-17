using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using othello_api.Models;
using othello_api.Services;
using static GameTimerManager;

public class GameHub : Hub
{

    /* // dont exactly know what of this i need */
    /* public static readonly Dictionary<string, SessionConnection> sessionPlayerConnections = new(); */
    /* // caching relationship gameid sessionid for quick lookups */
    /* // this is probably not needed, just send session from frontend, oh well todo */
    /* public static readonly Dictionary<int, string> gameSessionMap = new(); */
    /**/
    /* // map of pending player actions in game */
    /* public static readonly Dictionary<string, List<PlayerRequest>> sessionPlayerRequests = new(); */

    public static IHubContext<GameHub> _hubContext;
    private readonly GameDataService _gameDataService;
    private readonly OthelloService _othelloService;
    private readonly HttpClient _httpClient;


    // TODO make sure that a single user cannot create an infinite amount of games...
    // LOCK down azure 

    public GameHub(HttpClient httpClient, IHubContext<GameHub> context, OthelloService service, GameDataService gameDataService)
    {
        _hubContext = context;
        _gameDataService = gameDataService;
        _httpClient = httpClient;
        _othelloService = service;
    }

    // FUNCTIONS CALLED FROM THE CLIENT

    public async Task StartSession(Color playerColor, int timeIncrement, int timeLimitSeconds)
    {
        var sessionId = Guid.NewGuid().ToString("N");

        var sessionConnection = new SessionConnection
        {
            PlayerConnections = new List<PlayerConnection>(),
            TimeLimitSeconds = timeLimitSeconds,
            IncrementTime = timeIncrement,
            WhiteWins = 0,
            BlackWins = 0,
            CurrentGameId = null
        };
        _gameDataService.sessionPlayerConnections.Add(sessionId, sessionConnection);

        // return gameInfo 
        var messages = await this._othelloService.GetMessagesAsync(sessionId);

        var newPlayerConnection = new PlayerConnection() { PlayerColor = playerColor, ConnectionId = Context.ConnectionId };
        sessionConnection.PlayerConnections.Add(newPlayerConnection);
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

        var playerUrlConnection = sessionId + (int)playerColor;
        var opponentUrlConnection = sessionId + (int)Utils.GetOppositeColor(playerColor);

        await Clients.Group(sessionId).SendAsync("sessionJoined",
            sessionId,
            playerColor,
            playerUrlConnection,
            opponentUrlConnection,
            new List<ChatMessage>(),
            sessionConnection.BlackWins,
            sessionConnection.WhiteWins,
            sessionConnection.IncrementTime,
            sessionConnection.TimeLimitSeconds
        );
    }

    public async Task<bool> JoinSession(string connectionString)
    {
        // parse url, first part is sessionId, last number is playercolor
        var sessionId = connectionString.Substring(0, connectionString.Length - 1);
        int lastNumber = Int32.Parse(connectionString[^1].ToString());

        Color playerColor;

        if (lastNumber == (int)Color.WHITE)
        {
            playerColor = Color.WHITE;
        }
        else if (lastNumber == (int)Color.BLACK)
        {
            playerColor = Color.BLACK;
        }
        else
        {
            // wrong color in url, return unsuccesful
            return false;
        }

        SessionConnection sessionConnection;

        if (_gameDataService.sessionPlayerConnections.TryGetValue(sessionId, out sessionConnection))
        {

            if (sessionConnection.PlayerConnections.Any(connection => connection.PlayerColor == playerColor))
            {
                // a player with that color already exists, return unsuccesful
                return false;
            }


            // return gameInfo 
            var messages = await this._othelloService.GetMessagesAsync(sessionId);

            var newPlayerConnection = new PlayerConnection() { PlayerColor = playerColor, ConnectionId = Context.ConnectionId };
            sessionConnection.PlayerConnections.Add(newPlayerConnection);
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            var playerUrlConnection = sessionId + (int)playerColor;
            var opponentUrlConnection = sessionId + (int)Utils.GetOppositeColor(playerColor);

            await Clients.Client(Context.ConnectionId).SendAsync("sessionJoined",
                sessionId,
                playerColor,
                playerUrlConnection,
                opponentUrlConnection,
                messages,
                sessionConnection.BlackWins,
                sessionConnection.WhiteWins,
                sessionConnection.IncrementTime,
                sessionConnection.TimeLimitSeconds
            );

            if (!sessionConnection.CurrentGameId.HasValue)
            {
                var newGame = await NewGame(sessionId);
                sessionConnection.CurrentGameId = newGame.Id;
                _gameDataService.sessionPlayerConnections[sessionId] = sessionConnection;
            }

            if (sessionConnection.CurrentGameId.HasValue)
            {
                var currentGame = await _othelloService.GetGameByIdAsync(sessionConnection.CurrentGameId.Value);
                await Clients.Group(sessionId).SendAsync("gameInfo", currentGame);
                await GameStateUpdated(sessionConnection.CurrentGameId.Value, sessionId);
            }

            return true;
        }
        else
        {
            // connectingId does not exist in dictionary
            return false;
        }

    }


    public async Task MakeMove(string sessionId, int gameId, Color playerColor, int targetSquare)
    {
        await _othelloService.MakeMoveAsync(gameId, targetSquare, playerColor);
        if (TimerHasStarted(gameId))
        {
            GameTimerManager.IncrementTime(gameId, playerColor);
        }
        await GameStateUpdated(gameId, sessionId);
    }

    public async Task Surrender(string sessionId)
    {
        var session = _gameDataService.sessionPlayerConnections[sessionId];
        var playerColor = session.PlayerConnections.FirstOrDefault(connection => connection.ConnectionId == Context.ConnectionId).PlayerColor;
        await _othelloService.EndGameAsync(session.CurrentGameId.Value, Utils.GetOppositeColor(playerColor), GameStatus.SURRENDERED);
    }

    // TODO check correct user and game
    public async Task IncrementOpponentTime(string sessionId, int gameId, Color playerColor)
    {
        await SendMessage(sessionId, Color.NONE, $"{Utils.FormatPlayerColor(Utils.GetOppositeColor(playerColor))} + 15 seconds");
        IncrementTime(gameId, Utils.GetOppositeColor(playerColor), useCustomIncrementAmount: true);
        await GameStateUpdated(gameId, sessionId);
    }

    // TODO make sure the right person is answering...
    public async Task PlayerActionMade(string sessionId, Color playerColor, PlayerAction action, ActionType actionType)
    {
        if (!_gameDataService.sessionPlayerConnections.ContainsKey(sessionId)) return;
        if (!_gameDataService.sessionPlayerRequests.ContainsKey(sessionId))
        {
            _gameDataService.sessionPlayerRequests.Add(sessionId, new List<PlayerRequest>());
        }

        var opponentColor = Utils.GetOppositeColor(playerColor);

        var formattedAction = action.ToString().ToLower();
        formattedAction = Char.ToUpper(formattedAction[0]) + formattedAction.Substring(1);

        switch (actionType)
        {
            case ActionType.REQUEST:
                // dont need to add the action type since the list only contains requests
                if (!_gameDataService.sessionPlayerRequests[sessionId].Any(request => (request.PlayerColor == playerColor && request.Action == action)))
                    _gameDataService.sessionPlayerRequests[sessionId].Add(
                        new PlayerRequest() { PlayerColor = playerColor, Action = action }
                    );
                // if both player made the same request, simply allow it
                else if (_gameDataService.sessionPlayerRequests[sessionId].Any(request =>
                             (request.PlayerColor == Utils.GetOppositeColor(playerColor) && request.Action == action)))
                {
                    await AcceptRequest(sessionId, action, playerColor);
                }

                await SendMessage(sessionId, Color.NONE, $"{formattedAction} requested");

                break;
            case ActionType.CANCEL:
                // dont need any more checks. Its okay if either player remove the request
                await SendMessage(sessionId, Color.NONE, $"{formattedAction} cancelled");

                _gameDataService.sessionPlayerRequests[sessionId].RemoveAll(request => request.PlayerColor == playerColor && request.Action == action);
                break;
            case ActionType.REJECT:
                await SendMessage(sessionId, Color.NONE, $"{formattedAction} rejected");

                _gameDataService.sessionPlayerRequests[sessionId].RemoveAll(request => request.PlayerColor != playerColor && request.Action == action);
                break;
            case ActionType.ACCEPT:
                await SendMessage(sessionId, Color.NONE, $"{formattedAction} accepted");
                await AcceptRequest(sessionId, action, playerColor);
                break;
        }

        await Clients.Group(sessionId).SendAsync("PlayerAction", _gameDataService.sessionPlayerRequests[sessionId]);
    }

    public async Task AcceptRequest(string sessionId, PlayerAction action, Color playerColor)
    {
        var gameId = _gameDataService.sessionPlayerConnections[sessionId].CurrentGameId;
        if (!gameId.HasValue && action != PlayerAction.REMATCH) throw new Exception("game id not found");

        if (action == PlayerAction.DRAW && _gameDataService.sessionPlayerRequests[sessionId].Any(request => request.Action == PlayerAction.DRAW && request.PlayerColor != playerColor))
        {
            await _othelloService.EndGameAsync(gameId.Value, Color.NONE, GameStatus.DRAW);
        }
        if (action == PlayerAction.TAKEBACK && _gameDataService.sessionPlayerRequests[sessionId].Any(request => request.Action == PlayerAction.TAKEBACK && request.PlayerColor != playerColor))
        {
            await _othelloService.Takeback(gameId.Value);
            await GameStateUpdated(gameId.Value, sessionId);
        }
        if (action == PlayerAction.REMATCH && _gameDataService.sessionPlayerRequests[sessionId].Any(request => request.Action == PlayerAction.REMATCH && request.PlayerColor != playerColor) && _gameDataService.sessionPlayerConnections[sessionId].CurrentGameId == null)
        {
            var newGame = await NewGame(sessionId);

            var session = _gameDataService.sessionPlayerConnections[sessionId];
            session.CurrentGameId = newGame.Id;
            _gameDataService.sessionPlayerConnections[sessionId] = session;

            await Clients.Group(sessionId).SendAsync("gameInfo", newGame);
            await GameStateUpdated(newGame.Id, sessionId);
        }

        _gameDataService.sessionPlayerRequests[sessionId].RemoveAll(request => request.Action == action);
    }

    public async Task SendMessage(string sessionId, Color senderColor, string content)
    {
        var savedMessage = this._othelloService.SaveChatMessage(sessionId, senderColor, content);
        await Clients.Group(sessionId).SendAsync("receiveMessage", senderColor, content, savedMessage.Timestamp);
    }

    public async Task Ping() { }


    // Internal functions 

    // game state updated, notify clients
    private async Task GameStateUpdated(int gameId, string sessionId)
    {
        var game = await _othelloService.GetGameByIdAsync(gameId);

        // get legal moves and skip the turn if none are found
        var nextPlayerPossibleMoves = GameStateUtil.GetLegalMoves(game.State, game.PlayerTurn);
        if (nextPlayerPossibleMoves.Count == 0)
        {
            await _othelloService.SetPlayersTurnAsync(gameId, Utils.GetOppositeColor(game.PlayerTurn));
            game.PlayerTurn = Utils.GetOppositeColor(game.PlayerTurn);
            nextPlayerPossibleMoves = GameStateUtil.GetLegalMoves(game.State, Utils.GetOppositeColor(game.PlayerTurn));
        }


        if (game.Moves.Count >= 2 && !TimerHasStarted(gameId))
        {
            SetTimerStarted(gameId);

            await SendMessage(sessionId, Color.NONE, "Timers started");
        }
        if (TimerHasStarted(gameId))
        {
            ChangeTimerState(gameId, game.PlayerTurn, TimerAction.Resume);
            ChangeTimerState(gameId, Utils.GetOppositeColor(game.PlayerTurn), TimerAction.Pause);
        }

        await Clients
            .Group(sessionId)
            .SendAsync(
                "gameStateUpdated",
                game.State,
                game.PlayerTurn,
                nextPlayerPossibleMoves,
                GetTimeInSeconds(gameId, Color.BLACK),
                GetTimeInSeconds(gameId, Color.WHITE),
                DateTime.Now,
                game.Moves.Any() ? game.Moves.Last() : null
            );


        // if neither player can make a move, end the game
        if (nextPlayerPossibleMoves.Count == 0)
        {
            var blackMarks = GameStateUtil.GetBlackMarksCount(game.State);
            var whiteMarks = GameStateUtil.GetWhiteMarksCount(game.State);
            var winner = Color.NONE;
            if (blackMarks > whiteMarks)
                winner = Color.BLACK;
            if (blackMarks < whiteMarks)
                winner = Color.WHITE;
            // notify clients that game ended
            _othelloService.EndGameAsync(gameId, winner, GameStatus.WON_BY_MARKS);
            return;
        }

    }



    /* private async Task EndGameAsync(int gameId, Color winner, GameStatus status) */
    /* { */
    /*     Console.WriteLine("GAME ENDED"); */
    /*     Console.WriteLine(gameId); */
    /*     Console.WriteLine(winner); */
    /*     Console.WriteLine(status); */
    /*     await this._gameDataService.EndGameAsync(gameId, winner, status); */
    /**/
    /*     // dispose of old objects */
    /*     var sessionId = _gameDataService.gameSessionMap[gameId]; */
    /*     var session = sessionPlayerConnections[sessionId]; */
    /*     session.CurrentGameId = null; */
    /*     var gameTimers = GameTimersDictionary[gameId]; */
    /*     gameTimers.BlackTime.Dispose(); */
    /*     gameTimers.WhiteTime.Dispose(); */
    /*     GameTimersDictionary.Remove(gameId); */
    /*     sessionPlayerRequests[sessionId] = new List<PlayerRequest>(); */
    /**/
    /**/
    /*     if (winner == Color.WHITE) session.WhiteWins++; */
    /*     else if (winner == Color.BLACK) session.BlackWins++; */
    /*     sessionPlayerConnections[sessionId] = session; */
    /**/
    /*     //EndGameAsync(gameId, winner, GameStatus.WON_TIME); */
    /*     if (status == GameStatus.SURRENDERED) */
    /*     { */
    /*         await SendMessage(sessionId, Color.NONE, $"{Utils.FormatPlayerColor(Utils.GetOppositeColor(winner))} surrendered, {Utils.FormatPlayerColor(winner)} won "); */
    /*     } */
    /*     else if (status == GameStatus.WON_TIME) */
    /*     { */
    /*         await SendMessage(sessionId, Color.NONE, $"{Utils.FormatPlayerColor(winner)} won on time"); */
    /*     } */
    /*     else if (status == GameStatus.WON_BY_MARKS) */
    /*     { */
    /*         await SendMessage(sessionId, Color.NONE, $"{Utils.FormatPlayerColor(winner)} won by marks"); */
    /*     } */
    /**/
    /*     await Clients.Group(sessionId).SendAsync("gameEnded", winner, status, session.BlackWins, session.WhiteWins); */
    /* } */

    private async Task<Game> NewGame(string sessionId)
    {
        var newGame = _othelloService.NewGame(_gameDataService.sessionPlayerConnections[sessionId].TimeLimitSeconds, _gameDataService.sessionPlayerConnections[sessionId].IncrementTime, sessionId);

        await SendMessage(sessionId, Color.NONE, "New game started");
        return newGame;
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        foreach (var session in _gameDataService.sessionPlayerConnections)
        {
            var player = session.Value.PlayerConnections.FirstOrDefault(p => p.ConnectionId == connectionId);

            if (!player.Equals(default(PlayerConnection)))
            {
                session.Value.PlayerConnections.Remove(player);

                await _hubContext.Clients.Group(session.Key).SendAsync("PlayerDisconnected", player.PlayerColor);
                await Groups.RemoveFromGroupAsync(connectionId, session.Key);





                // this is a bit of a hack but works well enough. If both users have been disconnected for 10 seconds, remove the game
                // it has some bugs though
                if (session.Value.PlayerConnections.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    if (session.Value.PlayerConnections.Count == 0)
                    {
                        _gameDataService.sessionPlayerConnections.Remove(session.Key);
                        _gameDataService.gameSessionMap.Remove(session.Value.CurrentGameId.Value);
                    }
                }

                break;
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
    public struct GameTimer
    {
        public int BlackTime;
        public int WhiteTime;
    }
}



