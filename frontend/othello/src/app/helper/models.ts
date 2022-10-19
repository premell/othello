export enum Color {
  Black = 0,
  White = 1,
  None = -1
}


export interface Mark {
  SquareNumber: number
  Color: Color
}

/* export interface GameState {
  // The color of the mark on every square (including None)
  SquareColors: Color[],
  MoveNumber: number
} */

export interface Game {
  Id: string,
  TimeLimit: number,
  TimeIncrement: number,
  GameStatus: string,
  WhiteTimeRemaining: number,
  BlackTimeRemaining: number,
  Moves: Move[]
}

export interface AttemptMove {
  PlayerColor: Color,
	MoveNumber: number,
	TargetSquare: number,
	RemainingTime: number,
}

export interface Move {
  PlayerColor: Color,
	MoveNumber: number,
	TargetSquare: number,
	RemainingTime: number,
  ResultingState: number[]
}

// export enum ErrorType {
//   IllegalMove
// }

export interface InvalidMove {
  Message: string,
  //Type: ErrorType.IllegalMove
}
