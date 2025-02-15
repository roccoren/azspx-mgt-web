<template>
  <div>
    <div class="mb-6">
      <router-link to="/" class="text-blue-600 hover:text-blue-800">
        &larr; Back to Jobs
      </router-link>
    </div>

    <div v-if="store.loading" class="text-center py-8">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <div v-else-if="store.error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
      {{ store.error }}
    </div>

    <div v-else-if="job" class="bg-white rounded-lg shadow p-6">
      <div class="flex justify-between items-start mb-6">
        <div>
          <h1 class="text-2xl font-bold">{{ job.displayName }}</h1>
          <p class="text-gray-500">ID: {{ job.id }}</p>
        </div>
        <span
          :class="{
            'px-3 py-1 rounded-full text-sm font-medium': true,
            'bg-yellow-100 text-yellow-800': job.status === 'Running',
            'bg-green-100 text-green-800': job.status === 'Succeeded',
            'bg-red-100 text-red-800': job.status === 'Failed'
          }"
        >
          {{ job.status }}
        </span>
      </div>

      <div class="grid gap-4">
        <div class="border-t pt-4">
          <h2 class="font-semibold text-lg mb-2">Details</h2>
          <dl class="grid grid-cols-2 gap-4">
            <div>
              <dt class="text-gray-600">Created</dt>
              <dd>{{ new Date(job.createdDateTime).toLocaleString() }}</dd>
            </div>
            <div v-if="job.lastActionDateTime">
              <dt class="text-gray-600">Last Updated</dt>
              <dd>{{ new Date(job.lastActionDateTime).toLocaleString() }}</dd>
            </div>
            <div>
              <dt class="text-gray-600">Locale</dt>
              <dd>{{ job.locale }}</dd>
            </div>
            <div v-if="job.model">
              <dt class="text-gray-600">Model</dt>
              <dd>{{ job.model.self }}</dd>
            </div>
          </dl>
        </div>

        <div class="border-t pt-4">
          <h2 class="font-semibold text-lg mb-2">Properties</h2>
          <dl class="grid grid-cols-2 gap-4">
            <div>
              <dt class="text-gray-600">Word Level Timestamps</dt>
              <dd>{{ job.properties.wordLevelTimestampsEnabled ? 'Enabled' : 'Disabled' }}</dd>
            </div>
            <div>
              <dt class="text-gray-600">Display Form Word Level Timestamps</dt>
              <dd>{{ job.properties.displayFormWordLevelTimestampsEnabled ? 'Enabled' : 'Disabled' }}</dd>
            </div>
            <div v-if="job.properties.diarization">
              <dt class="text-gray-600">Speaker Diarization</dt>
              <dd>{{ job.properties.diarization.enabled ? 'Enabled' : 'Disabled' }}</dd>
            </div>
          </dl>
        </div>

        <div v-if="job.error" class="border-t pt-4">
          <h2 class="font-semibold text-lg mb-2">Error</h2>
          <p class="text-red-600">{{ job.error }}</p>
        </div>

        <div v-if="job.status === 'Succeeded'" class="border-t pt-4">
          <h2 class="font-semibold text-lg mb-2">Results</h2>
          <div v-if="job.links?.files" class="space-y-2">
            <div class="flex items-center space-x-2">
              <a
                :href="job.links.files"
                target="_blank"
                class="text-blue-600 hover:text-blue-800"
              >
                Download Files
              </a>
              <button
                @click="deleteOutput(job.id)"
                class="text-red-600 hover:text-red-800 text-sm"
              >
                Delete
              </button>
            </div>
          </div>
        </div>

        <div class="border-t pt-4">
          <button
            @click="deleteJob(job.id)"
            class="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
          >
            Delete Job
          </button>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-8 text-gray-500">
      Job not found.
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useTranscriptionStore } from '../stores/transcription';
import type { TranscriptionJob } from '../types';

const props = defineProps<{
  id: string;
}>();

const router = useRouter();
const store = useTranscriptionStore();

const job = computed(() => store.currentJob);

onMounted(async () => {
  await store.getJob(props.id);
});

async function deleteJob(jobId: string) {
  if (confirm('Are you sure you want to delete this job?')) {
    const success = await store.deleteJob(jobId);
    if (success) {
      router.push('/');
    }
  }
}

async function deleteOutput(jobId: string) {
  if (confirm('Are you sure you want to delete the transcription output?')) {
    await store.deleteOutput(jobId);
  }
}
</script>