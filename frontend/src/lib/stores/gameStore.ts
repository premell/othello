import {
  readable,
  writable,
  type Readable,
  type Writable,
} from "svelte/store";
import type {
  Game,
  PlayerActionRequest,
  SessionInfo,
} from "$lib/models/Models";
import { GameTimers } from "$lib/timer";

export const currentGame: Writable<Game> = writable();

export const sessionInfo: Writable<SessionInfo> = writable();

export const soundMuted: Writable<boolean> = writable<boolean>(typeof localStorage !== 'undefined' && localStorage.muted === 'true');
soundMuted.subscribe((value: boolean) => {
  if (typeof localStorage !== 'undefined') {
    localStorage.muted = String(value);
  }
});

export const gameTimers: Readable<GameTimers> = readable(new GameTimers(0, 0));
export const userRemainingTime: Writable<number> = writable(0);
export const opponentRemainingTime: Writable<number> = writable(0);

export const playerActionRequests: Writable<PlayerActionRequest[]> = writable(
  []
);

export const moveNumberToDisplay: Writable<number> = writable(0);
