<script lang="ts">
  import { currentGame, sessionInfo } from "$lib/stores/gameStore";
  import FaPlus from "svelte-icons/fa/FaPlus.svelte";
  import { Tooltip, tooltip } from "@svelte-plugins/tooltips";
  import { incrementOpponentTime } from "$lib/signalr";
    import { current_component } from "svelte/internal";
    import { GAME_STATUS_VALUES } from "$lib/enums/enums";

  export let time: number;
  export let isPlayersTurn: boolean;
  export let positionTop: boolean;

  let percentageLeft: string = "100%";

  let showHours = false;
  let showMilliseconds = false;
  let formattedHours: string;
  let formattedMinutes: string;
  let formattedSeconds: string;
  let formattedMilliseconds: number;

  $: {
    let hours = Math.floor(time / 3600);
    let minutes = Math.floor((time % 3600) / 60);
    let seconds = Math.floor(time % 60);

    percentageLeft =
      Math.min(time / $sessionInfo.timeLimitSeconds, 1) * 100 + "%";

    // i say milliseconds but these are actually deciseconds
    formattedMilliseconds = Math.floor((time - Math.floor(time)) * 10);

    if (hours === 0) showHours = false;
    else showHours = true;

    if (hours === 0 && minutes === 0 && seconds < 10) showMilliseconds = true;
    else showMilliseconds = false;

    if (hours < 10) formattedHours = "0" + hours;
    else formattedHours = hours.toString();
    if (minutes < 10) formattedMinutes = "0" + minutes;
    else formattedMinutes = minutes.toString();
    if (seconds < 10) formattedSeconds = "0" + seconds;
    else formattedSeconds = seconds.toString();
  }
</script>

{#if !positionTop}
  <div class="bar_container  bar_container_bottom">
    <div class="bar" style="width: {percentageLeft};" />
  </div>
{/if}
<div class="clock_container">
  <div
    class="clock"
    class:show_hours={showHours}
    class:players_turn={isPlayersTurn}
    class:show_milliseconds={showMilliseconds}
    class:positionTop
    class:positionBottom={!positionTop}
  >
    {#if showHours}
      {formattedHours}<sep>:</sep>
    {/if}
    {formattedMinutes}<sep>:</sep>{formattedSeconds}{#if showMilliseconds}
      <tenths><sep>.</sep>{formattedMilliseconds} </tenths>
    {/if}
  </div>
  {#if positionTop && $currentGame.status === GAME_STATUS_VALUES.PLAYING}
    <div
      class="increment_opponent_time"
      on:click={() => incrementOpponentTime()}
      use:tooltip={{
        content: "Give 15 seconds",
        position: "left",
        autoPosition: true,
        align: "center",
      }}
    >
      <div class="increment_icon">
        <FaPlus />
      </div>
    </div>
  {/if}
</div>

{#if positionTop}
  <div class="bar_container">
    <div class="bar" style="width: {percentageLeft};" />
  </div>
{/if}

<style>
  .clock_container {
    display: flex;
    justify-content: space-between;
  }

  .clock {
    background-color: var(--secondary-color);
    height: 60px;
    font-size: 2.8em;
    letter-spacing: 0.12em;
    line-height: 1.2em;
    font-weight: 700;
    padding: 0 2vmin;
    will-change: transform;
    font-family: "Roboto", sans-serif;
    position: relative;

    display: flex;
    align-items: center;

    box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.14),
      0 3px 1px -2px rgba(0, 0, 0, 0.2), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
  }

  .players_turn {
    background-color: var(--timer-color);
    color: var(--main-text-color);
  }

  .show_hours {
    font-size: 2.5em;
  }

  .show_milliseconds {
    font-size: 2.5em;
    background-color: var(--milliseconds-red);
  }

  .positionTop {
    border-radius: 3px 3px 0 0;
    z-index: 1;
  }

  .positionBottom {
    border-radius: 0 0 3px 3px;
    /* this interferes with the tooltips TODO */
    /* z-index: 11; */
  }

  .bar_container {
    background-color: var(--secondary-color);
    display: block;
    position: relative;
    z-index: 10;
    height: 4px;
    transform-origin: left;
  }

  .bar_container_bottom {
    box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.14),
      0 3px 1px -2px rgba(0, 0, 0, 0.2), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
  }

  .bar {
    height: 4px;
    background: #629924;
  }

  sep {
    opacity: 0.5;
    font-size: 0.8em;
  }
  tenths {
    font-size: 70%;
    margin-bottom: -0.18em;
  }

  .increment_opponent_time {
    display: flex;
    align-items: center;
    padding: 0 0.5em;
  }

  .increment_icon {
    width: 30px;
    height: 30px;
    background-color: var(--big-button-background);
    color: var(--bg-color);
    padding: 5px;
    border-radius: 6px;
    opacity: 0.5;
  }

  .increment_icon:hover {
    background-color: var(--big-button-background-hover);
    cursor: pointer;
    opacity: 1;
  }
</style>
