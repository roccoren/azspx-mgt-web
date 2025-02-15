<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Transcription Jobs</h1>
      <button
        @click="showNewJobModal = true"
        class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
      >
        New Job
      </button>
    </div>

    <div v-if="store.loading" class="text-center py-8">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <div v-else-if="store.error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
      {{ store.error }}
    </div>

    <div v-else-if="store.jobs.length === 0" class="text-center py-8 text-gray-500">
      No transcription jobs found.
    </div>

    <div v-else class="grid gap-4">
      <div
        v-for="job in store.jobs"
        :key="job.id"
        class="bg-white p-4 rounded-lg shadow hover:shadow-md transition-shadow"
      >
        <div class="flex justify-between items-start">
          <div>
            <h3 class="font-semibold text-lg">{{ job.displayName }}</h3>
            <p class="text-sm text-gray-500">Created: {{ new Date(job.createdDateTime).toLocaleString() }}</p>
          </div>
          <div class="flex items-center space-x-2">
            <span
              :class="{
                'px-2 py-1 rounded text-sm': true,
                'bg-yellow-100 text-yellow-800': job.status === 'Running',
                'bg-green-100 text-green-800': job.status === 'Succeeded',
                'bg-red-100 text-red-800': job.status === 'Failed'
              }"
            >
              {{ job.status }}
            </span>
          </div>
        </div>

        <div class="mt-4 flex justify-between items-center">
          <router-link
            v-if="job.id"
            :to="{ name: 'speech-job-details', params: { id: job.id }}"
            class="text-blue-600 hover:text-blue-800"
          >
            View Details
          </router-link>
          <span v-else class="text-gray-400">
            Details Unavailable
          </span>
          <button
            @click="deleteJob(job.id)"
            class="text-red-600 hover:text-red-800"
          >
            Delete
          </button>
        </div>
      </div>
    </div>

    <!-- New Job Modal -->
    <div v-if="showNewJobModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
      <div class="bg-white p-6 rounded-lg max-w-md w-full">
        <h2 class="text-xl font-bold mb-4">New Transcription Job</h2>
        <form @submit.prevent="createJob">
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Name</label>
            <input
              v-model="newJob.displayName"
              type="text"
              required
              class="w-full p-2 border rounded"
            />
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Audio URL</label>
            <input
              v-model="newJob.audioUrl"
              type="url"
              required
              class="w-full p-2 border rounded"
            />
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Locale</label>
            <input
              v-model="newJob.locale"
              type="text"
              placeholder="en-US"
              class="w-full p-2 border rounded"
            />
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Options</label>
            <div class="space-y-2">
              <label class="flex items-center">
                <input
                  v-model="newJob.enableDiarization"
                  type="checkbox"
                  class="mr-2"
                />
                Enable Speaker Diarization
              </label>
              <label class="flex items-center">
                <input
                  v-model="newJob.enableWordLevelTimestamps"
                  type="checkbox"
                  class="mr-2"
                />
                Enable Word-Level Timestamps
              </label>
              <label class="flex items-center">
                <input
                  v-model="newJob.enableDisplayFormWordLevelTimestamps"
                  type="checkbox"
                  class="mr-2"
                />
                Enable Display Form Word-Level Timestamps
              </label>
            </div>
          </div>
          <div class="flex justify-end space-x-2">
            <button
              type="button"
              @click="showNewJobModal = false"
              class="px-4 py-2 text-gray-600 hover:text-gray-800"
            >
              Cancel
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Create
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useTranscriptionStore } from '../stores/transcription';
import type { TranscriptionJobRequest } from '../types';

const store = useTranscriptionStore();
const showNewJobModal = ref(false);
const newJob = ref<TranscriptionJobRequest>({
  displayName: '',
  audioUrl: '',
  locale: 'en-US',
  enableDiarization: false,
  enableWordLevelTimestamps: false,
  enableDisplayFormWordLevelTimestamps: false
});

async function createJob() {
  const result = await store.createJob(newJob.value);
  if (result) {
    showNewJobModal.value = false;
    newJob.value = {
      displayName: '',
      audioUrl: '',
      locale: 'en-US',
      enableDiarization: false,
      enableWordLevelTimestamps: false,
      enableDisplayFormWordLevelTimestamps: false
    };
  }
}

async function deleteJob(jobId: string) {
  if (confirm('Are you sure you want to delete this job?')) {
    await store.deleteJob(jobId);
  }
}

import { onMounted } from 'vue';

// Fetch jobs when component is mounted
onMounted(() => {
  store.fetchJobs();
});
</script>