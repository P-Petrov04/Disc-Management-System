import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    open: "/",
    historyApiFallback: true,
    proxy: {
      "/api": {
        target: "https://localhost:7254/",
        changeOrigin: true,
        secure: false
      }
    }
  }
});