<template>
  <nav class="bg-gray-800 p-4">
    <div class="container mx-auto flex items-center justify-between">
      <router-link to="/" class="text-white text-xl font-bold">
        Azure Speech Manager
      </router-link>
      <div v-if="authStore.isAuthenticated" class="flex items-center space-x-4">
        <span class="text-gray-300">{{ authStore.username }}</span>
        <button
          @click="authStore.logout"
          class="text-white hover:text-gray-300"
        >
          Logout
        </button>
      </div>
    </div>
  </nav>

  <main class="container mx-auto p-4">
    <div v-if="transcriptionStore.error" 
         class="mb-4 bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
      {{ transcriptionStore.error }}
    </div>
    <router-view />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useTranscriptionStore } from './stores/transcription';
import { useAuthStore } from './stores/auth';
import { useRouter } from 'vue-router';

const router = useRouter();
const authStore = useAuthStore();
const transcriptionStore = useTranscriptionStore();

onMounted(() => {
  if (authStore.isAuthenticated) {
    transcriptionStore.fetchJobs();
  } else {
    router.push('/login');
  }
});
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

body {
  font-family: 'Inter', sans-serif;
  @apply bg-gray-100;
}
</style>
