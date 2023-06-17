using ArgumentException = System.ArgumentException;

public static class GameStateUtil
{
    public static string PlaceMark(string stateAsString, int targetSquare, Color playerColor)
    {
        var color = (int)playerColor;
        var opponentColor = (int)Utils.GetOppositeColor(playerColor);

        var state = StateStringToArray(stateAsString);

        int[] newState = (int[])state.Clone();
        newState[targetSquare] = color;

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            int position = targetSquare + (int)direction;
            List<int> flippedSquares = new List<int>();

            while (IsValidMove(targetSquare, position, direction) && newState[position] == opponentColor)
            {
                flippedSquares.Add(position);
                position += (int)direction;
            }

            if (IsValidMove(targetSquare, position, direction) && newState[position] == color)
            {
                foreach (int square in flippedSquares)
                {
                    newState[square] = color;
                }
            }
        }

        if (StateArrayToString(newState) == stateAsString)
        {
            throw new ArgumentException("Invalid move");
        }

        return StateArrayToString(newState);
    }

    public static List<int> GetLegalMoves(string state, Color playerColor)
    {
        var color = (int)playerColor;
        var opponentColor = (int)Utils.GetOppositeColor(playerColor);

        var legalMoves = new List<int>();
        var board = StateStringToArray(state);

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] != (int)Color.NONE)
            {
                continue;
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int position = i + (int)direction;
                if (!IsValidMove(i, position, direction)) continue;

                var isValidMove = false;

                // found opponent piece
                if (board[position] != opponentColor) continue;

                position += (int)direction;

                while (IsValidMove(i, position, direction))
                {
                    if (board[position] == (int)Color.NONE) break;
                    else if (board[position] == color)
                    {
                        legalMoves.Add(i);
                        isValidMove = true;
                        break;
                    }
                    position += (int)direction;

                }
                if (isValidMove) break;
            }
        }

        return legalMoves;
    }


private static bool IsValidMove(int currentPosition, int newPosition, Direction direction)
{

    if ( newPosition is < 0 or >= 64) return false;


    int currentRow = currentPosition / 8;
    int currentCol = currentPosition % 8;
    int newRow = newPosition / 8;
    int newCol = newPosition % 8;


    switch (direction)
    {
        case Direction.UP:
            return currentCol == newCol && newRow < currentRow;
        case Direction.DOWN:
            return currentCol == newCol && newRow > currentRow;
        case Direction.LEFT:
            return currentRow == newRow && currentCol > newCol;
        case Direction.RIGHT:
            return currentRow == newRow && currentCol < newCol;
        case Direction.UP_LEFT:
            return currentRow > newRow && currentCol > newCol && (currentRow - newRow) == (currentCol - newCol);
        case Direction.DOWN_RIGHT:
            return currentRow < newRow && currentCol < newCol && (newRow - currentRow) == (newCol - currentCol);
        case Direction.UP_RIGHT:
            return currentRow > newRow && currentCol < newCol && (currentRow - newRow) == (newCol - currentCol);
        case Direction.DOWN_LEFT:
            return currentRow < newRow && currentCol > newCol && (newRow - currentRow) == (currentCol - newCol);
        default:
            throw new ArgumentOutOfRangeException();
    }
}


    private static string StateArrayToString(int[] state)
    {
        return string.Join(",", state);
    }

    private static int[] StateStringToArray(string state)
    {
        string[] stateArray = state.Split(',');
        int[] result = new int[stateArray.Length];
        for (int i = 0; i < stateArray.Length; i++)
        {
            if (string.IsNullOrEmpty(stateArray[i]))
            {
                result[i] = (int)Color.NONE;
            }
            else
            {
                result[i] = int.Parse(stateArray[i]);
            }
        }
        return result;
    }

    public static int GetBlackMarksCount(string state)
    {
        return state.Count(c => c == '0');
    }

    public static int GetWhiteMarksCount(string state)
    {
        return state.Count(c => c == '1');
    }
}

public enum Direction
{
    UP = -8,
    UP_RIGHT = -7,
    RIGHT = 1,
    DOWN_RIGHT = 9,
    DOWN = 8,
    DOWN_LEFT = 7,
    LEFT = -1,
    UP_LEFT = -9
}
