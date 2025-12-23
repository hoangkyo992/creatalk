import { fileURLToPath, URL } from "node:url";
import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import vueJsx from "@vitejs/plugin-vue-jsx";
import Pages from "vite-plugin-pages";
import Components from "unplugin-vue-components/vite";
import Layouts from "vite-plugin-vue-layouts";

// https://vitejs.dev/config/
export default ({ mode }) => {
  process.env = { ...process.env, ...loadEnv(mode, process.cwd()) };
  return defineConfig({
    plugins: [
      vue(),
      vueJsx({
        mergeProps: true
      }),
      Pages({
        exclude: ["**/components/*.vue"]
      }),
      Components({
        dirs: ["src/components", "src/pages/**/components"],
        // search for subdirectories
        deep: true
      }),
      Layouts()
    ],
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url))
      }
    },
    server: {
      port: 3008,
      proxy: {
        "/api": {
          target: process.env.VITE_API_BASE_URL,
          changeOrigin: true,
          secure: false,
          configure: (proxy) => {
            proxy.on("error", (err) => {
              console.log("proxy error", err);
            });
            proxy.on("proxyReq", (proxyReq, req) => {
              console.log("Sending Request to the target:", req.method, req.url);
            });
            proxy.on("proxyRes", (proxyRes, req) => {
              console.log("Received Response from the target:", proxyRes.statusCode, req.url);
            });
          }
        }
      }
    },
    build: {
      chunkSizeWarningLimit: 3200
    },
    base: `/`,
    css: {
      preprocessorOptions: {
        scss: {
          api: "modern-compiler" // or "modern"
        }
      }
    }
  });
};
