<script>
  import "$lib/styles.css";
  import Navbar from "$lib/components/Navbar.svelte";
  import "@fontsource/noto-sans";
</script>

<div class="app">
  <Navbar />
  <slot />
</div>

<style>
  .app {
    min-height: 100vh;
    background: var(--bg-color);
  }
</style>
