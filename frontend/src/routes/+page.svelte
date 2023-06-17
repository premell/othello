<script lang="ts">
  import { startSession } from "$lib/signalr.js";
  import { sessionInfo } from "$lib/stores/gameStore.js";
  import { tooltip } from "@svelte-plugins/tooltips";
  import RangeSlider from "svelte-range-slider-pips";
  import { COLOR_VALUES, type Color } from "$lib/enums/enums.ts";
  import FaCheck from "svelte-icons/fa/FaCheck.svelte";

  import FaRegCopy from "svelte-icons/fa/FaRegCopy.svelte";
  import { onMount } from "svelte";

  export let data;

  let baseUrl = "";
  onMount(() => (baseUrl = window.location.href));

  let choosenColor: Color = COLOR_VALUES.NONE;
  let formattedChoosenColor = "";
  $: {
    switch (choosenColor) {
      case COLOR_VALUES.NONE:
        formattedChoosenColor = "random";
        break;
      case COLOR_VALUES.WHITE:
        formattedChoosenColor = "white";
        break;
      case COLOR_VALUES.BLACK:
        formattedChoosenColor = "black";
        break;
    }
  }

  let sliderLimitValue = [5];
  let realLimitValue: number = 0;
  $: realLimitValue = mapLimitValue(sliderLimitValue);
  const mapLimitValue = (value: number[]) => {
    if (value[0] <= 20) return value[0];
    const scale = [25, 30, 35, 40, 45, 60, 75, 90, 105, 120, 150, 180];
    return scale[value[0] - 21];
  };

  let sliderIncrementValue = [3];
  let realIncrementValue: number = 0;
  $: realIncrementValue = mapIncrementValue(sliderIncrementValue);
  const mapIncrementValue = (value: number[]) => {
    if (value[0] <= 20) return value[0];
    const scale = [25, 30, 35, 40, 45, 60, 75, 90, 105, 120, 150, 180];
    return scale[value[0] - 21];
  };

  const createNewGame = async () => {
    let color = choosenColor;
    if (choosenColor === COLOR_VALUES.NONE) {
      let rand = Math.round(Math.random());
      color = rand > 0.5 ? COLOR_VALUES.WHITE : COLOR_VALUES.BLACK;
    }

    startSession(color, realIncrementValue, realLimitValue * 60);
  };

  let textCopied = false;
  async function copyOpponentLinkToClipboard() {
    await navigator.clipboard.writeText(
      baseUrl + "game/" + $sessionInfo?.opponentUrlConnection
    );
    textCopied = true;
  }
</script>

{#if !$sessionInfo?.opponentUrlConnection}
  <div class="main_container create_main_container">
    <div class="title">Create new game</div>

    <div>
      Timelimit: <b>{realLimitValue}</b> minutes
    </div>
    <RangeSlider
      id="limitValueSlider"
      formatter={() => realLimitValue}
      bind:values={sliderLimitValue}
      min={1}
      max={32}
      hoverable={false}
      step={1}
    />

    <div>
      Increment: <b>{realIncrementValue}</b> seconds
    </div>
    <RangeSlider
      id="incrementValueSlider"
      formatter={() => realIncrementValue}
      bind:values={sliderIncrementValue}
      min={1}
      max={32}
      step={1}
      hoverable={false}
    />

    <div>
      Color: <b>{formattedChoosenColor}</b>
    </div>

    <div class="color_chooser_container">
      <div
        class="black_color color_button"
        on:click={() => (choosenColor = COLOR_VALUES.BLACK)}
        class:color_active={choosenColor === COLOR_VALUES.BLACK}
      >
        <img src="/images/black.png" />
      </div>
      <div
        class="random_color color_button active big_color_button"
        on:click={() => (choosenColor = COLOR_VALUES.NONE)}
        class:color_active={choosenColor === COLOR_VALUES.NONE}
      >
        <img src="/images/black_white.png" />
      </div>
      <div
        class="white_color color_button"
        on:click={() => (choosenColor = COLOR_VALUES.WHITE)}
        class:color_active={choosenColor === COLOR_VALUES.WHITE}
      >
        <img src="/images/white.png" />
      </div>
    </div>
    <div class="create_button big_button" on:click={createNewGame}>
      Create game
    </div>
  </div>
{:else}
  <div class="main_container link_main_container">
    <div class="title">Share link to challenge a friend!</div>

    <div class="link_text">Timelimit: <b>{realLimitValue}</b> minutes</div>
    <div class="link_text">Increment: <b>{realIncrementValue}</b> seconds</div>
    <div class="link_text">Color: <b>{formattedChoosenColor}</b></div>

    <div class="link_container">
      <div
        class="link_text"
        style="text-align: start;border-right: 3px solid var(--bg-color)"
      >
        {baseUrl}game/{$sessionInfo?.opponentUrlConnection}
      </div>
      <div
        class="copy_icon_container"
        use:tooltip={{
          content: "Copy link",
          position: "bottom",
          autoPosition: true,
          align: "center",
        }}
        on:click={copyOpponentLinkToClipboard}
      >
        {#if !textCopied}
          <div class="copy_icon">
            <FaRegCopy />
          </div>
          Copy
        {:else}
          <div class="copy_icon">
            <FaCheck />
          </div>
          Copied!
        {/if}
      </div>
    </div>
  </div>
{/if}

<style>
  :global(#incrementValueSlider, #limitValueSlider) {
    width: 80%;
    cursor: pointer;
    background: var(--bg-color);
  }

  :global(#incrementValueSlider .rangeNub, #limitValueSlider .rangeNub) {
    background: var(--regular-text-color);
  }

  :global(
      #incrementValueSlider.focus .rangeNub,
      #limitValueSlider.focus .rangeNub
    ) {
    background-color: var(--secondary-color) !important;
    border: 2px solid var(--secondary-text-color);
  }

  .main_container {
    box-sizing: border-box;
    background: var(--secondary-color);
    border-radius: 4px;
    padding: 1.5rem 1rem;
    margin: auto;
    margin-top: 6em;
    width: 100%;

    display: flex;
    flex-direction: column;
    align-items: center;

    box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.14),
      0 3px 1px -2px rgba(0, 0, 0, 0.2), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
  }

  .create_main_container {
    min-height: 470px;
    max-width: 420px;

    min-width: 300px;
  }

  @media (max-height: 800px) and (min-height: 600px) {
    .create_main_container {
      margin: 0;
      position: absolute;
      top: 50%;
      left: 50%;
      -ms-transform: translate(-50%, -50%);
      transform: translate(-50%, -50%);
    }
  }

  @media (max-height: 600px) {
    .create_main_container {
      margin-top: 0px;
    }
  }

  .link_main_container {
    min-height: 300px;
    max-width: 750px;
    min-width: 300px;
  }

  .link_text {
    margin: 1px 0;
    text-align: center;
  }

  .title {
    font-weight: 700;
    font-size: 1.7rem;
    padding-bottom: 1.5rem;
  }

  .color_chooser_container {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 20px;
    margin: 20px 0;
  }

  .color_button {
    width: 80px;
    height: 80px;
    border-radius: 4px;
    box-shadow: 0 2px 5px 0 rgba(0, 0, 0, 0.225);
    background: var(--bg-color);

    box-sizing: border-box;
    padding: 10px;
  }

  .big_color_button {
    width: 100px;
    height: 100px;
  }

  .color_button:hover {
    background: var(--secondary-color);
    box-shadow: 0 2px 5px 0 rgba(0, 0, 0, 0.225);
    cursor: pointer;
  }

  .color_active {
    background: var(--secondary-color);
    box-shadow: 0 2px 5px 0 rgba(0, 0, 0, 0.225);
  }

  .create_button {
    margin-top: 20px;
    height: 40px;
    padding: 0.5rem 4rem;
  }

  .link_container {
    margin-top: 2rem;
    display: flex;
    background: var(--button-background);
    border-radius: 4px;
    border: 3px solid var(--bg-color);
    width: 95%;
  }

  .link_text {
    overflow: hidden;
    padding: 0.5rem 1rem;
    width: 100%;
    border-right: 3px solid var(--bg-color);
    white-space: nowrap;
  }

  .copy_icon_container {
    width: 100px;
    display: flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.5rem;
    background: var(--secondary-color);
    justify-content: center;
  }
  .copy_icon_container:hover {
    background: var(--bg-color);
    cursor: pointer;
  }
  .copy_icon {
    width: 20px;
    height: 20px;
  }
</style>
