<script>
  import "$lib/styles.css";
  import Footer from "$lib/components/Footer.svelte";
  import Navbar from "$lib/components/Navbar.svelte";
  import "@fontsource/noto-sans";
  import { onMount } from "svelte";

  import {setBoardHeightProperty} from "$lib/utils"
</script>

<div class="app">
  <Navbar />
  <slot />
</div>
  <!-- <Footer /> -->

<style>
  .app {
    min-height: 100vh;
    background: var(--bg-color);
  }


</style>
