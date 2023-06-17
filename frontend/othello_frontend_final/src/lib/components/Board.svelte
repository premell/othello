<script lang="ts">
  import {
    currentGame,
    moveNumberToDisplay,
    getHistroricGameState,
    historicGameStates,
    sessionInfo,
  } from "$lib/stores/gameStore";
  import { makeMove } from "$lib/signalr";
  import { stateStringToArray } from "$lib/utils";

  let squares: string[] = [];
  $: {
    //console.log($historicGameStates);
    squares = stateStringToArray(getHistroricGameState($moveNumberToDisplay));
  }

  function cellClicked(e: any, squareIndex: number) {
    // if you click board while looking at a previous state, you get jumped to current state
    if ($moveNumberToDisplay !== $currentGame.moves.length)
      moveNumberToDisplay.set($currentGame.moves.length);
    else if (
      $currentGame?.legalMoves?.includes(squareIndex) &&
      $currentGame.playerTurn === $sessionInfo.userColor
    ) {
      const squareNumber = parseInt(e.srcElement.id);
      makeMove(squareNumber);
    }
  }
</script>

<div class="grid">
  {#each squares as square, i}
    <div
      class="cell"
      class:available_move={$moveNumberToDisplay ===
        $currentGame.moves.length &&
        $currentGame?.legalMoves?.includes(i) &&
        $currentGame.playerTurn === $sessionInfo.userColor}
      on:click={(e) => cellClicked(e, i)}
      id={i.toString()}
      class:last_played_mark={i ===
        $currentGame.moves.find(
          (move) => move.moveNumber === $moveNumberToDisplay
        )?.square}
    >
      {#if square === "0"}
        <div class="mark">
          <img src="/images/black.png" />
        </div>
      {:else if square === "1"}
        <div class="mark">
          <img src="/images/white.png" />
        </div>
      {/if}
    </div>
  {/each}
</div>

<style>
  img {
    max-width: 100%;
    max-height: 100%;
  }

  .grid {
    display: grid;
    grid-template-columns: repeat(8, 1fr);
    grid-template-rows: repeat(8, 1fr);
    gap: 2px;
    width: 100%;
    height: 100%;
    background-color: #ccc;
    border-radius: 5px;
    background-color: #434040;
    overflow: hidden;
    box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.14),
      0 3px 1px -2px rgba(0, 0, 0, 0.2), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
  }

  .cell {
    background-color: white;
    width: 100%;
    height: 100%;

    background-color: var(--board-color);
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .mark {
    border-radius: 100%;
    width: 80%;
    height: 80%;
  }

  .last_played_mark {
    background-color: var(--board-last-played) !important;
  }

  .available_move {
    background-color: var(--board-available);
    pointer-events: fill;
  }

  .available_move:hover {
    cursor: pointer;
    background-color: var(--board-hover);
  }
</style>
