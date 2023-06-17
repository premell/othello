import { get } from "svelte/store";
import { COLOR_VALUES, PLAYER_VALUES, type Player } from "$lib/enums/enums";
import type { Color } from "$lib/enums/enums";
import { sessionInfo } from "$lib/stores/gameStore";

export function getPlayerFromColor(color: Color): Player {
  if (color === get(sessionInfo).userColor) return PLAYER_VALUES.USER;
  else return PLAYER_VALUES.OPPONENT;
}

export function getOppositeColor(color: Color): Color {
  return color === COLOR_VALUES.BLACK ? COLOR_VALUES.WHITE : COLOR_VALUES.BLACK;
}

export const stateArrayToString = (state: string[]): string => state.join(",");
export const stateStringToArray = (state: string): string[] => state.split(",");
export const getBlackMarksCount = (state: string): number =>
  [...state].filter((c) => c === "0").length;
export const getWhiteMarksCount = (state: string): number =>
  [...state].filter((c) => c === "1").length;

export function setBoardHeightProperty() {
  const maxAttempts = 100;
  const delay = (ms: number) => new Promise((res) => setTimeout(res, ms));

  const trySetBoardHeight = async (attempts = 0) => {
    let board = document.getElementById("board");

    if (board) {
      let wHeight = window.innerHeight;
      let wWidth = window.innerWidth;
      console.log(wHeight);
      console.log(wWidth);
      let minBoardSize = 300;
      let boardSize;
      let componentSize;

      if (wWidth >= 1200) {
        wWidth = wWidth - 60;
        wHeight = wHeight - 70;

        boardSize = Math.min(wHeight, wWidth * 0.5556); // 55.56% of the smaller dimension

        if (boardSize > 1000) {
          boardSize = 1000;
        }

        const remainingWidth = wWidth - boardSize;
        componentSize = remainingWidth / 2; // split remaining space between the two components

        if (componentSize > 400) {
          componentSize = 400;
        }
      } else if (wWidth >= 900) {
        wWidth = wWidth - 40;
        wHeight = wHeight - 70;

        boardSize = Math.min(wHeight, wWidth * 0.71);

        if (boardSize > 1000) {
          boardSize = 1000;
        }

        const remainingWidth = wWidth - boardSize;
        componentSize = remainingWidth; // split remaining space between the two components

        if (componentSize > 400) {
          componentSize = 400;
        }
        if (componentSize < 250) {
          componentSize = 250;
        }
      } else if (wWidth >= 600) {
        boardSize = Math.min(wHeight - 70, wWidth - 40);
      } else {
        boardSize = Math.min(wHeight - 70, wWidth - 20);
      }

      if (boardSize < minBoardSize) {
        boardSize = minBoardSize;
      }

      document.documentElement.style.setProperty(
        "--board-size",
        `${boardSize}px`
      );
      document.documentElement.style.setProperty(
        "--component-size",
        `${componentSize}px`
      );
      return;
    }

    if (attempts < maxAttempts) {
      await delay(50); // Wait 50ms
      await trySetBoardHeight(attempts + 1); // Await here
    } else {
      console.error("Failed to set board height after max attempts");
    }
  };

  trySetBoardHeight();
}
