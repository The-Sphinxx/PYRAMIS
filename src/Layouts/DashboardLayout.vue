<template>
  <div class="flex h-screen bg-base-200 p-4 gap-4 overflow-hidden">
    <!-- Sidebar -->
    <Sidebar />
    
    <!-- Main Content Area -->
    <main class="flex-1 overflow-y-auto overflow-x-hidden rounded-2xl shadow-sm relative min-w-0 max-w-full">
    

      <!-- Page Content -->
      <div class="px-2 md:px-6 lg:px-2 py-2 md:py-2 w-full max-w-full overflow-x-hidden">
        <router-view />
      </div>
    </main>
    <ToastContainer />
  </div>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import Sidebar from '@/components/Dashboard/Sidebar.vue';
import ToastContainer from '@/components/Common/ToastContainer.vue';

const route = useRoute();
const isDark = ref(false);

// Page titles and descriptions based on route
const pageInfo = {
  '/dashboard/overview': {
    title: 'Overview',
    description: 'Welcome back! Here\'s what\'s happening today.'
  },
  '/dashboard/bookings': {
    title: 'Bookings',
    description: 'Manage all customer bookings and reservations.'
  },
  '/dashboard/analytics': {
    title: 'Analytics',
    description: 'View insights and performance metrics.'
  },
  '/dashboard/users-manage': {
    title: 'Users Management',
    description: 'Manage users and their permissions.'
  },
  '/dashboard/trips-manage': {
    title: 'Trips Management',
    description: 'Plan and manage tour packages and trips.'
  },
  '/dashboard/hotels-manage': {
    title: 'Hotels Management',
    description: 'Manage hotel listings and accommodations.'
  },
  '/dashboard/cars-manage': {
    title: 'Cars Management',
    description: 'Manage vehicle rentals and transportation.'
  },
  '/dashboard/attractions-manage': {
    title: 'Attractions Management',
    description: 'Manage tourist attractions and destinations.'
  },
  '/dashboard/support': {
    title: 'Customer Support',
    description: 'Manage customer inquiries and support tickets.'
  }
};

const pageTitle = computed(() => {
  return pageInfo[route.path]?.title || 'Dashboard';
});

const pageDescription = computed(() => {
  return pageInfo[route.path]?.description || '';
});

const toggleTheme = () => {
  isDark.value = !isDark.value;
  const html = document.documentElement;
  html.setAttribute('data-theme', isDark.value ? 'nileheritageDark' : 'nileheritage');
  localStorage.setItem('theme', isDark.value ? 'nileheritageDark' : 'nileheritage');
};

onMounted(() => {
  const savedTheme = localStorage.getItem('theme');
  isDark.value = savedTheme === 'nileheritageDark';
  document.documentElement.setAttribute('data-theme', savedTheme || 'nileheritage');
});
</script>

<style scoped>
/* Ensure proper scrolling */
main {
  max-height: 100vh;
}

/* Prevent horizontal overflow */
:deep(body) {
  overflow-x: hidden;
}
</style>
