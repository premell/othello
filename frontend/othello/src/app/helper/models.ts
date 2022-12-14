export enum Color {
  Black = 0,
  White = 1,
  None = -1
}


export interface mark {
  squareNumber: number
  color: Color
}

/* export interface GameState {
  // The color of the mark on every square (including None)
  SquareColors: Color[],
  MoveNumber: number
} */

export interface game {
  id: number,
  timeLimit: number,
  timeIncrement: number,
  gameStatus: string,
  whiteTimeRemaining: number,
  blackTimeRemaining: number,
  moves: move[]
}

export interface attemptMove {
  playerColor: Color,
	moveNumber: number,
	targetSquare: number,
	remainingTime: number,
}

export interface move {
  playerColor: Color,
	moveNumber: number,
	targetSquare: number,
	remainingTime: number,
  resultingState: number[]
}

// export enum ErrorType {
//   IllegalMove
// }

export interface invalidMove {
  message: string,
  //Type: ErrorType.IllegalMove
}



// THEMES
export interface theme {
    squareColor: string,
    squareHover: string,
    whiteMarkColor: string,
    blackMarkColor: string,
    markOutline: string,
    mainButtonColor: string,
    mainButtonHover: string,
    secondaryButtonColor: string,
    secondaryButtonHover: string,

    backgroundColor: string,
    containerColor: string,

    strongTextColor: string,
    regularTextColor: string,
    weakTextColor: string,
}

export interface themes {
  light: theme;
  dark: theme;
}

