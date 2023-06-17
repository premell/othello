import {
  derived,
  get,
  readable,
  writable,
  type Readable,
  type Writable,
} from "svelte/store";
import * as signalR from "@microsoft/signalr";
//import.meta.env["NODE_TLS_REJECT_UNAUTHORIZED"] = 0;
//import { startConnection, gameStateUpdated, playerAction, gameEnded } from "../signalr.js";
//import { connection } from "../signalr.js";
import type { HubConnection } from "@microsoft/signalr";
import type {
  Game,
  Move,
  PlayerActionRequest,
  SessionInfo,
} from "$lib/models/Models";
import type { ChatMessage } from "$lib/models/Models";
import { GameTimers } from "$lib/timer";
import { COLOR_VALUES } from "$lib/enums/enums";
import type { Color } from "$lib/enums/enums";

// export const currentPlayerTurn: Writable<Game|null> = writable(null)
export const currentGame: Writable<Game> = writable();

export const sessionInfo: Writable<SessionInfo> = writable();

// export const soundMuted: <boolean> = writable(false);
export const soundMuted: Writable<boolean> = writable<boolean>(typeof localStorage !== 'undefined' && localStorage.muted === 'true');
soundMuted.subscribe((value: boolean) => {
  if (typeof localStorage !== 'undefined') {
    localStorage.muted = String(value);
  }
});

export const gameTimers: Readable<GameTimers> = readable(new GameTimers(0, 0));
export const userRemainingTime: Writable<number> = writable(0);
export const opponentRemainingTime: Writable<number> = writable(0);

// all current player actions
export const playerActionRequests: Writable<PlayerActionRequest[]> = writable(
  []
);

// the move number of the state to display, for when you go through the history
export const moveNumberToDisplay: Writable<number> = writable(0);

type HistoricGameStates = {
  [key: number]: string;
};

// TODO make this cool and readonly
// cache earlier gamestates
export const historicGameStates = writable<HistoricGameStates>({});

export const getHistroricGameState = (moveNumber: number): string => {
  let tmpHistoricGameStates = get(historicGameStates);

  if (tmpHistoricGameStates?.[moveNumber])
    return tmpHistoricGameStates[moveNumber];

  let tmpActiveGame = get(currentGame);

  for (let move of tmpActiveGame.moves) {
    if (tmpHistoricGameStates?.[move.moveNumber]) break;

    // move numbers start at 1 right? for some stupid reason. Also TODO refactor this
    if (move.moveNumber == 1) {
      tmpHistoricGameStates[1] =
        ",,,,,,,,,,,,,,,,,,,,,,,,,,,1,0,,,,,,,0,1,,,,,,,,,,,,,,,,,,,,,,,,,,,";
      continue;
    }

    historicGameStates[moveNumber] = getMoveResultingState(
      move.square,
      move.playerColor,
      getColorByNumber((move.playerColor + 1) % 2),
      tmpHistoricGameStates[move.moveNumber - 1]
    );
  }
  historicGameStates.set(tmpHistoricGameStates);

  return tmpHistoricGameStates[moveNumber];
};

// todo unsubscribe
currentGame.subscribe((game) => {
  // Check if the game has the 'moves' property
  // if (game.hasOwnProperty('moves')) {
  // Get the current lookupTable value
  //
  if (!game?.moves) return;

  let currentLookupTable = get(historicGameStates);

  // Get the latest move number and state
  const moveNumber = game.moves.length;
  const resultingState = game.state;

  // Update the lookupTable with the move number and resulting state
  historicGameStates.update((table: HistoricGameStates) => {
    return { ...table, [moveNumber]: resultingState };
  });
  // }

  // can the game be updated some other way?
  // not sure if we should update it every time a new move is made
  moveNumberToDisplay.set(game.moves.length);
});

// export const userTakebackRequested = writable<boolean>(false);
// export const userDrawRequested = writable<boolean>(false);
//
// export const opponentTakebackRequested = writable<boolean>(false);
// export const opponentDrawRequested = writable<boolean>(false);

// TODO make this write only
// TODO put this in a seperate file
//export const gameTimers = writable<GameTimers>();

function getMoveResultingState(
  square: number,
  playerColor: Color,
  arg2: Color,
  arg3: string
): any {
  throw new Error("Function not implemented.");
}

function getColorByNumber(NONE: any): any {
  throw new Error("Function not implemented.");
}
