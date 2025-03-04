import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    open: "/",
    proxy: {
      "/api": {
        target: "https://localhost:7254/login",
        changeOrigin: true,
        secure: false
      }
    }
  }
});
