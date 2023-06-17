<script lang="ts">
  import { get } from "svelte/store";
  import { Tooltip, tooltip } from "@svelte-plugins/tooltips";
  import {
    ACTION_TYPE_VALUES,
    PLAYER_ACTION_VALUES,
    type ActionType,
    type PlayerAction,
    COLOR_VALUES,
    GAME_STATUS_VALUES,
  } from "$lib/enums/enums";
  import type { Move, PlayerActionRequest } from "$lib/models/Models";
  import {
    currentGame,
    gameTimers,
    moveNumberToDisplay,
    opponentRemainingTime,
    playerActionRequests,
    sessionInfo,
    userRemainingTime,
  } from "$lib/stores/gameStore";
  import FaRegHandshake from "svelte-icons/fa/FaRegHandshake.svelte";
  import FaUndoAlt from "svelte-icons/fa/FaUndoAlt.svelte";
  import FaRegFlag from "svelte-icons/fa/FaRegFlag.svelte";
  import FaStepBackward from "svelte-icons/fa/FaStepBackward.svelte";
  import FaStepForward from "svelte-icons/fa/FaStepForward.svelte";
  import FaFastBackward from "svelte-icons/fa/FaFastBackward.svelte";
  import FaFastForward from "svelte-icons/fa/FaFastForward.svelte";
  import FaCheck from "svelte-icons/fa/FaCheck.svelte";
  import FaTimes from "svelte-icons/fa/FaTimes.svelte";

  import { playerActionSend, surrender } from "$lib/signalr";
  import Timer from "$lib/components/Timer.svelte";
  import { onMount, tick } from "svelte";
  import { getOppositeColor } from "$lib/utils";

  let movesWithSkipTurns: GameMoveWithSkips[] = [];

  let isPressingSurrender = false;
  let surrenderProgress = 0;
  function handleMousedownSurrender() {
    isPressingSurrender = true;
    let start = Date.now();

    const timer = setInterval(() => {
      if (!isPressingSurrender) {
        clearInterval(timer);
        surrenderProgress = 0;
      } else {
        surrenderProgress = Math.min((Date.now() - start) / 1000, 1);
        if (surrenderProgress === 1) {
          surrender();
          clearInterval(timer);
        }
      }
    }, 10);
  }

  function handleMouseupSurrender() {
    isPressingSurrender = false;
    surrenderProgress = 0;
  }

  function getEndGameText() {
    if ($currentGame.winner == COLOR_VALUES.NONE) return "Game tie";

    let gameWinner =
      $currentGame.winner === COLOR_VALUES.WHITE ? "White" : "Black";
    let gameLoser =
      getOppositeColor($currentGame.winner) === COLOR_VALUES.WHITE
        ? "White"
        : "Black";

    let leftStatement: string;
    switch ($currentGame.status) {
      case GAME_STATUS_VALUES.SURRENDERED:
        leftStatement = `${gameLoser} gave up`;
        break;
      case GAME_STATUS_VALUES.WON_TIME:
        leftStatement = "Timeout";
        break;
      case GAME_STATUS_VALUES.WON_BY_MARKS:
        leftStatement = "More marks";
        break;
      case GAME_STATUS_VALUES.CANCELLED:
        leftStatement = "Cancelled";
        break;
      default:
        leftStatement = "strange";
    }

    return leftStatement + ` • ${gameWinner} won`;
  }

  type RequestsMapType = {
    [K in PlayerAction]: string;
  };
  let userRequestsMap: RequestsMapType = {
    [PLAYER_ACTION_VALUES.DRAW]: "Draw offer sent",
    [PLAYER_ACTION_VALUES.TAKEBACK]: "Takeback offer sent",
    [PLAYER_ACTION_VALUES.REMATCH]: "Rematch offer sent",
  };

  let opponentRequestsMap: RequestsMapType = {
    [PLAYER_ACTION_VALUES.DRAW]: "Your opponent offers a draw",
    [PLAYER_ACTION_VALUES.TAKEBACK]: "Your opponent proposes a takeback",
    [PLAYER_ACTION_VALUES.REMATCH]: "Your opponent requested a rematch",
  };

  $: {
    let tmpMoves: GameMoveWithSkips[] = [];
    $currentGame.moves.forEach((move: Move, index: number) => {
      // has a turn been skipped?
      if ((move.playerColor + move.moveNumber) % 2 === 0) {
        tmpMoves.push({
          turnSkipped: true,
          square: 64 + index,
        } as GameMoveWithSkips);
      }

      tmpMoves.push(move);
    });
    movesWithSkipTurns = tmpMoves;
  }

  interface GameMoveWithSkips extends Move {
    turnSkipped?: boolean;
  }

  function viewEarlierGameState(moveNumber: number) {
    moveNumberToDisplay.set(moveNumber);
  }

  function playerAction(action: PlayerAction, actionType: ActionType) {
    playerActionSend(action, actionType);
  }

  let movesContainer: any;

  $: {
    scrollMovesContainer($moveNumberToDisplay);
  }

  function updateNumberToDisplay(numberToDisplay: number) {
    moveNumberToDisplay.set(
      Math.max(Math.min(numberToDisplay, $currentGame.moves.length), 1)
    );
  }

  async function scrollMovesContainer(numberToDisplay: number) {
    await tick();

    let scrollHeight = (Math.ceil($moveNumberToDisplay / 2) - 0.5) * 25 - 50;
    scrollHeight = Math.max(scrollHeight, 0);
    if (movesContainer) movesContainer.scrollTop = scrollHeight;
  }
</script>

<div class="outer-container">
  <div class="container">
    <Timer
      positionTop={true}
      time={$opponentRemainingTime}
      isPlayersTurn={$currentGame.playerTurn === $sessionInfo.opponentColor}
    />
    <div class="main_container">
      <div class="mark_containers">
        <div class="mark">
          <img src="/images/white.png" />
        </div>

        <div class="mark_score">
          {$currentGame.state
            .split(",")
            .filter((m) => m === COLOR_VALUES.WHITE + "").length}

          - {$currentGame.state
            .split(",")
            .filter((m) => m === COLOR_VALUES.BLACK + "").length}
        </div>

        <div class="mark">
          <img src="/images/black.png" />
        </div>
      </div>

      {#if $currentGame.status === GAME_STATUS_VALUES.PLAYING}
        <div class="replay">
          <div
            class="replay_icon_container"
            class:icon_disabled={$moveNumberToDisplay <= 1}
            on:click={() => updateNumberToDisplay(0)}
          >
            <div class="replay_icon">
              <FaFastBackward />
            </div>
          </div>
          <div
            class:icon_disabled={$moveNumberToDisplay <= 1}
            class="replay_icon_container"
            on:click={() => updateNumberToDisplay($moveNumberToDisplay - 1)}
          >
            <div class="replay_icon">
              <FaStepBackward />
            </div>
          </div>
          <div
            class="replay_icon_container"
            class:icon_disabled={$moveNumberToDisplay >=
              $currentGame.moves.length}
            on:click={() => updateNumberToDisplay($moveNumberToDisplay + 1)}
          >
            <div class="replay_icon">
              <FaStepForward />
            </div>
          </div>
          <div
            class="replay_icon_container"
            class:icon_disabled={$moveNumberToDisplay >=
              $currentGame.moves.length}
            on:click={() => updateNumberToDisplay($currentGame.moves.length)}
          >
            <div class="replay_icon">
              <FaFastForward />
            </div>
          </div>
        </div>

        <div class="moves" bind:this={movesContainer}>
          {#each movesWithSkipTurns as move (move.square)}
            <div
              class="move"
              class:move_skipped={move.turnSkipped}
              class:current_move={move.moveNumber === $moveNumberToDisplay}
              on:click={() => viewEarlierGameState(move.moveNumber)}
            >
              {#if move.turnSkipped}
                no moves
              {:else}
                {move.moveNumber}
              {/if}
            </div>
          {/each}

          {#if movesWithSkipTurns.length % 2 !== 0}
            <div class="move move_hidden" />
          {/if}
        </div>
      {:else}
        <!-- <div>game over!</div> -->
        <div class="gameover_container">
          <p class="gameover_score">
            {$sessionInfo.whiteWins}-{$sessionInfo.blackWins}
          </p>

          <p class="gameover_text">
            {getEndGameText()}
          </p>
          {#if }
          <div
            class="rematch_button"
            class:disabled={$playerActionRequests.some(
              (request) => request.action === PLAYER_ACTION_VALUES.REMATCH
            )}
            on:click={() =>
              playerAction(
                PLAYER_ACTION_VALUES.REMATCH,
                ACTION_TYPE_VALUES.REQUEST
              )}
          >
            REMATCH
          </div>
        </div>
      {/if}

      {#if $currentGame.status === GAME_STATUS_VALUES.PLAYING}
        <div class="useraction">
          <div
            class="useraction_icon_container"
            class:disabled={$playerActionRequests.some(
              (request) => request.action === PLAYER_ACTION_VALUES.TAKEBACK
            ) || $currentGame.moves.length === 0}
            use:tooltip={{
              content: "Request takeback",
              position: "bottom",
              autoPosition: true,
              align: "center",
            }}
            on:click={() =>
              playerAction(
                PLAYER_ACTION_VALUES.TAKEBACK,
                ACTION_TYPE_VALUES.REQUEST
              )}
          >
            <div class="useraction_icon">
              <FaUndoAlt />
            </div>
          </div>
          <div
            class="useraction_icon_container"
            class:disabled={$playerActionRequests.some(
              (request) => request.action === PLAYER_ACTION_VALUES.DRAW
            )}
            use:tooltip={{
              content: "Offer draw",
              position: "bottom",
              autoPosition: true,
              align: "center",
            }}
            on:click={() =>
              playerAction(
                PLAYER_ACTION_VALUES.DRAW,
                ACTION_TYPE_VALUES.REQUEST
              )}
          >
            <div class="useraction_icon  draw_icon">
              <FaRegHandshake />
            </div>
          </div>

          <div
            class="useraction_icon_container surrender_icon_container"
            on:mousedown={handleMousedownSurrender}
            on:mouseup={handleMouseupSurrender}
            on:mouseleave={handleMouseupSurrender}
            style:--surrender-progress={surrenderProgress}
            use:tooltip={{
              content: "Surrender",
              position: "bottom",
              autoPosition: true,
              align: "center",
            }}
          >
            <div class="useraction_icon">
              <FaRegFlag />
            </div>
          </div>
        </div>
      {/if}
      {#each $playerActionRequests as request (request.action + request.playerColor)}
        {#if request.playerColor === $sessionInfo.userColor}
          <div class="request requst_with_padding">
            <p class="request_message">
              {userRequestsMap[request.action]}
            </p>
            <div
              class="cancel_request_button"
              on:click={() =>
                playerAction(request.action, ACTION_TYPE_VALUES.CANCEL)}
            >
              Cancel
            </div>
          </div>
        {:else}
          <div class="request">
            <div
              class="reject_opponent_request"
              on:click={() =>
                playerAction(request.action, ACTION_TYPE_VALUES.REJECT)}
              use:tooltip={{
                content: "Decline",
                position: "bottom",
                autoPosition: true,
                align: "center",
              }}
            >
              <div class=" useraction_icon">
                <FaTimes />
              </div>
            </div>
            <p class="request_message">
              {opponentRequestsMap[request.action]}
            </p>
            <div
              class="accept_opponent_request"
              on:click={() =>
                playerAction(request.action, ACTION_TYPE_VALUES.ACCEPT)}
              use:tooltip={{
                content: "Accept",
                position: "bottom",
                autoPosition: true,
                align: "center",
              }}
            >
              <div class=" useraction_icon">
                <FaCheck />
              </div>
            </div>
          </div>
        {/if}
      {/each}
    </div>
    <Timer
      positionTop={false}
      time={$userRemainingTime}
      isPlayersTurn={$currentGame.playerTurn === $sessionInfo.userColor}
    />
  </div>
</div>

<style>
  .outer-container {
    width: 100%;
    position: relative;
  }

  .container {
    width: 100%;
  }

  .main_container {
    position: relative;
    z-index: 2;
    background: grey;
    background-color: var(--secondary-color);
    border-radius: 0 3px 3px 0;
  }

  .mark_containers {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 10px;
    gap: 10px;
  }

  .mark_score {
    font-size: 2.4em;
    font-weight: 700;
  }

  .mark {
    height: 40px;
    width: 40px;
  }

  .replay {
    background: var(--secondary-color);
  }

  .replay,
  .useraction {
    display: flex;
    width: 100%;
    align-items: center;
    flex-grow: 1;
  }

  .moves {
    align-content: start;
    display: flex;
    flex-wrap: wrap;
    height: 100px;
    overflow-y: auto;
  }
  .move {
    height: 25px;
    flex: 1 1 50%;
    box-sizing: border-box;
    padding-left: 0.7em;
  }
  .current_move {
    font-weight: bold;
    background-color: var(--orange-light);
    color: var(--dark-text);
  }

  .move_skipped {
    background-color: var(--bg-color);
  }

  .move:hover {
    cursor: pointer;
    color: white;
    background-color: var(--orange-strong);
  }

  .move_hidden {
    visibility: hidden;
  }

  .useraction {
    height: 40px;
    margin-top: 10px;
    width: 100%;
    align-items: center;
    justify-content: center;
  }

  .useraction_icon_container,
  .replay_icon_container {
    align-items: center;
    text-align: center;
  }

  .replay_icon_container {
    width: 100%;
    padding: 5px 0;
  }

  .replay_icon_container:hover:not(.icon_disabled),
  .useraction_icon_container:hover:not(.icon_disabled) {
    background-color: var(--light-green-accent-color);
    color: white;
    cursor: pointer;
  }

  .surrender_icon_container:hover {
    background: linear-gradient(
      to right,
      var(--light-red-accent-color) calc(var(--surrender-progress) * 100%),
      var(--light-green-accent-color) calc(var(--surrender-progress) * 100%),
      var(--light-green-accent-color)
    );
  }

  .icon_disabled {
    opacity: 0.5;
  }

  .replay_icon {
    display: inline-block;
    height: 15px;
    width: 15px;
  }

  .useraction_icon {
    padding: 0.25em 1em;
    display: inline-block;
    height: 24px;
    width: 24px;
  }

  .draw_icon {
    width: 30px;
  }

  .request {
    display: flex;
    align-items: center;
    border: 2px solid var(--button-background);
    height: 55px;

    background-color: var(--button-background);
  }

  .requst_with_padding {
    padding: 0 3%;
  }

  .request_message {
    width: 100%;
    text-align: center;
  }

  .cancel_request_button {
    color: white;
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 3px;
    height: 40px;
    width: 30%;
    background-color: var(--cancel-button);
    cursor: pointer;
  }

  .cancel_request_button:hover {
    background-color: var(--orange-very-strong);
  }

  .reject_opponent_request {
    display: flex;
    align-items: center;
    height: 100%;
    background-color: var(--secondary-color);
    color: var(--light-red-accent-color);
  }

  .reject_opponent_request:hover {
    cursor: pointer;
    background: var(--light-red-accent-color);
    color: white;
  }

  .accept_opponent_request {
    display: flex;
    align-items: center;
    height: 100%;
    background-color: var(--secondary-color);
    color: var(--light-green-accent-color);
  }
  .accept_opponent_request:hover {
    cursor: pointer;
    background: var(--light-green-accent-color);
    color: white;
  }

  .disabled {
    opacity: 0.5;
    pointer-events: none;
  }

  .gameover_container {
    background-color: var(--secondary-color);
  }

  .gameover_score {
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 1.5em;
    font-weight: bold;
  }

  .gameover_text {
    display: flex;
    justify-content: center;
    align-items: center;
    font-style: italic;
    font-size: 1em;
  }

  .rematch_button {
    height: 84px;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: var(--rematch-button);

    font-size: 1.2em;
  }

  .rematch_button:hover {
    background-color: var(--rematch-button-hover);
    cursor: pointer;
    color: white;
  }

  img {
    max-width: 100%;
    max-height: 100%;
  }
</style>
