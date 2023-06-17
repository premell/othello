public enum Color {
  NONE = -1,
  BLACK = 0,
  WHITE = 1
}

// public enum ColorOrNeither : Color
// {
//     // BLACK = 0,
//     // WHITE,
//     NEITHER
// }
//

public enum PlayerAction{
  DRAW,
  TAKEBACK,
  REMATCH
}

public enum ActionType{
  REQUEST,
  CANCEL,
  REJECT, 
  ACCEPT,
}

public enum GameStatus
{
    PLAYING = 0,
    SURRENDERED,
    WON_TIME,
    WON_BY_MARKS,
    CANCELLED,
    DRAW
}

