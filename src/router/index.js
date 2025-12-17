// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'

// Import all view components
import Home from '@/Views/Home.vue'
import About from '@/Views/About.vue'
import ContactUs from '@/Views/ContactUs.vue'

// Authentication
import Login from '@/Views/Authentication/Login.vue'
import SignUp from '@/Views/Authentication/SignUp.vue'
import ForgetPassword from '@/Views/Authentication/ForgetPassword.vue'

// User Account
import Profile from '@/Views/UserAccount/Profile.vue'
import Wishlist from '@/Views/UserAccount/Wishlist.vue'
import Reservations from '@/Views/UserAccount/Reservations.vue'

// Dashboard
import UsersManage from '@/Views/Dashboard/UsersManage.vue'
import TripsManage from '@/Views/Dashboard/TripsManage.vue'
import HotelsManage from '@/Views/Dashboard/HotelsMange.vue'
import CarsManage from '@/Views/Dashboard/CarsManage.vue'
import PaymentManage from '@/Views/Dashboard/PaymentManage.vue'

// AI
import AiCollectData from '@/Views/Ai/AiCollectData.vue'
import AiPlanResults from '@/Views/Ai/AiPlanResults.vue'

// Attractions
import AttractionsList from '@/Views/Attractions/AttractionsList.vue'
import AttractionDetails from '@/Views/Attractions/AttractionDetails.vue'
import AttractionReview from '@/Views/Attractions/AttractionReview.vue'
import AttractionCheckOut from '@/Views/Attractions/AttractionCheckOut.vue'
import AttractionConfirmation from '@/Views/Attractions/AttractionConfirmation.vue'

// Cars
import CarsList from '@/Views/Cars/Carslist.vue'
import CarDetails from '@/Views/Cars/CarDetails.vue'
import CarReview from '@/Views/Cars/CarReview.vue'
import CarCheckOut from '@/Views/Cars/CarCheckOut.vue'
import CarConfirmation from '@/Views/Cars/CarConfirmation.vue'

// Hotels
import HotelsList from '@/Views/Hotels/HotelsList.vue'
import HotelDetails from '@/Views/Hotels/HotelDetails.vue'
import HotelReview from '@/Views/Hotels/HotelReview.vue'
import HotelCheckOut from '@/Views/Hotels/HotelCheckOut.vue'
import HotelConfirmation from '@/Views/Hotels/HotelConfirmation.vue'

// Trips
import TripsList from '@/Views/Trips/TripsList.vue'
import TripDetails from '@/Views/Trips/TripDetails.vue'
import TripReview from '@/Views/Trips/TripReview.vue'
import TripCheckOut from '@/Views/Trips/TripCheckOut.vue'
import TripConfirmation from '@/Views/Trips/TripConfirmation.vue'

const routes = [
  // Home Routes
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/home',
    redirect: '/',
  },

  // Public Pages
  {
    path: '/about',
    name: 'About',
    component: About,
  },
  {
    path: '/contact-us',
    name: 'ContactUs',
    component: ContactUs,
  },

  // Authentication Routes
  {
    path: '/authentication/login',
    name: 'Login',
    component: Login,
  },
  {
    path: '/authentication/sign-up',
    name: 'SignUp',
    component: SignUp,
  },
  {
    path: '/authentication/forget-password',
    name: 'ForgetPassword',
    component: ForgetPassword,
  },

  // User Account Routes (Protected)
  {
    path: '/user-account/profile',
    name: 'Profile',
    component: Profile,
    meta: { requiresAuth: false },
  },
  {
    path: '/user-account/wishlist',
    name: 'Wishlist',
    component: Wishlist,
    meta: { requiresAuth: false },
  },
  {
    path: '/user-account/reservations',
    name: 'Reservations',
    component: Reservations,
    meta: { requiresAuth: false },
  },

  // Dashboard Routes (Protected)
  {
    path: '/dashboard/users-manage',
    name: 'UsersManage',
    component: UsersManage,
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard/trips-manage',
    name: 'TripsManage',
    component: TripsManage,
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard/hotels-manage',
    name: 'HotelsManage',
    component: HotelsManage,
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard/cars-manage',
    name: 'CarsManage',
    component: CarsManage,
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard/payment-manage',
    name: 'PaymentManage',
    component: PaymentManage,
    meta: { requiresAuth: false },
  },

  // AI Routes (Protected)
  {
    path: '/ai/collect-data',
    name: 'AiCollectData',
    component: AiCollectData,
    meta: { requiresAuth: false },
  },
  {
    path: '/ai/plan-results',
    name: 'AiPlanResults',
    component: AiPlanResults,
    meta: { requiresAuth: false },
  },

  // Attractions Routes
  {
    path: '/attractions/list',
    name: 'AttractionsList',
    component: AttractionsList,
  },
  {
    path: '/attractions/details/:id',
    name: 'AttractionDetails',
    component: AttractionDetails,
  },
  {
    path: '/attractions/review/:id',
    name: 'AttractionReview',
    component: AttractionReview,
    meta: { requiresAuth: false },
  },
  {
    path: '/attractions/checkout/:id',
    name: 'AttractionCheckOut',
    component: AttractionCheckOut,
    meta: { requiresAuth: false },
  },
  {
    path: '/attractions/confirmation/:id',
    name: 'AttractionConfirmation',
    component: AttractionConfirmation,
    meta: { requiresAuth: false },
  },

  // Cars Routes
  {
    path: '/cars/list',
    name: 'CarsList',
    component: CarsList,
  },
  {
    path: '/cars/details/:id',
    name: 'CarDetails',
    component: CarDetails,
  },
  {
    path: '/cars/review/:id',
    name: 'CarReview',
    component: CarReview,
    meta: { requiresAuth: false },
  },
  {
    path: '/cars/checkout/:id',
    name: 'CarCheckOut',
    component: CarCheckOut,
    meta: { requiresAuth: false },
  },
  {
    path: '/cars/confirmation/:id',
    name: 'CarConfirmation',
    component: CarConfirmation,
    meta: { requiresAuth: false },
  },

  // Hotels Routes
  {
    path: '/hotels/list',
    name: 'HotelsList',
    component: HotelsList,
  },
  {
    path: '/hotels/details/:id',
    name: 'HotelDetails',
    component: HotelDetails,
  },
  {
    path: '/hotels/review/:id',
    name: 'HotelReview',
    component: HotelReview,
    meta: { requiresAuth: false },
  },
  {
    path: '/hotels/checkout/:id',
    name: 'HotelCheckOut',
    component: HotelCheckOut,
    meta: { requiresAuth: false },
  },
  {
    path: '/hotels/confirmation/:id',
    name: 'HotelConfirmation',
    component: HotelConfirmation,
    meta: { requiresAuth: true },
  },

  // Trips Routes
  {
    path: '/trips/list',
    name: 'TripsList',
    component: TripsList,
  },
  {
    path: '/trips/details/:id',
    name: 'TripDetails',
    component: TripDetails,
  },
  {
    path: '/trips/review/:id',
    name: 'TripReview',
    component: TripReview,
    meta: { requiresAuth: false },
  },
  {
    path: '/trips/checkout/:id',
    name: 'TripCheckOut',
    component: TripCheckOut,
    meta: { requiresAuth: false },
  },
  {
    path: '/trips/confirmation/:id',
    name: 'TripConfirmation',
    component: TripConfirmation,
    meta: { requiresAuth: false },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

import { useAuthStore } from '@/stores/authStore'

// Navigation guard for protected routes
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)

  // Use auth store to check authentication status
  const authStore = useAuthStore()

  if (requiresAuth && !authStore.isAuthenticated) {
    // Redirect to login page if not authenticated
    next({
      name: 'Login',
      query: { redirect: to.fullPath }, // Save the intended destination
    })
  } else {
    next()
  }
})

export default router
