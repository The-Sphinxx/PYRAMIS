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
import ResetPassword from '@/Views/Authentication/ResetPassword.vue'
import VerifyEmail from '@/Views/Authentication/VerifyEmail.vue'

// User Account
import Profile from '@/Views/UserAccount/Profile.vue'
import Wishlist from '@/Views/UserAccount/Wishlist.vue'
import Reservations from '@/Views/UserAccount/Reservations.vue'

// Dashboard
import DashboardLayout from '@/Layouts/DashboardLayout.vue'
import Overview from '@/Views/Dashboard/Overview.vue'
import Analytics from '@/Views/Dashboard/Analytics.vue'
import Bookings from '@/Views/Dashboard/Bookings.vue'
import CustomerSupport from '@/Views/Dashboard/CustomerSupport.vue'
import UsersManage from '@/Views/Dashboard/UsersManage.vue'
import TripsManage from '@/Views/Dashboard/TripsManage.vue'
import HotelsManage from '@/Views/Dashboard/HotelsManage.vue'
import CarsManage from '@/Views/Dashboard/CarsManage.vue'
import AttractionsManage from '@/Views/Dashboard/AttractionsManage.vue'

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
import CarBooking from '@/Views/Cars/CarBooking.vue'


// Hotels
import HotelsList from '@/Views/Hotels/HotelsList.vue'
import HotelFilter from '@/Views/Hotels/hotelfilter.vue'
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
    path: '/auth/login',
    name: 'Login',
    component: Login,
  },
  {
    path: '/auth/sign-up',
    name: 'SignUp',
    component: SignUp,
  },
  {
    path: '/auth/forget-password',
    name: 'ForgetPassword',
    component: ForgetPassword,
  },
  {
    path: '/auth/reset-password',
    name: 'ResetPassword',
    component: ResetPassword,
  },
  {
    path: '/auth/verify-email',
    name: 'VerifyEmail',
    component: VerifyEmail,
    meta: { requiresAuth: true },
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

  // Dashboard Routes (Protected) - Using nested routing with DashboardLayout
  {
    path: '/dashboard',
    component: DashboardLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: 'overview',
        name: 'Overview',
        component: Overview,
      },
      {
        path: 'analytics',
        name: 'Analytics',
        component: Analytics,
      },
      {
        path: 'bookings',
        name: 'Bookings',
        component: Bookings,
      },
      {
        path: 'support',
        name: 'CustomerSupport',
        component: CustomerSupport,
      },
      {
        path: 'users-manage',
        name: 'UsersManage',
        component: UsersManage,
      },
      {
        path: 'trips-manage',
        name: 'TripsManage',
        component: TripsManage,
      },
      {
        path: 'hotels-manage',
        name: 'HotelsManage',
        component: HotelsManage,
      },
      {
        path: 'cars-manage',
        name: 'CarsManage',
        component: CarsManage,
      },
      {
        path: 'attractions-manage',
        name: 'AttractionsManage',
        component: AttractionsManage,
      },
    ],
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
    path: '/attractions/review/:id',
    name: 'AttractionReview',
    component: AttractionReview,
    meta: { requiresAuth: true },
  },
  {
    path: '/attractions/checkout/:id',
    name: 'AttractionCheckOut',
    component: AttractionCheckOut,
    meta: { requiresAuth: true },
  },
  {
    path: '/attractions/confirmation/:id',
    name: 'AttractionConfirmation',
    component: AttractionConfirmation,
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
  path: '/cars/booking',
  name: 'CarBooking',
  component: CarBooking,
    meta: { requiresAuth: true },
},
{
  path: '/cars/review/:id',
  name: 'CarReview',
  component: CarReview,
    meta: { requiresAuth: true },
},
{
  path: '/cars/checkout/:id',
  name: 'CarCheckOut',
  component: CarCheckOut,
    meta: { requiresAuth: true },
},
{
  path: '/cars/confirmation/:id',
  name: 'CarConfirmation',
  component: CarConfirmation,
    meta: { requiresAuth: true },
},


  // Hotels Routes
  {
    path: '/hotels/list',
    name: 'HotelsList',
    component: HotelsList,
  },
  {
    path: '/hotels/filter',
    name: 'HotelFilter',
    component: HotelFilter,
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
    meta: { requiresAuth: true },
  },
  {
    path: '/hotels/checkout/:id',
    name: 'HotelCheckOut',
    component: HotelCheckOut,
    meta: { requiresAuth: true },
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
    meta: { requiresAuth: true },
  },
  {
    path: '/trips/checkout/:id',
    name: 'TripCheckOut',
    component: TripCheckOut,
    meta: { requiresAuth: true },
  },
  {
    path: '/trips/confirmation/:id',
    name: 'TripConfirmation',
    component: TripConfirmation,
    meta: { requiresAuth: true },
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
