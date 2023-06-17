import type { Color, GameStatus, PlayerAction } from "$lib/enums/enums";

export interface Move {
  playerColor: Color;
  square: number;
  moveNumber: number;
  resultingState: string;
}

export interface Game {
  id: number;
  legalMoves: number[];
  state: string;
  playerTurn: Color;
  status: GameStatus;
  winner: Color;
  moves: Move[];
  gameStarttime: Date;
}

export interface SessionInfo {
  id: number;
  playerUrlConnection: string,
  opponentUrlConnection: string,
  timeLimitSeconds: number;
  timeIncrementSeconds: number;
  blackWins: number;
  whiteWins: number;
  userColor: Color;
  opponentColor: Color;
  messages: ChatMessage[];
}

export interface ChatMessage {
  senderColor: Color;
  content: string;
  timeStamp: Date;
}

export interface PlayerActionRequest {
  playerColor: Color;
  action: PlayerAction;
}
