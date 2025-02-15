import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import JobListView from '../views/JobListView.vue';
import JobDetailsView from '../views/JobDetailsView.vue';
import LoginView from '../views/LoginView.vue';
import TableStorageView from '../views/TableStorageView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/speech',
      name: 'speech',
      component: JobListView,
      meta: { requiresAuth: true }
    },
    {
      path: '/speech/job/:id',
      name: 'speech-job-details',
      component: JobDetailsView,
      props: true,
      meta: { requiresAuth: true }
    },
    {
      path: '/tables',
      name: 'tables',
      component: TableStorageView,
      meta: { requiresAuth: true }
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/',
      redirect: '/speech'
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