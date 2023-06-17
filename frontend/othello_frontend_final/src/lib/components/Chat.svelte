<script lang="ts">
  import { currentGame, sessionInfo } from "$lib/stores/gameStore";
  import { sendMessage } from "$lib/signalr";
  import { get } from "svelte/store";
  import type { ChatMessage } from "$lib/models/Models";
  import { COLOR_VALUES } from "$lib/enums/enums";
  import Board from "./Board.svelte";

  let value = "";

  function handleKeydown(e: any) {
    if (e.keyCode == 13) {
      send();
    }
  }

  let messageContainer: any;
  function send() {
    if (value.trim() === "") return;

    sendMessage(value);
    value = "";
  }

  $: if ($sessionInfo?.messages && messageContainer) {
    scrollToBottom(messageContainer);
  }

  const scrollToBottom = async (node: any) => {
    setTimeout(() => {
      node.scroll({ top: node.scrollHeight, behavior: "smooth" });
    }, 0);
  };
</script>

<!-- class:userMessage={message.senderColor === $sessionInfo.userColor} -->
<!-- class:serverMessage={message.senderColor === COLOR_VALUES.NONE} -->
<div class="container">
  <div class="message_container" bind:this={messageContainer}>
    {#each $sessionInfo.messages as message}
      {#if message.senderColor === COLOR_VALUES.NONE}
        <div class="message server_message">
          {message.content}
        </div>
      {:else}
        <div class="message">
          <b
            >{message.senderColor === COLOR_VALUES.BLACK
              ? "Black"
              : "White"}:</b
          >
          {message.content}
        </div>
      {/if}
    {/each}
  </div>
  <div class="chat_container">
    <input
      type="text"
      placeholder="Write a message"
      class="chat_input"
      bind:value
      on:keydown={handleKeydown}
    />
    <!-- <button class="chat_button" on:click={send}>Skicka</button> -->
  </div>
</div>

<style>
  .container {
    position: relative;
    background: var(--secondary-color);
    height: 100%;
    min-height: 200px;

    box-shadow: 0 2px 2px 0 rgba(0, 0, 0, 0.14),
      0 3px 1px -2px rgba(0, 0, 0, 0.2), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 5px;
    overflow: auto;
  }

  .message {
    width: 100%;
    font-size: 0.9em;
    padding: 0.25em 10px 0.25em 10px;
    box-sizing: border-box;
  }

  .message_container {
    height: calc(100% - 30px);
    /* display: flex; */
    /* flex-direction: column-reverse; */
    overflow-y: auto;
    padding: 15px 0 5px 0;
    box-sizing: border-box;
  }

  .chat_container {
    width: 100%;
    height: 30px;
    position: absolute;
    bottom: 0px;
    display: flex;
  }

  .chat_input {
    width: 100%;
    box-sizing: border-box;
    flex: 0 0 auto;
    border: 0;
    border-top-color: currentcolor;
    border-top-style: none;
    border-top-width: 0px;
    border-radius: 0;
    padding: 3px 20px 3px 4px;

    font: inherit;
    color: var(--regular-text-color);
    outline-color: #1b78d0;
  }

  .chat_input:focus {
    outline: none;
    box-shadow: 0 0 10px var(--bg-color);
  }

  .server_message {
    font-family: "Roboto", sans-serif;
    text-align: center;
    font-weight: 900;
  }
</style>
