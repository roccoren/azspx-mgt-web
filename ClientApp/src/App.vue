<template>
  <nav class="bg-gray-800 p-4">
    <div class="container mx-auto">
      <div class="flex items-center justify-between mb-4">
        <router-link to="/" class="text-white text-xl font-bold">
          Azure Management Portal
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
      <div v-if="authStore.isAuthenticated" class="flex space-x-4">
        <router-link 
          to="/speech" 
          class="px-4 py-2 rounded-t-lg"
          :class="[$route.path.startsWith('/speech') ? 'bg-white text-gray-800' : 'text-gray-300 hover:text-white']"
        >
          Speech Services
        </router-link>
        <router-link 
          to="/tables" 
          class="px-4 py-2 rounded-t-lg"
          :class="[$route.path.startsWith('/tables') ? 'bg-white text-gray-800' : 'text-gray-300 hover:text-white']"
        >
          Table Storage
        </router-link>
      </div>
    </div>
  </nav>

  <main class="container mx-auto p-4">
    <!-- Transcription Service Errors -->
    <div v-if="transcriptionStore.error" 
         class="mb-4 bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
      {{ transcriptionStore.error }}
    </div>
    
    <!-- Table Storage Errors -->
    <div v-if="tableStorageStore.error" 
         class="mb-4 bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
      {{ tableStorageStore.error }}
    </div>

    <!-- Loading Indicator -->
    <div v-if="transcriptionStore.loading || tableStorageStore.loading" 
         class="mb-4 bg-blue-100 border border-blue-400 text-blue-700 px-4 py-3 rounded">
      Loading...
    </div>

    <router-view />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useTranscriptionStore } from './stores/transcription';
import { useTableStorageStore } from './stores/tableStorage';
import { useAuthStore } from './stores/auth';
import { useRouter, useRoute } from 'vue-router';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const transcriptionStore = useTranscriptionStore();
const tableStorageStore = useTableStorageStore();

onMounted(() => {
  if (authStore.isAuthenticated) {
    if (route.path.startsWith('/speech')) {
      transcriptionStore.fetchJobs();
    } else if (route.path.startsWith('/tables')) {
      tableStorageStore.fetchTables();
    }
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
