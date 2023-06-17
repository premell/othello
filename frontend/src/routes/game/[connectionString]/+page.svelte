<script lang="ts">
  import { page } from "$app/stores";
  import { onMount } from "svelte";
  import Board from "$lib/components/Board.svelte";
  import Chat from "$lib/components/Chat.svelte";
  import ScoreBoard from "$lib/components/ScoreBoard.svelte";

  import { currentGame, sessionInfo } from "$lib/stores/gameStore";
  import { joinSession } from "$lib/signalr";
  import { get } from "svelte/store";
  import { setBoardHeightProperty } from "$lib/utils";

  let connectionError = false;

  onMount(async () => {
    if (!get(sessionInfo)?.id) {
      const connectionSuccessful = await joinSession(
        $page.params.connectionString
      );
      if (!connectionSuccessful) connectionError = true;
    }

    // resize the board size when screen size changes
    window.addEventListener("resize", setBoardHeightProperty);

    // Call the function once to set the property initially
    setBoardHeightProperty();
  });
</script>

{#if connectionError}
  <main class="game_error_container">
    <div class="game_error_text">GAME NOT FOUND</div>

    <h1>Unfortunetly a session with the given id was not found</h1>
  </main>
{/if}

{#if $currentGame?.id}
  <div class="main">
    <div class="chat">
      <Chat />
    </div>
    <div class="board" id="board">
      <Board />
    </div>
    <div class="util">
      <ScoreBoard />
    </div>
  </div>
{/if}

<style lang="scss">
  .main {
    padding: 0 10px;
    max-width: 1850px;
    display: grid;
    grid-gap: 20px;
    grid-template-columns: var(--board-size);
    grid-template-rows: var(--board-size) 400px 400px;
    //background-color: blue;

    grid-template-areas:
      "board"
      "util"
      "chat";
  }

  .chat,
  .util {
    max-width: 400px;
  }

  .chat {
    box-sizing: border-box;
  }

  // kinda small
  @media (min-width: 600px) {
    .main {
      grid-template-columns: calc(var(--board-size) / 2) calc(
          var(--board-size) / 2
        );
      grid-template-rows: var(--board-size) 400px;
      grid-template-areas:
        "board board"
        "util chat";
    }
    .chat,
    .util {
      height: 100%;
    }
  }

  // medium
  @media (min-width: 900px) {
    .main {
      min-width: 100px;
      grid-template-columns: var(--board-size) var(--component-size);
      grid-template-rows: 380px calc(var(--board-size) - 380px - 2vmin);

      grid-template-areas:
        "board util"
        "board chat";
    }
    .chat,
    .util {
      height: 100%;
    }
  }

  // grid-template-rows: auto fit-content(0) fit-content(0);
  // large
  @media (min-width: 1200px) {
    .main {
      margin: auto;
      grid-template-rows: var(--board-size);

      grid-template-columns: var(--component-size) var(--board-size) var(
          --component-size
        );
      grid-template-areas: "chat board util";
    }

    .chat,
    .util {
      height: 100%;
    }

    .util {
      display: flex;
      align-items: center;
    }

    .chat {
      padding: 2vmin 0;
    }
  }

  .chat {
    grid-area: chat;
  }

  .board {
    grid-area: board;
  }

  .util {
    grid-area: util;
  }

  .game_error_container {
    margin-top: 10vh;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .game_error_text {
    font-weight: 900;
    font-size: 60px;
  }
</style>
