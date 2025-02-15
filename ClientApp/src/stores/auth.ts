import { defineStore } from 'pinia';
import axios from 'axios';
import router from '../router';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  token: string;
  username: string;
  expiration: string;
}

interface AuthState {
  token: string | null;
  username: string | null;
  loading: boolean;
  error: string | null;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: localStorage.getItem('token'),
    username: localStorage.getItem('username'),
    loading: false,
    error: null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    getAuthHeader: (state) => state.token ? `Bearer ${state.token}` : undefined
  },

  actions: {
    async login(credentials: LoginRequest) {
      try {
        this.loading = true;
        this.error = null;

        const response = await axios.post<LoginResponse>(
          '/api/auth/login',
          credentials,
          {
            headers: {
              'Content-Type': 'application/json'
            }
          }
        );

        if (response.data && response.data.token) {
          this.token = response.data.token;
          this.username = response.data.username;
          
          localStorage.setItem('token', response.data.token);
          localStorage.setItem('username', response.data.username);
          
          router.push('/');
        } else {
          this.error = 'Invalid response from server';
          console.error('Invalid response:', response);
        }
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Invalid username or password';
        console.error('Login error:', error);
        this.token = null;
        this.username = null;
        localStorage.removeItem('token');
        localStorage.removeItem('username');
      } finally {
        this.loading = false;
      }
    },

    logout() {
      this.token = null;
      this.username = null;
      this.error = null;
      localStorage.removeItem('token');
      localStorage.removeItem('username');
      router.push('/login');
    }
  }
});

// Create axios interceptor to add auth header
axios.interceptors.request.use((config) => {
  const authStore = useAuthStore();
  const token = authStore.token;
  
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  return config;
});

// Handle 401 responses
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const authStore = useAuthStore();
      authStore.logout();
    }
    return Promise.reject(error);
  }
);