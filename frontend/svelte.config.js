import adapter from "@sveltejs/adapter-node";
import sveltePreprocess from "svelte-preprocess";
import azure from "svelte-adapter-azure-swa";

import { vitePreprocess } from "@sveltejs/kit/vite";
// import preprocess from "svelte-preprocess";

/** @type {import('@sveltejs/kit').Config} */
const config = {
  // Consult https://kit.svelte.dev/docs/integrations#preprocessors
  // for more information about preprocessors
  preprocess: vitePreprocess(),
  // preprocess: sveltePreprocess({ sourceMap: !process.env.ROLLUP_WATCH }),

  kit: {
    // adapter-auto only supports some environments, see https://kit.svelte.dev/docs/adapter-auto for a list.
    // If your environment is not supported or you settled on a specific environment, switch out the adapter.
    // See https://kit.svelte.dev/docs/adapters for more information about adapters.
    adapter: adapter({
			// default options are shown
			out: 'build',
			precompress: false,
			env: {
				host: '0.0.0.0',
				port: process.env.PORT || 5173
			}
		})
  },
};

// customStaticWebAppConfig: {
//   navigationFallback: {
//     rewrite: "index.html",
//     exclude: ["/images/*.{png,jpg,gif}", "/css/*"],
//   },
// },

export default config;
