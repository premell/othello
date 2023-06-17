import adapter from "@sveltejs/adapter-node";

import { vitePreprocess } from "@sveltejs/kit/vite";

const config = {
  preprocess: vitePreprocess(),

  kit: {
    adapter: adapter({
			out: 'build',
			precompress: false,
			env: {
				host: '0.0.0.0',
				port: process.env.PORT || 5173
			}
		})
  },
};

export default config;
