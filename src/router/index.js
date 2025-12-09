import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';

// Authentication Views - استخدم dynamic imports
const Login = () => import('@/Views/Authentication/Login.vue');
const Signup = () => import('@/Views/Authentication/SignUp.vue');
const ForgotPassword = () => import('@/Views/Authentication/ForgetPassword.vue');

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('@/Views/Home.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/authentication/login',
    name: 'Login',
    component: Login,
    meta: { guest: true }
  },
  {
    path: '/login',
    redirect: '/authentication/login'
  },
  {
    path: '/authentication/signup',
    name: 'Signup',
    component: Signup,
    meta: { guest: true }
  },
  {
    path: '/signup',
    redirect: '/authentication/signup'
  },
  {
    path: '/authentication/forgot-password',
    name: 'ForgotPassword',
    component: ForgotPassword,
    meta: { guest: true }
  },
  {
    path: '/forgot-password',
    redirect: '/authentication/forgot-password'
  },
  {
    path: '/profile',
    name: 'Profile',
    component: () => import('@/Views/UserAccount/Profile.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/trips',
    name: 'Trips',
    component: () => import('@/Views/Trips/TripsList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/hotels',
    name: 'Hotels',
    component: () => import('@/Views/Hotels/HotelsList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/cars',
    name: 'Cars',
    component: () => import('@/Views/Cars/Carslist.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/attractions',
    name: 'Attractions',
    component: () => import('@/Views/Attractions/AttractionsList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/ai-planner',
    name: 'AiPlanner',
    component: () => import('@/Views/Ai/AiCollectData.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/Views/NotFound.vue')
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  }
});

// Navigation Guards
router.beforeEach((to, from, next) => {

    next({
      name: 'Login',
      query: { redirect: to.fullPath }
    });
  } else if (isGuest && authStore.isAuthenticated) {
    // صفحات الضيوف والمستخدم مسجل دخول بالفعل
    next({ name: 'Home' });
  } else {
    next();
  }


export default router;