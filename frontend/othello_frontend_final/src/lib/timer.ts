import { PLAYER_VALUES, type Player } from "$lib/enums/enums";
import {
  opponentRemainingTime,
  userRemainingTime,
} from "$lib/stores/gameStore";

export class PlayerTimer {
  private remainingTime: number;
  private remainingTimeDuringLastSync: number;
  private lastSyncTime: Date;
  private intervalId: NodeJS.Timeout | null;
  private currentInterval: number;
  private player: Player;

  constructor(initialTime: number, player: Player) {
    this.remainingTime = initialTime;
    this.lastSyncTime = new Date();
    this.remainingTimeDuringLastSync = initialTime;
    this.intervalId = null;
    this.currentInterval = 1000;

    this.player = player;
  }

  updateTimeInterval(interval: number) {
    // clear previous interval if any
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }

    // Set the new interval
    this.intervalId = setInterval(() => {
      this.updateTime();
    }, interval);
  }

  updateTime() {
    const currentTime = new Date();
    const elapsedTime =
      currentTime.getTime() / 1000 - this.lastSyncTime.getTime() / 1000;
    this.remainingTime = Math.max(
      this.remainingTimeDuringLastSync - elapsedTime,
      0
    );

    if (this.player === PLAYER_VALUES.USER) {
      userRemainingTime.set(this.remainingTime);
    } else {
      opponentRemainingTime.set(this.remainingTime);
    }

    // If the time requirement for the interval has changed, update the interval
    let newInterval = this.remainingTime <= 10 ? 100 : 1000;

    if (newInterval !== this.currentInterval) {
      this.currentInterval = newInterval;
      this.updateTimeInterval(newInterval);
    }
  }

  start() {
    if (this.intervalId) return;

    let intervalTime = this.remainingTime <= 10 ? 100 : 1000;

    this.intervalId = setInterval(() => {
      this.updateTime();
    }, intervalTime);
  }

  pause() {
    if (!this.intervalId) return;

    clearInterval(this.intervalId);
    this.intervalId = null;
  }

  sync(remainingTime: number, syncedAt: Date) {
    this.remainingTime = remainingTime;
    this.remainingTimeDuringLastSync = remainingTime;
    this.lastSyncTime = syncedAt;

    this.updateTime();
  }

  getRemainingTime() {
    return this.remainingTime;
  }
}

export class GameTimers {
  userTimer: PlayerTimer;
  opponentTimer: PlayerTimer;
  timersHasStarted: boolean;

  constructor(playerTime: number, opponentTime: number) {
    this.userTimer = new PlayerTimer(playerTime, PLAYER_VALUES.USER);
    this.opponentTimer = new PlayerTimer(opponentTime, PLAYER_VALUES.OPPONENT);
    this.timersHasStarted = false;
  }

  syncTimers(
    userRemainingTime: number,
    opponentRemainingTime: number,
    currentTurnPlayer: Player,
    syncedAt: Date
  ) {
    this.userTimer.sync(userRemainingTime, syncedAt);
    this.opponentTimer.sync(opponentRemainingTime, syncedAt);

    if (!this.timersHasStarted) return;

    if (currentTurnPlayer === PLAYER_VALUES.USER) {
      this.userTimer.start();
      this.opponentTimer.pause();
    } else {
      this.opponentTimer.start();
      this.userTimer.pause();
    }
  }

  startTimers() {
    this.timersHasStarted = true;
  }

  stopTimers() {
    this.opponentTimer.pause();
    this.userTimer.pause();
    this.timersHasStarted = false;
  }
}
