<script lang="ts">
  import { onMount } from "svelte";
  import { tooltip } from "@svelte-plugins/tooltips";

  import FaGithub from "svelte-icons/fa/FaGithub.svelte";
  import FaMoon from "svelte-icons/fa/FaMoon.svelte";
  import FaSun from "svelte-icons/fa/FaSun.svelte";
  import FaVolumeMute from "svelte-icons/fa/FaVolumeMute.svelte";
  import FaVolumeUp from "svelte-icons/fa/FaVolumeUp.svelte";
  import FaLinkedinIn from "svelte-icons/fa/FaLinkedinIn.svelte";
  import { soundMuted } from "$lib/stores/gameStore";

  let currentTheme = "";
  onMount(() => {
    currentTheme = document.documentElement.dataset.theme ?? "";
    const userPrefersDarkMode = window.matchMedia(
      "(prefers-color-scheme: dark)"
    ).matches;

    const hasUserSetThemeManually =
      document.documentElement.dataset.theme != undefined;

    if (!hasUserSetThemeManually) {
      setThemeTemporary(userPrefersDarkMode ? "dark" : "light");
    }
  });

  function setThemeTemporary(theme: string) {
    document.documentElement.dataset.theme = theme;
    currentTheme = theme;
  }

  function setThemePermentantly(theme: string) {
    document.documentElement.dataset.theme = theme;
    document.cookie = `siteTheme=${theme};max-age=31536000;path="/"`;
    currentTheme = theme;
  }
</script>

<div class="navbar">
  <div class="left">
    <div class="title">Othello</div>
    <div class="subtitle" on:click={() => (location.href = "/")}>New game</div>
  </div>
  {#if currentTheme !== ""}
    <div class="right">
      <div class="subtitle">
        <a
          href="https://github.com/premell/othello#how-to-play--video_game"
          target="_blank"
          rel="noopener noreferrer"
        >
          How to play
        </a>
      </div>

      <div
        class="icon github_icon"
        use:tooltip={{
          content: "My Github",
          position: "bottom",
          autoPosition: true,
          align: "center",
        }}
      >
        <a
          href="https://github.com/premell/othello"
          target="_blank"
          rel="noopener noreferrer"
        >
          <FaGithub />
        </a>
      </div>
      <div
        class="icon linkedln_icon"
        use:tooltip={{
          content: "My Linkedln",
          position: "bottom",
          autoPosition: true,
          align: "center",
        }}
      >
        <a
          href="https://www.linkedin.com/in/elmer-lingest%C3%A5l-3571021a8/"
          target="_blank"
          rel="noopener noreferrer"
        >
          <FaLinkedinIn />
        </a>
      </div>
      {#if currentTheme == "dark"}
        <div
          class="icon sun_moon"
          on:click={() => setThemePermentantly("light")}
          use:tooltip={{
            content: "Light theme",
            position: "bottom",
            autoPosition: true,
            align: "center",
          }}
        >
          <FaMoon />
        </div>
      {:else}
        <div
          class="icon sun_icon"
          on:click={() => setThemePermentantly("dark")}
          use:tooltip={{
            content: "Dark theme",
            position: "bottom",
            autoPosition: true,
            align: "center",
          }}
        >
          <FaSun />
        </div>
      {/if}

      {#if $soundMuted}
        <div
          class="icon mute_icon"
          on:click={() => soundMuted.set(false)}
          use:tooltip={{
            content: "Unmute",
            position: "bottom",
            autoPosition: true,
            align: "center",
          }}
        >
          <FaVolumeMute />
        </div>
      {:else}
        <div
          class="icon unmute_icon"
          on:click={() => soundMuted.set(true)}
          use:tooltip={{
            content: "Mute",
            position: "bottom",
            autoPosition: true,
            align: "center",
          }}
        >
          <FaVolumeUp />
        </div>
      {/if}
    </div>
  {/if}
</div>

<style>
  .icon {
    cursor: pointer;
    height: 20px;
    width: 20px;
    color: var(--navbar-text) !important;
  }

  .navbar {
    height: 60px;
    display: flex;
    max-width: 1800px;
    justify-content: space-between;
  }

  .left {
    align-items: center;
    display: flex;
    gap: 5px;
    margin-left: 14px;
  }

  .right {
    align-items: center;
    display: flex;
    gap: 5px;
    margin-right: 14px;
  }

  @media (min-width: 510px) {
    .navbar {
      margin: auto;
      padding: 0 20px;
    }

    .left {
      gap: 20px;
      margin-left: 14px;
    }

    .right {
      margin-right: 14px;
      gap: 20px;
    }
  }

  :global(.tooltip) {
    pointer-events: none;
  }
</style>
