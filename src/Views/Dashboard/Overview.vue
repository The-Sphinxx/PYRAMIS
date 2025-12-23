<template>
  <div class="min-h-screen bg-base-200 space-y-2">
    <!-- Stats Overview Section -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
      <!-- Hotels Stats -->
      <div class="stats shadow bg-base-100 border border-base-300">
        <div class="stat">
          <div class="stat-figure text-primary">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-8 h-8 stroke-current">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"></path>
            </svg>
          </div>
          <div class="stat-title">Total Hotels</div>
          <div class="stat-value text-primary">{{ hotels.length }}</div>
          <div class="stat-desc">{{ activeHotelsCount }} active properties</div>
        </div>
      </div>

      <!-- Cars Stats -->
      <div class="stats shadow bg-base-100 border border-base-300">
        <div class="stat">
          <div class="stat-figure text-secondary">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-8 h-8 stroke-current">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
            </svg>
          </div>
          <div class="stat-title">Cars Fleet</div>
          <div class="stat-value text-secondary">{{ cars.length }}</div>
          <div class="stat-desc">{{ availableCarsCount }} available now</div>
        </div>
      </div>

      <!-- Attractions Stats -->
      <div class="stats shadow bg-base-100 border border-base-300">
        <div class="stat">
          <div class="stat-figure text-accent">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-8 h-8 stroke-current">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
            </svg>
          </div>
          <div class="stat-title">Attractions</div>
          <div class="stat-value text-accent">{{ attractions.length }}</div>
          <div class="stat-desc">{{ availableAttractionsCount }} available</div>
        </div>
      </div>

      <!-- Trips Stats -->
      <div class="stats shadow bg-base-100 border border-base-300">
        <div class="stat">
          <div class="stat-figure text-info">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-8 h-8 stroke-current">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 21v-4m0 0V5a2 2 0 012-2h6.5l1 1H21l-3 6 3 6h-8.5l-1-1H5a2 2 0 00-2 2zm9-13.5V9"></path>
            </svg>
          </div>
          <div class="stat-title">Trips</div>
          <div class="stat-value text-info">{{ trips.length }}</div>
          <div class="stat-desc">{{ upcomingTripsCount }} upcoming</div>
        </div>
      </div>

      <!-- Bookings Stats -->
      <div class="stats shadow bg-base-100 border border-base-300">
        <div class="stat">
          <div class="stat-figure text-success">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-8 h-8 stroke-current">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"></path>
            </svg>
          </div>
          <div class="stat-title">Total Bookings</div>
          <div class="stat-value text-success">{{ bookings.length }}</div>
          <div class="stat-desc">{{ confirmedBookingsCount }} confirmed</div>
        </div>
      </div>

      <!-- Users Stats -->
      <div class="stats shadow bg-base-100 border border-base-300">
        <div class="stat">
          <div class="stat-figure text-warning">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-8 h-8 stroke-current">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"></path>
            </svg>
          </div>
          <div class="stat-title">Total Users</div>
          <div class="stat-value text-warning">{{ users.length }}</div>
          <div class="stat-desc">{{ activeUsersCount }} active users</div>
        </div>
      </div>
    </div>

    <!-- Analytics Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Analytics Overview</h2>
          <router-link 
            :to="{ name: 'Analytics' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        
        <!-- Compact Charts -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <!-- Monthly Bookings & Revenue -->
          <div class="bg-base-200/30 p-4 rounded-lg">
            <h3 class="font-semibold text-sm mb-3">Monthly Bookings & Revenue</h3>
            <div class="h-64">
              <Bar :data="mixedChartData" :options="mixedChartOptions" />
            </div>
          </div>

          <!-- Booking Status Distribution -->
          <div class="bg-base-200/30 p-4 rounded-lg">
            <h3 class="font-semibold text-sm mb-3">Booking Status</h3>
            <div class="h-64 flex items-center justify-center">
              <PolarArea :data="polarAreaChartData" :options="polarAreaChartOptions" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Hotels Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Hotels Management</h2>
          <router-link 
            :to="{ name: 'HotelsManage' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <HotelsManage :compact-mode="true" />
      </div>
    </div>

    <!-- Cars Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Cars Fleet Management</h2>
          <router-link 
            :to="{ name: 'CarsManage' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <CarsManage :compact-mode="true" />
      </div>
    </div>

    <!-- Attractions Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Attractions Management</h2>
          <router-link 
            :to="{ name: 'AttractionsManage' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <AttractionsManage :compact-mode="true" />
      </div>
    </div>

    <!-- Trips Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Trips Management</h2>
          <router-link 
            :to="{ name: 'TripsManage' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <TripsManage :compact-mode="true" />
      </div>
    </div>

    <!-- Bookings Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Bookings Management</h2>
          <router-link 
            :to="{ name: 'Bookings' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <Bookings :compact-mode="true" />
      </div>
    </div>

    <!-- Users Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Users Management</h2>
          <router-link 
            :to="{ name: 'UsersManage' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <UsersManage :compact-mode="true" />
      </div>
    </div>

    <!-- Admins Section -->
    <div class="card bg-base-100 shadow-xl border border-base-300">
      <div class="card-body">
        <div class="flex items-center justify-between mb-4">
          <h2 class="card-title text-xl">Admins Management</h2>
          <router-link 
            :to="{ name: 'AdminsManage' }" 
            class="btn btn-sm btn-primary gap-2"
          >
            Open Full Page
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"/>
            </svg>
          </router-link>
        </div>
        <AdminsManage :compact-mode="true" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import { Bar, PolarArea } from 'vue-chartjs';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  RadialLinearScale,
  ArcElement
} from 'chart.js';

// Import actual management components
import HotelsManage from './HotelsManage.vue';
import CarsManage from './CarsManage.vue';
import AttractionsManage from './AttractionsManage.vue';
import TripsManage from './TripsManage.vue';
import Bookings from './Bookings.vue';
import UsersManage from './UsersManage.vue';
import AdminsManage from './AdminsManage.vue';

import { 
  hotelsAPI, 
  carsAPI, 
  attractionsAPI, 
  tripsAPI, 
  bookingsAPI, 
  usersAPI 
} from '@/Services/dashboardApi';

// Register Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  RadialLinearScale,
  ArcElement
);

const authStore = useAuthStore();
const userName = computed(() => authStore.user?.firstName || 'Admin');

// Data refs
const hotels = ref([]);
const cars = ref([]);
const attractions = ref([]);
const trips = ref([]);
const bookings = ref([]);
const users = ref([]);

// Fetch all data
const fetchAllData = async () => {
  try {
    const [hotelsRes, carsRes, attractionsRes, tripsRes, bookingsRes, usersRes] = 
      await Promise.all([
        hotelsAPI.getAll(),
        carsAPI.getAll(),
        attractionsAPI.getAll(),
        tripsAPI.getAll(),
        bookingsAPI.getAll(),
        usersAPI.getAll()
      ]);

    hotels.value = hotelsRes.data || [];
    cars.value = carsRes.data || [];
    attractions.value = attractionsRes.data || [];
    trips.value = tripsRes.data || [];
    bookings.value = bookingsRes.data || [];
    users.value = usersRes.data || [];
  } catch (error) {
    console.error('Error fetching overview data:', error);
  }
};

// Preview data (top 3 items) - Transform data same as management pages
const hotelsPreview = computed(() => 
  hotels.value.slice(0, 3).map(hotel => ({
    ...hotel,
    images: Array.isArray(hotel.images) ? hotel.images[0] : hotel.images
  }))
);

const carsPreview = computed(() => 
  cars.value.slice(0, 3).map(car => ({
    ...car,
    images: Array.isArray(car.images) ? car.images[0] : car.images,
    location: car.city || 'Cairo',
    price: car.price || car.pricePerDay,
    status: car.status || 'Available'
  }))
);

const attractionsPreview = computed(() => 
  attractions.value.slice(0, 3).map(attraction => ({
    ...attraction,
    images: Array.isArray(attraction.images) ? attraction.images[0] : attraction.images
  }))
);

const tripsPreview = computed(() => 
  trips.value.slice(0, 3).map(trip => ({
    ...trip,
    images: Array.isArray(trip.images) ? trip.images[0] : trip.images
  }))
);

const bookingsPreview = computed(() => bookings.value.slice(0, 3));
const usersPreview = computed(() => users.value.slice(0, 3));
const adminsPreview = computed(() => 
  users.value.filter(u => u.role === 'admin').slice(0, 3)
);

// Stats calculations for badges
const activeHotelsCount = computed(() => 
  hotels.value.filter(h => h.status === 'Active').length
);

const availableCarsCount = computed(() => 
  cars.value.filter(c => c.status === 'Available' || c.available_now > 0).length
);

const availableAttractionsCount = computed(() => 
  attractions.value.filter(a => a.availability === 'Available').length
);

const upcomingTripsCount = computed(() => 
  trips.value.filter(t => t.status === 'Upcoming').length
);

const confirmedBookingsCount = computed(() => 
  bookings.value.filter(b => b.status === 'confirmed' || b.status === 'Confirmed').length
);

const activeUsersCount = computed(() => 
  users.value.filter(u => u.status === 'Active' && u.role !== 'admin').length
);

// Column configurations (same as management pages)
const hotelsColumns = [
  { label: 'Property', field: 'images', type: 'image', showNameWithImage: true, headerClass: 'w-1/4' },
  { label: 'Rating', field: 'rating', type: 'rating', headerClass: 'w-1/6' },
  { label: 'City', field: 'city', type: 'text', headerClass: 'w-1/6' },
  { label: 'Price / Night', field: 'price', type: 'price', headerClass: 'w-1/6' },
  { label: 'Actions', type: 'actions', headerClass: 'w-1/6' }
];

const carsColumns = [
  { label: 'Vehicle', field: 'images', type: 'image', showNameWithImage: true, headerClass: 'w-1/4' },
  { label: 'City', field: 'city', type: 'text', headerClass: 'w-1/6' },
  { label: 'Price / Day', field: 'price', type: 'price', headerClass: 'w-1/6' },
  { label: 'Total Fleet', field: 'total_fleet', type: 'text', headerClass: 'w-1/12' },
  { label: 'Actions', type: 'actions', headerClass: 'w-1/6' }
];

const attractionsColumns = [
  { label: 'Attraction', field: 'images', type: 'image', showNameWithImage: true, headerClass: 'w-1/4' },
  { label: 'Price', field: 'price', type: 'text', headerClass: 'w-1/6' },
  { label: 'City', field: 'city', type: 'text', headerClass: 'w-1/6' },
  { label: 'Rating', field: 'rating', type: 'rating', headerClass: 'w-1/6' },
  { label: 'Action', type: 'actions', headerClass: 'w-1/8' }
];

const tripsColumns = [
  { label: 'Trip', field: 'images', type: 'image', showNameWithImage: true, headerClass: 'w-1/4' },
  { label: 'Destination', field: 'city', type: 'text', headerClass: 'w-1/6' },
  { label: 'Duration', field: 'duration', type: 'text', headerClass: 'w-1/6' },
  { label: 'Price', field: 'price', type: 'price', headerClass: 'w-1/6' },
  { label: 'Actions', type: 'actions', headerClass: 'w-1/6' }
];

const bookingsColumns = [
  { label: 'Booking ID', field: 'bookingNumber', type: 'text', headerClass: 'w-1/6' },
  { label: 'Item', field: 'itemName', type: 'text', headerClass: 'w-1/4' },
  { label: 'Type', field: 'type', type: 'text', headerClass: 'w-1/8' },
  { label: 'Date', field: 'bookingDate', type: 'date', headerClass: 'w-1/6' },
  { label: 'Actions', type: 'actions', headerClass: 'w-1/6' }
];

const usersColumns = [
  { label: 'User', field: 'profileImage', type: 'image', showNameWithImage: true, headerClass: 'w-1/4' },
  { label: 'Email', field: 'email', type: 'text', headerClass: 'w-1/4' },
  { label: 'Country', field: 'country', type: 'text', headerClass: 'w-1/6' },
  { label: 'Status', field: 'status', type: 'badge', headerClass: 'w-1/8' },
  { label: 'Actions', type: 'actions', headerClass: 'w-1/6' }
];

const adminsColumns = [
  { label: 'Admin', field: 'profileImage', type: 'image', showNameWithImage: true, headerClass: 'w-1/4' },
  { label: 'Email', field: 'email', type: 'text', headerClass: 'w-1/4' },
  { label: 'Role', field: 'role', type: 'text', headerClass: 'w-1/6' },
  { label: 'Status', field: 'status', type: 'badge', headerClass: 'w-1/8' },
  { label: 'Actions', type: 'actions', headerClass: 'w-1/6' }
];

// Chart Data (from Analytics.vue)
const mixedChartData = computed(() => ({
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  datasets: [
    {
      label: 'Bookings',
      data: [45, 52, 48, 65, 70, 68, 75, 80, 72, 85, 90, 95],
      backgroundColor: 'rgba(200, 106, 65, 0.8)', // primary color
      borderColor: '#C86A41',
      borderWidth: 2
    },
    {
      label: 'Revenue (x1000 EGP)',
      data: [120, 145, 135, 180, 195, 185, 210, 225, 205, 240, 255, 270],
      backgroundColor: 'rgba(54, 179, 126, 0.8)', // success color
      borderColor: '#36B37E',
      borderWidth: 2
    }
  ]
}));

const mixedChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { position: 'top' },
    title: { display: false }
  }
};

const polarAreaChartData = computed(() => ({
  labels: ['Confirmed', 'Pending', 'Cancelled', 'Completed'],
  datasets: [{
    label: 'Bookings',
    data: [
      bookings.value.filter(b => b.status === 'confirmed').length,
      bookings.value.filter(b => b.status === 'pending').length,
      bookings.value.filter(b => b.status === 'cancelled').length,
      bookings.value.filter(b => b.status === 'completed').length
    ],
    backgroundColor: [
      'rgba(54, 179, 126, 0.7)',  // confirmed - success #36B37E
      'rgba(242, 165, 64, 0.7)',  // pending - warning #F2A540
      'rgba(228, 88, 88, 0.7)',   // cancelled - error #E45858
      'rgba(200, 106, 65, 0.7)'   // completed - primary #C86A41
    ],
    borderWidth: 2,
    borderColor: '#fff'
  }]
}));

const polarAreaChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { position: 'bottom' }
  }
};

onMounted(() => {
  fetchAllData();
});
</script>
