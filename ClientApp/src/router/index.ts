import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import JobListView from '../views/JobListView.vue';
import JobDetailsView from '../views/JobDetailsView.vue';
import LoginView from '../views/LoginView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'jobs',
      component: JobListView,
      meta: { requiresAuth: true }
    },
    {
      path: '/job/:id',
      name: 'job-details',
      component: JobDetailsView,
      props: true,
      meta: { requiresAuth: true }
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    }
  ]
});

// Navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);

  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login');
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/');
  } else {
    next();
  }
});

export default router;