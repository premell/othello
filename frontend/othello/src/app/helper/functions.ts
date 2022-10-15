import { GameState, Move, InvalidMove, Color } from '../helper/models'

// const generateStateFromMoves = (moves: Move[]): GameState[] => {
//   let States: GameState[] = []
//
//   moves.forEach(move => {
//     //if()
//
//
//
//   });
// }

const changeMarksInLine = (
  lineToCheck: number[],
  gameState: GameState,
  playerColor: Color,
  opponentColor: Color
): GameState => {

  let tmp_arr: number[] = []
  for (const square of lineToCheck) {
    if (gameState.SquareColors[square] === opponentColor) tmp_arr.push(+square)

    else if (gameState.SquareColors[square] === Color.None) break
    else if (gameState.SquareColors[square] === playerColor) {
      tmp_arr.forEach((element) => {
        gameState.SquareColors[element] = playerColor
      })
      break
    }
  }

  return gameState
}

const changeMarks = (
  arraysToCheck: number[][],
  gameState: GameState,
  playerColor: Color,
  opponentColor: Color
): GameState => {
  arraysToCheck.forEach((line) => {
    //console.log("loop")
    gameState = changeMarksInLine(line, gameState, playerColor, opponentColor)
  })

  return gameState
}

export const placeMark = (
  currentGameState: GameState,
  move: Move
): GameState | InvalidMove => {
  let newGameState: GameState = JSON.parse(JSON.stringify(currentGameState));
  const playerColor: Color = move.Color
  const opponentColor: Color = getOpponentColor(playerColor)
  const square = move.TargetSquare

  let arraysToCheck: number[][] = [[], [], [], [], [], [], [], []]

  // Check all marks to left
  for (let i = square - 1; i > square - (square % 8) - 1; i--) {
    arraysToCheck[0].push(i)
  }
  // Check all marks to right
  for (let i = square + 1; i < Math.floor((square + 8) / 8) * 8; i++) {
    arraysToCheck[1].push(i)
  }

  // Check all marks to up
  for (let i = square - 8; i > 0; i = i - 8) {
    arraysToCheck[2].push(i)
  }

  // Check all marks down
  for (let i = square + 8; i < 64; i = i + 8) {
    arraysToCheck[3].push(i)
  }

  // check all marks diagonal down right
  for (let i = square + 9; i < 64; i = i + 9) {
    // so that it doesnt go around the board
    if (i % 8 === 0) break
    arraysToCheck[4].push(i)
  }

  // check all marks diagonal down left
  for (let i = square + 7; i < 64; i = i + 7) {
    // so that it doesnt go around the board
    if (i % 8 === 7) break
    arraysToCheck[5].push(i)
  }

  // check all marks diagonal up right
  for (let i = square - 7; i > 0; i = i - 7) {
    // so that it doesnt go around the board
    if (i % 8 === 0) break
    arraysToCheck[6].push(i)
  }

  // check all marks diagonal up left
  for (let i = square - 9; i > 0; i = i - 9) {
    // so that it doesnt go around the board
    if (i % 8 === 7) break
    arraysToCheck[7].push(i)
  }

  newGameState = changeMarks(
    arraysToCheck,
    newGameState,
    playerColor,
    opponentColor
  )

  if (newGameState.SquareColors + "" == currentGameState.SquareColors + "")
    return {
      Message: 'must change atleast one mark',
    }

  newGameState.SquareColors[square] = playerColor

  return newGameState
}

let beginningBoard: GameState = {
  SquareColors: [],
  MoveNumber: 1,
}

// creates an object with the default state from beginning of a game
export const createDefaultState = (): GameState => {
  if (beginningBoard.SquareColors.length === 0) {
    // populate the board with none values
    for (let i = 0; i < 64; i++) {
      beginningBoard.SquareColors.push(Color.None)
    }

    // place the four beginning marks
    beginningBoard.SquareColors[27] = Color.White
    beginningBoard.SquareColors[28] = Color.Black
    beginningBoard.SquareColors[35] = Color.Black
    beginningBoard.SquareColors[36] = Color.White
  }

  return { ...beginningBoard }
}

export const getOpponentColor = (playerColor: Color): Color =>
  playerColor === Color.White ? Color.Black : Color.White
