import { PUBLIC_API_BASEURL } from "$env/static/public";

import * as signalR from "@microsoft/signalr";
import { get } from "svelte/store";
import { COLOR_VALUES, GAME_STATUS_VALUES } from "$lib/enums/enums";
import { page } from "$app/stores";
import type {
  Game,
  ChatMessage,
  Move,
  PlayerActionRequest,
  SessionInfo,
} from "$lib/models/Models";

import type {
  Color,
  ActionType,
  GameStatus,
  PlayerAction,
} from "$lib/enums/enums";
import {
  currentGame,
  gameTimers,
  playerActionRequests,
  sessionInfo,
  soundMuted,
} from "$lib/stores/gameStore";
import { getOppositeColor, getPlayerFromColor } from "$lib/utils";
import { goto } from "$app/navigation";

const connection = new signalR.HubConnectionBuilder()
  .configureLogging(signalR.LogLevel.Debug)
  .withUrl(PUBLIC_API_BASEURL + "/Hubs/GameHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
  })
  .withAutomaticReconnect()
  .build();

// SEND MESSAGE TO BACKEND

export function startSession(
  playerColor: Color,
  timeIncrement: number,
  timeLimitSeconds: number
) {
  connection
    .start()
    .then(() => {
      // send a ping every 20 seconds to keep connection alive
      setInterval(() => {
        connection.invoke("Ping").catch((err) => console.error(err));
      }, 20000);

      connection.onclose((e) => {
        console.log("SignalR connection closed due to error: ", e);
      });
      // connection.configureLogging(signalR.LogLevel.Information);

      return connection.invoke(
        "StartSession",
        playerColor,
        timeIncrement,
        timeLimitSeconds
      );
    })
    .catch((err) => {
      console.error("Error calling StartSession method: " + err);
    });
}

export function joinSession(connectionString: string): Promise<boolean> {
  return connection
    .start()
    .then(() => {
      // send a ping every 20 seconds to keep connection alive, kinda stupid tbh
      setInterval(() => {
        connection.invoke("Ping").catch((err) => console.error(err));
      }, 20000);

      return connection.invoke("JoinSession", connectionString)
    })
    .catch((err) => {
      console.error("Error calling JoinSession method: " + err);
    });
}

export function incrementOpponentTime() {
  connection.send(
    "IncrementOpponentTime",
    get(sessionInfo).id,
    get(currentGame).id,
    get(sessionInfo).userColor
  );
}

export function makeMove(targetSquare: number) {
  connection.send(
    "MakeMove",
    get(sessionInfo).id,
    get(currentGame).id,
    get(sessionInfo).userColor,
    targetSquare
  );
}

export function playerActionSend(action: PlayerAction, actionType: ActionType) {
  connection.send(
    "PlayerActionMade",
    get(sessionInfo).id,
    get(sessionInfo).userColor,
    action,
    actionType
  );
}

export function sendMessage(content: string) {
  connection.send(
    "SendMessage",
    get(sessionInfo).id,
    get(sessionInfo).userColor,
    content
  );
}

export function surrender() {
  connection.send("Surrender", get(sessionInfo).id);
}

// RECIEVE MESSAGE FROM BACKEND
connection.on(
  "gameEnded",
  (winner: Color, status: GameStatus, blackWins: number, whiteWins: number) => {
    console.log("game ended");
    sessionInfo.update((session) => {
      return {
        ...session,
        blackWins,
        whiteWins,
      };
    });
    if (!get(soundMuted)) new Audio("/sounds/game_end.wav").play();


    playerActionRequests.set([]);
    get(gameTimers).stopTimers();


    currentGame.update((game) => {
      return { ...game, winner, status, legalMoves: [], playerTurn: COLOR_VALUES.NONE, };
    });
  }
);

connection.on("playerAction", (playerActions: PlayerActionRequest[]) => {
  playerActionRequests.set(playerActions);
});

connection.on(
  "receiveMessage",
  (senderColor: Color, content: string, time: Date) => {
    sessionInfo.update((sessionInfo: SessionInfo) => {
      return {
        ...sessionInfo,
        messages: [
          ...sessionInfo.messages,
          {
            senderColor: senderColor,
            content: content,
            timeStamp: time,
          } as ChatMessage,
        ],
      };
    });
  }
);

connection.on(
  "sessionJoined",
  (
    sessionId,
    playerColor: Color,
    playerUrlConnection: string,
    opponentUrlConnection: string,
    messages: ChatMessage[],
    blackWins: number,
    whiteWins: number,
    incrementTime: number,
    timeLimitSeconds: number
  ) => {
    console.log("session joined");
    console.log(sessionId);
    sessionInfo.set({
      id: sessionId,
      timeLimitSeconds: timeLimitSeconds,
      timeIncrementSeconds: incrementTime,
      blackWins,
      whiteWins,
      playerUrlConnection: playerUrlConnection,
      opponentUrlConnection: opponentUrlConnection,
      userColor: playerColor,
      opponentColor: getOppositeColor(playerColor),
      messages: messages,
    });
  }
);

connection.on("gameInfo", (game: Game) => {
  // goto game page and start clock
  console.log("game started");
  currentGame.set(game);

  if (!get(soundMuted)) new Audio("/sounds/game_start.wav").play();
  page.subscribe((res) => {
    if (!res.route.id?.includes("/game/[connectionString]"))
      goto(`/game/${get(sessionInfo).playerUrlConnection}`);
  });
});

connection.on(
  "gameStateUpdated",
  (
    state: string,
    playerTurn: Color,
    legalMoves: number[],
    blackTimeRemaining: number,
    whiteTimeRemaining: number,
    timeOfUpdate: Date,
    lastMove?: Move
  ) => {
    if (!get(currentGame)) return;

    let moves = get(currentGame).moves;

    // if the move number is lower than the last move, it means the move was taken back.
    if (
      lastMove &&
      (lastMove?.moveNumber ?? 0) < (moves[moves.length - 1]?.moveNumber ?? 0)
    ) {
      moves.splice(lastMove?.moveNumber ?? 0);
    } else if (
      lastMove &&
      (lastMove?.moveNumber ?? 0) > (moves[moves.length - 1]?.moveNumber ?? 0)
    ) {
      moves.push(lastMove);
      if (!get(soundMuted)) new Audio(`/sounds/move.ogg`).play();
    }

    currentGame.update((game: Game) => {
      return {
        ...game,
        playerTurn,
        state,
        moves: moves,
        legalMoves,
      };
    });

    if (moves.length >= 2) {
      get(gameTimers).startTimers();
    }

    if (get(sessionInfo).userColor === COLOR_VALUES.BLACK) {
      get(gameTimers).syncTimers(
        blackTimeRemaining,
        whiteTimeRemaining,
        getPlayerFromColor(playerTurn),
        new Date(timeOfUpdate)
      );
    } else {
      get(gameTimers).syncTimers(
        whiteTimeRemaining,
        blackTimeRemaining,
        getPlayerFromColor(playerTurn),
        new Date(timeOfUpdate)
      );
    }
  }
);
