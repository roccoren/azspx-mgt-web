import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5141',
        changeOrigin: true,
        rewrite: (path) => path.replace(/\/$/, '') // Remove trailing slash
      }
    }
  },
  build: {
    outDir: '../wwwroot',
    emptyOutDir: true
  }
});
