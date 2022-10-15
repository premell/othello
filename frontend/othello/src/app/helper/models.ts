export enum Color {
  Black = 0,
  White = 1,
  None = -1
}


export interface Mark {
  SquareNumber: number
  Color: Color
}

export interface GameState {
  // The color of the mark on every square (including None)
  SquareColors: Color[],
  MoveNumber: number
}

export interface Move {
  Color: Color,
	MoveNumber: number,
	TargetSquare: number,
	RemainingTime: number 
}

// export enum ErrorType {
//   IllegalMove
// }

export interface InvalidMove {
  Message: string,
  //Type: ErrorType.IllegalMove
}
