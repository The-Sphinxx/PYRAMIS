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
import AttractionBooking from '@/Views/Attractions/AttractionBooking.vue'

// Cars
import CarsList from '@/Views/Cars/Carslist.vue'
import CarDetails from '@/Views/Cars/CarDetails.vue'
import CarBooking from '@/Views/Cars/CarBooking.vue'

// Hotels
import HotelsList from '@/Views/Hotels/HotelsList.vue'
import HotelDetails from '@/Views/Hotels/HotelDetails.vue'
import HotelBooking from '@/Views/Hotels/HotelBooking.vue'

// Trips
import TripsList from '@/Views/Trips/TripsList.vue'
import TripDetails from '@/Views/Trips/TripDetails.vue'
import TripBooking from '@/Views/Trips/TripBooking.vue'

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
    meta: { requiresAuth: true },
  },
  {
    path: '/user-account/wishlist',
    name: 'Wishlist',
    component: Wishlist,
    meta: { requiresAuth: true },
  },
  {
    path: '/user-account/reservations',
    name: 'Reservations',
    component: Reservations,
    meta: { requiresAuth: true },
  },

  // Dashboard Routes (Protected)
  {
    path: '/dashboard/users-manage',
    name: 'UsersManage',
    component: UsersManage,
    meta: { requiresAuth: true },
  },
  {
    path: '/dashboard/trips-manage',
    name: 'TripsManage',
    component: TripsManage,
    meta: { requiresAuth: true },
  },
  {
    path: '/dashboard/hotels-manage',
    name: 'HotelsManage',
    component: HotelsManage,
    meta: { requiresAuth: true },
  },
  {
    path: '/dashboard/cars-manage',
    name: 'CarsManage',
    component: CarsManage,
    meta: { requiresAuth: true },
  },
  {
    path: '/dashboard/payment-manage',
    name: 'PaymentManage',
    component: PaymentManage,
    meta: { requiresAuth: true },
  },

  // AI Routes (Protected)
  {
    path: '/ai/collect-data',
    name: 'AiCollectData',
    component: AiCollectData,
    meta: { requiresAuth: true },
  },
  {
    path: '/ai/plan-results',
    name: 'AiPlanResults',
    component: AiPlanResults,
    meta: { requiresAuth: true },
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
    path: '/attractions/booking/:id',
    name: 'AttractionBooking',
    component: AttractionBooking,
    meta: { requiresAuth: true },
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
    path: '/cars/booking/:id',
    name: 'CarBooking',
    component: CarBooking,
    meta: { requiresAuth: true },
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
    path: '/hotels/booking/:id',
    name: 'HotelBooking',
    component: HotelBooking,
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
    path: '/trips/booking/:id',
    name: 'TripBooking',
    component: TripBooking,
    meta: { requiresAuth: true },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Navigation guard for protected routes
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  
  // TODO: Replace this with your actual authentication check
  // Example: const isAuthenticated = store.getters.isAuthenticated
  const isAuthenticated = true // Placeholder - update with your auth logic
  
  if (requiresAuth && !isAuthenticated) {
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
