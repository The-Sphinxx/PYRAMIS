// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router'

// Import all view components
// Import all view components - converted to lazy loading
// Public Pages
const Home = () => import('@/Views/Home.vue')
const About = () => import('@/Views/About.vue')
const ContactUs = () => import('@/Views/ContactUs.vue')

// Authentication
const Login = () => import('@/Views/Authentication/Login.vue')
const SignUp = () => import('@/Views/Authentication/SignUp.vue')
const ForgetPassword = () => import('@/Views/Authentication/ForgetPassword.vue')
const ResetPassword = () => import('@/Views/Authentication/ResetPassword.vue')
const VerifyEmail = () => import('@/Views/Authentication/VerifyEmail.vue')

// User Account
const Profile = () => import('@/Views/UserAccount/Profile.vue')
const Wishlist = () => import('@/Views/UserAccount/Wishlist.vue')
const Reservations = () => import('@/Views/UserAccount/Reservations.vue')

// Dashboard
const DashboardLayout = () => import('@/Layouts/DashboardLayout.vue')
const MainLayout = () => import('@/Layouts/MainLayout.vue')
const Overview = () => import('@/Views/Dashboard/Overview.vue')
const Analytics = () => import('@/Views/Dashboard/Analytics.vue')
const Bookings = () => import('@/Views/Dashboard/Bookings.vue')
const CustomerSupport = () => import('@/Views/Dashboard/CustomerSupport.vue')
const UsersManage = () => import('@/Views/Dashboard/UsersManage.vue')
const TripsManage = () => import('@/Views/Dashboard/TripsManage.vue')
const HotelsManage = () => import('@/Views/Dashboard/HotelsManage.vue')
const CarsManage = () => import('@/Views/Dashboard/CarsManage.vue')

const AttractionsManage = () => import('@/Views/Dashboard/AttractionsManage.vue')
const AdminsManage = () => import('@/Views/Dashboard/AdminsManage.vue')
const AdminSettings = () => import('@/Views/Dashboard/AdminSettings.vue')

// AI
const AiCollectData = () => import('@/Views/Ai/AiCollectData.vue')
const AiPlanResults = () => import('@/Views/Ai/AiPlanResults.vue')

// Attractions
const AttractionsList = () => import('@/Views/Attractions/AttractionsList.vue')
const AttractionDetails = () => import('@/Views/Attractions/AttractionDetails.vue')
const AttractionReview = () => import('@/Views/Attractions/AttractionReview.vue')
const AttractionCheckOut = () => import('@/Views/Attractions/AttractionCheckOut.vue')
const AttractionConfirmation = () => import('@/Views/Attractions/AttractionConfirmation.vue')

// Cars
const CarsList = () => import('@/Views/Cars/Carslist.vue')
const CarDetails = () => import('@/Views/Cars/CarDetails.vue')
const CarReview = () => import('@/Views/Cars/CarReview.vue')
const CarCheckOut = () => import('@/Views/Cars/CarCheckOut.vue')
const CarConfirmation = () => import('@/Views/Cars/CarConfirmation.vue')
const CarBooking = () => import('@/Views/Cars/CarBooking.vue')

// Hotels
const HotelsList = () => import('@/Views/Hotels/HotelsList.vue')
const HotelFilter = () => import('@/Views/Hotels/hotelfilter.vue')
const HotelDetails = () => import('@/Views/Hotels/HotelDetails.vue')
const HotelReview = () => import('@/Views/Hotels/HotelReview.vue')
const HotelCheckOut = () => import('@/Views/Hotels/HotelCheckOut.vue')
const HotelConfirmation = () => import('@/Views/Hotels/HotelConfirmation.vue')

// Trips
const TripsList = () => import('@/Views/Trips/TripsList.vue')
const TripDetails = () => import('@/Views/Trips/TripDetails.vue')
const TripReview = () => import('@/Views/Trips/TripReview.vue')
const TripCheckOut = () => import('@/Views/Trips/TripCheckOut.vue')
const TripConfirmation = () => import('@/Views/Trips/TripConfirmation.vue')

const routes = [
  // Admin alias route for dashboard
  {
    path: '/admin/dashboard',
    redirect: '/dashboard/overview'
  },
  // Home Routes (No Layout)
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/home',
    redirect: '/',
  },

  // Authentication Routes (No Layout)
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

  // Main Layout Routes (Navbar & Footer included)
  {
    path: '/',
    component: MainLayout,
    children: [
      // Public Pages
      {
        path: 'about',
        name: 'About',
        component: About,
      },
      {
        path: 'contact-us',
        name: 'ContactUs',
        component: ContactUs,
      },

      // User Account Routes (Protected)
      {
        path: 'user-account/profile',
        name: 'Profile',
        component: Profile,
        meta: { requiresAuth: false },
      },
      {
        path: 'user-account/wishlist',
        name: 'Wishlist',
        component: Wishlist,
        meta: { requiresAuth: false },
      },
      {
        path: 'user-account/reservations',
        name: 'Reservations',
        component: Reservations,
        meta: { requiresAuth: false },
      },

      // AI Routes (Protected)
      {
        path: 'ai/collect-data',
        name: 'AiCollectData',
        component: AiCollectData,
        meta: { requiresAuth: false },
      },
      {
        path: 'ai/plan-results',
        name: 'AiPlanResults',
        component: AiPlanResults,
        meta: { requiresAuth: false },
      },

      // Attractions Routes
      {
        path: 'attractions/list',
        name: 'AttractionsList',
        component: AttractionsList,
      },
      {
        path: 'attractions/details/:id',
        name: 'AttractionDetails',
        component: AttractionDetails,
      },
      {
        path: 'attractions/review/:id',
        name: 'AttractionReview',
        component: AttractionReview,
        meta: { requiresAuth: false },
      },
      {
        path: 'attractions/checkout/:id',
        name: 'AttractionCheckOut',
        component: AttractionCheckOut,
        meta: { requiresAuth: false },
      },
      {
        path: 'attractions/confirmation/:id',
        name: 'AttractionConfirmation',
        component: AttractionConfirmation,
        meta: { requiresAuth: false },
      },

      // Cars Routes
      {
        path: 'cars/list',
        name: 'CarsList',
        component: CarsList,
      },
      {
        path: 'cars/details/:id',
        name: 'CarDetails',
        component: CarDetails,
      },
      {
        path: 'cars/booking',
        name: 'CarBooking',
        component: CarBooking,
        meta: { requiresAuth: false },
      },
      {
        path: 'cars/review/:id',
        name: 'CarReview',
        component: CarReview,
        meta: { requiresAuth: false },
      },
      {
        path: 'cars/checkout/:id',
        name: 'CarCheckOut',
        component: CarCheckOut,
        meta: { requiresAuth: false },
      },
      {
        path: 'cars/confirmation/:id',
        name: 'CarConfirmation',
        component: CarConfirmation,
        meta: { requiresAuth: false },
      },

      // Hotels Routes
      {
        path: 'hotels/list',
        name: 'HotelsList',
        component: HotelsList,
      },
      {
        path: 'hotels/filter',
        name: 'HotelFilter',
        component: HotelFilter,
      },
      {
        path: 'hotels/details/:id',
        name: 'HotelDetails',
        component: HotelDetails,
      },
      {
        path: 'hotels/review/:id',
        name: 'HotelReview',
        component: HotelReview,
        meta: { requiresAuth: false },
      },
      {
        path: 'hotels/checkout/:id',
        name: 'HotelCheckOut',
        component: HotelCheckOut,
        meta: { requiresAuth: false },
      },
      {
        path: 'hotels/confirmation/:id',
        name: 'HotelConfirmation',
        component: HotelConfirmation,
        meta: { requiresAuth: true },
      },

      // Trips Routes
      {
        path: 'trips/list',
        name: 'TripsList',
        component: TripsList,
      },
      {
        path: 'trips/details/:id',
        name: 'TripDetails',
        component: TripDetails,
      },
      {
        path: 'trips/review/:id',
        name: 'TripReview',
        component: TripReview,
        meta: { requiresAuth: false },
      },
      {
        path: 'trips/checkout/:id',
        name: 'TripCheckOut',
        component: TripCheckOut,
        meta: { requiresAuth: false },
      },
      {
        path: 'trips/confirmation/:id',
        name: 'TripConfirmation',
        component: TripConfirmation,
        meta: { requiresAuth: false },
      },
    ]
  },

  // Dashboard Routes (Protected) - Using nested routing with DashboardLayout
  {
    path: '/dashboard',
    component: DashboardLayout,
    meta: { requiresAuth: false, requiresAdmin: false },
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
      {
        path: 'admins-manage',
        name: 'AdminsManage',
        component: AdminsManage,
      },
      {
        path: 'settings',
        name: 'AdminSettings',
        component: AdminSettings,
      },
      {
        path: 'details/:type/:id',
        name: 'DashboardDetails',
        component: () => import('@/Views/Dashboard/DashboardDetails.vue'),
        props: true
      },
    ],
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
  const requiresAdmin = to.matched.some(record => record.meta.requiresAdmin)

  // Use auth store to check authentication status
  const authStore = useAuthStore()
  authStore.initAuth()

  if (requiresAuth && !authStore.isAuthenticated) {
    // Redirect to login page if not authenticated
    next({
      name: 'Login',
      query: { redirect: to.fullPath }, // Save the intended destination
    })
  } else if (requiresAdmin) {
    const role = authStore.user?.role || ''
    if (role !== 'Admin') {
      next({ path: '/' })
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
