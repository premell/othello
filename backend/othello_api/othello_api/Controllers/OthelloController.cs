using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using othello_api.Models;
using othello_api.Services;

namespace othello_api.Controllers;

[ApiController]
[Route("[controller]")]
public class OthelloController : ControllerBase
{
    private readonly ILogger<OthelloController> _logger;
    private readonly OthelloService _othelloService;
    private readonly IHubContext<GameHub> _hubContext;

    public OthelloController(
        ILogger<OthelloController> logger,
        OthelloService othelloService,
        IHubContext<GameHub> hubContext
    )
    {
        //var connString = builder.Configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _othelloService = othelloService;
        _hubContext = hubContext;
    }

   //[HttpGet("games")]
    //public ActionResult<List<Game>> GetAllGames() => Ok(this._othelloService.GetAllGames());

    [HttpGet]
public ActionResult<List<Game>> GetAllGames1()
{
    try
    {
        return Ok(this._othelloService.GetAllGames());
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}

    [HttpGet("test")]
    public ActionResult<string> Test() => Ok("testing api 2");
    
    /* [HttpGet("games/{id}")] */
    /* public async Task<ActionResult<Game>> GetGameById(int id) => Ok(await this._othelloService.GetGameByIdAsync(id)); */
    /**/
    /* [HttpPost("games")] */
    /* public ActionResult<Game> NewGame([FromBody] GameRequest game) */
    /* { */
    /*     var timeLimitSeconds = game.TimeLimitSeconds; */
    /*     var timeIncrementSeconds = game.TimeIncrementSeconds; */
    /**/
    /*     return Ok(this._othelloService.NewGame(timeLimitSeconds, timeIncrementSeconds)); */
    /* } */
}

/* [HttpPost("games/{id}/move")] */
/* public ActionResult<Game> MakeMove([FromBody] MoveRequest move, int gameId) */
/* { */
/*     var playerColor = move.PlayerColor; */
/*     var opponentColor = (Color)(((int)playerColor + 1) % 2); */
/*     var targetSquare = move.TargetSquare; */
/*     var newGame = this._othelloService.MakeMove(gameId, targetSquare, playerColor); */
/*     newGame.PlayerTurn = opponentColor; */
/**/
/*     var timers = GameTimerManager.GameTimersDictionary[gameId]; */
/*     if (playerColor == Color.WHITE) */
/*     { */
/*         timers.WhiteTime.Pause(); */
/*         timers.BlackTime.Resume(); */
/*     } */
/*     else */
/*     { */
/*         timers.BlackTime.Pause(); */
/*         timers.WhiteTime.Resume(); */
/*     } */
/**/
/*     var nextPlayerPossibleMoves = newGame.State.GetLegalMoves(opponentColor); */
/*     Console.WriteLine(nextPlayerPossibleMoves); */
/**/
/*     if (nextPlayerPossibleMoves.Count == 0) // next player has no moves and their turn should be skipped */
/*     { */
/*         // check if the currentplayer has any moves */
/*         var currentplayerPossibleMoves = newGame.State.GetLegalMoves(playerColor); */
/**/
/*         // neither player can make a move and the game is finished */
/*         if (currentplayerPossibleMoves.Count == 0) */
/*         { */
/*             var blackMarks = newGame.State.GetBlackMarksCount(); */
/*             var whiteMarks = newGame.State.GetWhiteMarksCount(); */
/*             var winner = Color.NONE; */
/*             if (blackMarks > whiteMarks) */
/*                 winner = Color.BLACK; */
/*             if (blackMarks < whiteMarks) */
/*                 winner = Color.WHITE; */
/*             this._othelloService.EndGame(gameId, winner, GameStatus.WON_BY_MARKS); */
/*         } */
/*         else */
/*         { */
/*             this._hubContext.Clients */
/*                 .Group(Convert.ToString(gameId)) */
/*                 .SendAsync("skipPlayerTurn", opponentColor); */
/*             newGame.PlayerTurn = playerColor; */
/*             nextPlayerPossibleMoves = currentplayerPossibleMoves; */
/*         } */
/*     } */
/*     else */
/*     { */
/*         newGame.PlayerTurn = opponentColor; */
/*         this._othelloService.SetPlayersTurn(gameId,newGame.PlayerTurn); */
/*     } */
/**/
/*     this._hubContext.Clients */
/*         .Group(Convert.ToString(gameId)) */
/*         .SendAsync( */
/*             "gameStateUpdated", */
/*             newGame.State.State, */
/*             newGame.PlayerTurn, */
/*             nextPlayerPossibleMoves */
/*         ); */
/**/
/*     return Ok(); */
/* } */

/* [HttpPost("games/{id}/action")] */
/* public ActionResult<Game> PlayerAction([FromBody] PlayerActionRequest playerAction) => */
/* public ActionResult<Game> PlayerAction([FromBody] PlayerActionRequest playerAction) => */
    /*     Ok(this._othelloService.PlayerActionAsync()); */


