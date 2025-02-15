import { defineStore } from 'pinia';
import axios from 'axios';
import type { TranscriptionJob, TranscriptionJobRequest } from '../types';

export const useTranscriptionStore = defineStore('transcription', {
  state: () => ({
    jobs: [] as TranscriptionJob[],
    currentJob: null as TranscriptionJob | null,
    loading: false,
    error: null as string | null
  }),

  actions: {
    async fetchJobs() {
      try {
        this.loading = true;
        const response = await axios.get<TranscriptionJob[]>('/api/transcription');
        console.log('API Response:', response.data);
        this.jobs = response.data;
        console.log('Store jobs after update:', this.jobs);
        this.error = null;
      } catch (err) {
        this.error = 'Failed to fetch transcription jobs';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },

    async getJob(jobId: string) {
      try {
        this.loading = true;
        const response = await axios.get<TranscriptionJob>(`/api/transcription/${jobId}`);
        this.currentJob = response.data;
        this.error = null;
        return response.data;
      } catch (err) {
        this.error = 'Failed to fetch transcription job';
        console.error(err);
        return null;
      } finally {
        this.loading = false;
      }
    },

    async createJob(request: TranscriptionJobRequest) {
      try {
        this.loading = true;
        const response = await axios.post('/api/transcription', request);
        await this.fetchJobs();
        this.error = null;
        return response.data;
      } catch (err) {
        this.error = 'Failed to create transcription job';
        console.error(err);
        return null;
      } finally {
        this.loading = false;
      }
    },

    async deleteJob(jobId: string) {
      try {
        if (!jobId) {
          throw new Error('Job ID is required');
        }
        this.loading = true;
        // Construct URL in the same way as getJob
        const url = `/api/transcription/${encodeURIComponent(jobId)}`.replace(/\/+$/, '');
        console.log('Deleting job with URL:', url); // Debug log
        const response = await axios.delete(url);
        console.log('Delete response:', response); // Debug log
        await this.fetchJobs();
        this.error = null;
        return true;
      } catch (err) {
        this.error = 'Failed to delete transcription job';
        console.error('Delete error:', err);
        return false;
      } finally {
        this.loading = false;
      }
    },

    async deleteOutput(jobId: string) {
      try {
        this.loading = true;
        // Use transcriptionID as the parameter name
        const url = `/api/transcription/${encodeURIComponent(jobId)}/output`;
        console.log('Deleting output with URL:', url); // Debug log
        await axios.delete(url);
        await this.getJob(jobId);
        this.error = null;
        return true;
      } catch (err) {
        this.error = 'Failed to delete transcription output';
        console.error(err);
        return false;
      } finally {
        this.loading = false;
      }
    }
  }
});