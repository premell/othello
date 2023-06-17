using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// using System.Linq.Dynamic;

namespace othello_api.Models;

public class GameMove
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int GameId { get; set; }

    // this should be playercolor
    [Required]
    public Color PlayerColor { get; set; }

    [Required]
    public int Square { get; set; }

    // starts from 0 
    [Required]
    public int MoveNumber { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [MaxLength(250)]
    [Required]
    public string ResultingState { get; set; }
}


public class Game
{
    [Required]
    public int Id { get; set; }

    [MaxLength(250)]
    [Required]
    public string State { get; set; }

    [Required]
    public List<GameMove> Moves { get; set; } = new List<GameMove>();

    [Required]
    public int TimeLimitSeconds { get; set; }

    [Required]
    public int TimeIncrementSeconds { get; set; }
    [Required]
    public DateTime GameStarttime { get; set; } = DateTime.Now;

    [Required]
    public GameStatus? Status { get; set; } = GameStatus.PLAYING;

    [Required]
    public Color? Winner { get; set; } = Color.NONE;

    [Required]
    public Color PlayerTurn { get; set; }

    // TODO make this cooler
    [MaxLength(32)]
    [Required]
    public string SessionId { get; set; }

    public Game(int timeLimitSeconds, int timeIncrementSeconds, string sessionId)
    {
        TimeLimitSeconds = timeLimitSeconds;
        TimeIncrementSeconds = timeIncrementSeconds;
        SessionId = sessionId;
        State = ",,,,,,,,,,,,,,,,,,,,,,,,,,,1,0,,,,,,,0,1,,,,,,,,,,,,,,,,,,,,,,,,,,,";
    }
}














/* public List<int> PossibleMoves(Color playerColor) */
/* { */
/*     Console.WriteLine(playerColor); */
/**/
/*     var color = ((int)playerColor).ToString(); */
/*     var opponentColor = (((int)playerColor + 1) % 2).ToString(); */
/**/
/*     var squares = this.StateStringToList(this.State); */
/**/
/*     var possibleMoves = new List<int>(); */
/**/
/**/
/*     for (int i = 0; i < 63; i++) */
/*     { */
/*         if(squares[i] == "") continue; */
/*         foreach (var direction in Enum.GetValues<Direction>()) */
/*         { */
/*             var squareToCheck = Int32.Parse(squares[i]) + (int)direction; */
/*             if ( */
/*                 squareToCheck > 63 */
/*                 || squareToCheck < 0 */
/*                 || squares[squareToCheck] != opponentColor */
/*             ) */
/*             continue; */
/**/
/*             // var squaresToChange = new List<int>(); */
/**/
/*             // squaresToChange.Add(squareToCheck); */
/*             squareToCheck += (int)direction; */
/**/
/*             var hasSeenOppositeColor = false; */
/*             var hasFoundPossibleMove = false; */
/**/
/*             while (squareToCheck <= 63 && squareToCheck >= 0) */
/*             { */
/*                 if (squares[squareToCheck] == "") */
/*                     break; */
/*                 else if (squares[squareToCheck] == opponentColor){ */
/*                     hasSeenOppositeColor = true; */
/*                 } */
/*                 else if (squares[squareToCheck] == color) */
/*                 { */
/*                     if(hasSeenOppositeColor) possibleMoves.Add(i); */
/*                     hasFoundPossibleMove = true; */
/*                     break; */
/*                 } */
/*             } */
/**/
/*             if(hasFoundPossibleMove) break; */
/*         } */
/*     } */
/**/
/*     Console.WriteLine("POSSIBLE MOVES ", possibleMoves); */
/**/
/*     return possibleMoves; */
/* } */
/**/


/* public string PlaceMark(int targetSquare, Color playerColor) */
/* { */
/*     var squares = this.StateStringToList(this.State); */
/**/
/*     if (targetSquare > squares.Count) */
/*         throw new Exception("target square is outside of board"); */
/**/
/*     if (string.IsNullOrEmpty(squares[targetSquare])) */
/*         throw new Exception("the target square already taken"); */
/**/
/*     return this.CalculateResultingState(this.State, targetSquare, playerColor); */
/*     // if(GetNumberOfMarksPlaced(newState) != GetNumberOfMarksPlaced(State) + 1) throw new Exception("Not correct numbers of marks placed"); */
/* } */
/**/


/* private static bool IsValidTransition(int startPosition, int newPosition, Direction direction) */
/* { */
/*     if (newPosition < 0 || newPosition >= 64) return false; */
/**/
/*     int startRow = startPosition / 8; */
/*     int newRow = newPosition / 8; */
/*     int startColumn = startPosition % 8; */
/*     int newColumn = newPosition % 8; */
/**/
/*     int rowDiff = newRow - startRow; */
/*     int colDiff = newColumn - startColumn; */
/**/
/*     int dirRow = (int)direction / 8; */
/*     int dirCol = (int)direction % 8; */
/**/
/*     // Check if the move is valid along the direction */
/*     if (dirRow == 0) */
/*     { */
/*         return colDiff == dirCol || (colDiff != 0 && colDiff % dirCol == 0); */
/*     } */
/*     else if (dirCol == 0) */
/*     { */
/*         return rowDiff == dirRow || (rowDiff != 0 && rowDiff % dirRow == 0); */
/*     } */
/*     else */
/*     { */
/*         return rowDiff == dirRow && colDiff == dirCol; */
/*     } */
/* } */
/**/

/* public List<int> GetLegalMoves(Color playerColor) */
/* { */
/*     var color = ((int)playerColor).ToString(); */
/*     var opponentColor = (((int)playerColor + 1) % 2).ToString(); */
/*     /* string opponentColor = playerColor == "0" ? "1" : "0"; */
/*     var legalMoves = new List<int>(); */
/*     string[] board = StateStringToArray(this.State); */
/**/
/*     for (int i = 0; i < board.Length; i++) */
/*     { */
/*         if (board[i] != "") */
/*         { */
/*             continue; */
/*         } */
/**/
/*         foreach (int direction in Enum.GetValues<Direction>()) */
/*         { */
/*             int position = i + direction; */
/*             bool foundOpponentPiece = false; */
/**/
/*             while (IsInBounds(position) && board[position] == opponentColor) */
/*             { */
/*                 foundOpponentPiece = true; */
/*                 position += direction; */
/*             } */
/**/
/*             if (foundOpponentPiece && IsInBounds(position) && board[position] == color) */
/*             { */
/*                 legalMoves.Add(i); */
/*                 break; */
/*             } */
/*         } */
/*     } */
/**/
/*     return legalMoves; */
/* } */

/* private static bool IsValidTransition(int startPosition, int newPosition, int direction) */
/* { */
/*     if (newPosition < 0 || newPosition >= 64) return false; */
/**/
/*     int startRow = startPosition / 8; */
/*     int newRow = newPosition / 8; */
/*     int startColumn = startPosition % 8; */
/*     int newColumn = newPosition % 8; */
/**/
/*     int rowDiff = newRow - startRow; */
/*     int colDiff = newColumn - startColumn; */
/**/
/*     // Check if the move is valid along the direction */
/*     return rowDiff == direction / 8 && colDiff == direction % 8; */
/* } */

