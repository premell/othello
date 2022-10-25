import { game, attemptMove, move, invalidMove, Color } from '../helper/models'

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
  currentState: number[],
  playerColor: Color,
  opponentColor: Color
): number[] => {

  let newState = [...currentState]
  let tmp_arr: number[] = []
  for (const square of lineToCheck) {
    if (newState [square] === opponentColor) tmp_arr.push(+square)

    else if (newState[square] === Color.None) break
    else if (newState[square] === playerColor) {
      tmp_arr.forEach((element) => {
        newState[element] = playerColor
      })
      break
    }
  }

  return newState
}

const changeMarks = (
  arraysToCheck: number[][],
  currentState: number[],
  playerColor: Color,
  opponentColor: Color
): number[] => {

  const newState = arraysToCheck.reduce((state, lineToCheck) => changeMarksInLine(lineToCheck, state, playerColor, opponentColor), currentState)

/*   arraysToCheck.forEach((line) => {
    //console.log("loop")
    gameState = changeMarksInLine(line, gameState, playerColor, opponentColor)
  }) */

  return newState
}

export const getStartingState = (): number[] => [-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,0,1,-1,-1,-1,-1,-1,-1,1,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1]

export const getStateFromString = (stateAsString: string): number[] => stateAsString.split(",").map(string => parseInt(string))

export const getCurrentState = (currentGame: game): number[]=> {
  if(currentGame.moves.length === 0) return getStartingState()
  else return currentGame.moves[currentGame.moves.length - 1].resultingState
}

export const placeMark = (
  currentGame: game,
  move: attemptMove
): move | invalidMove => {
  

  let currentState: number[] = getCurrentState(JSON.parse(JSON.stringify(currentGame))) //JSON.parse(JSON.stringify(currentGameState.Moves[currentGameState.Moves.length - 3].ResultingState));
  const playerColor: Color = move.playerColor
  const opponentColor: Color = getOpponentColor(playerColor)
  const square = move.targetSquare

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

  const newState = changeMarks(
    arraysToCheck,
    currentState,
    playerColor,
    opponentColor
  )

  if (newState + "" == currentState + "")
    return {
      message: 'must change atleast one mark',
    }

  newState[square] = playerColor
  const newMove: move= {
    ...move,
    resultingState: newState
  }

  return newMove
}

/* let beginningBoard: Game = {
  MoveNumber: 1,
} */

// creates an object with the default state from beginning of a game
/* export const createDefaultState = (): GameState => {
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
} */

export const getOpponentColor = (playerColor: Color): Color =>
  playerColor === Color.White ? Color.Black : Color.White
