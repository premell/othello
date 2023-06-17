// rollup.config.js
import svelte from 'rollup-plugin-svelte';
import autoPreprocess from 'svelte-preprocess'
import typescript from '@rollup/plugin-typescript';

export default {
  plugins: [
    svelte({
      /**
       * Auto preprocess supported languages with
       * ''/'external src files' support
       **/
      preprocess: autoPreprocess({ /* options available https://github.com/sveltejs/svelte-preprocess/blob/master/docs/preprocessing.md */ })
    }),
    /**
     * In case you want to use TypeScript outside of Svelte files, too
     */
    typescript()
  ]
}
