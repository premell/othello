// rollup.config.js
import svelte from 'rollup-plugin-svelte';
import autoPreprocess from 'svelte-preprocess'
import typescript from '@rollup/plugin-typescript';

export default {
  plugins: [
    svelte({
      preprocess: autoPreprocess({})
    }),
    typescript()
  ]
}
