<template>
    <div class="min-h-screen bg-base-100 pb-12">
      <!-- Hero Section with Background -->
      <div class="relative h-[585px] w-full mb-8">
        <!-- Background Image -->
        <div class="absolute inset-0">
          <img 
            src="/public/images/trip.png" 
            alt="Discover Egypt" 
            class="w-full h-full object-cover"
          />
        </div>

        <!-- Hero Content -->
        <div class="absolute inset-x-0 top-1/2 -translate-y-1/2 text-center text-white px-4">
          <h1 class="text-4xl md:text-6xl font-bold mb-4 drop-shadow-lg font-cairo">Discover Egypt</h1>
          <p class="text-lg md:text-xl max-w-2xl mx-auto mb-20 drop-shadow-md font-cairo text-white/90">
            From ancient pyramids to pristine beaches, find your perfect Egyptian adventure.
          </p>
        </div>

        <!-- Search Bar Positioned over the bottom edge -->
        <div class="absolute bottom-[127px] inset-x-0 mx-[120px] z-20">
          <Search 
            type="trips"
            :initial-data="searchParams"
            @search="handleSearch"
            :show-ai-planner="true"
          />
        </div>
      </div>

    <!-- Main Content -->
    <div class="mt-12 mx-[120px]">
      <!-- Section Header -->
      <div class="text-center mb-10">
        <h2 class="text-2xl md:text-3xl font-bold text-primary mb-2 font-cairo">Rent Your Perfect Ride</h2>
        <p class="text-base-content/70 font-cairo">
          Explore Egypt at your own pace with our diverse fleet of quality vehicles
        </p>
      </div>

      <!-- Trips Grid -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-12">
        <TripCard 
          v-for="trip in paginatedTrips" 
          :key="trip.id" 
          :trip="trip" 
        />
      </div>

       <!-- No Results State -->
       <div v-if="paginatedTrips.length === 0" class="text-center py-20">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-base-200 mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-base-content/40" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>
        <h3 class="text-lg font-bold text-base-content mb-2 font-cairo">No trips found</h3>
        <p class="text-base-content/60 font-cairo">Try changing your search parameters or explore other destinations.</p>
        <button @click="resetSearch" class="btn btn-primary btn-sm mt-4 font-cairo text-white">
          Show All Trips
        </button>
      </div>

      <!-- Pagination -->
      <div v-if="filteredTrips.length > itemsPerPage" class="flex justify-center mt-8">
        <Pagination 
          :current-page="currentPage" 
          :total="filteredTrips.length"
          :per-page="itemsPerPage"
          @update:current-page="handlePageChange"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import tripsApi from '@/Services/tripsApi';
import Search from '@/components/Common/Search.vue';
import TripCard from '@/components/Trips/TripCard.vue';
import Pagination from '@/components/Common/Pagination.vue';

// State
const trips = ref([]);
const currentPage = ref(1);
const itemsPerPage = 8;
const searchParams = ref({});

// Load Data
onMounted(async () => {
  try {
    const result = await tripsApi.getAllTrips();
    trips.value = Array.isArray(result) ? result : [];
  } catch (error) {
    console.error('Failed to load trips from API', error);
    trips.value = [];
  }
});

// Computed Filters
const filteredTrips = computed(() => {
  return trips.value.filter(trip => {
    // Filter by Location
    if (searchParams.value.pickupLocation && searchParams.value.pickupLocation !== 'All Cities') {
       const location = searchParams.value.pickupLocation.toLowerCase();
       const titleMatch = trip.title ? trip.title.toLowerCase().includes(location) : false;
       const descMatch = trip.description ? trip.description.toLowerCase().includes(location) : false;
       
       if (!titleMatch && !descMatch) {
         return false;
       }
    }
    
    // Additional filters (dates, etc) could go here
    // Note: The provided trips JSON doesn't have dates, so we can't filter by exact availability 
    // without more data structure, but this is a starter logic.

    return true;
  });
});

// Pagination Logic
// Pagination component calculates totalPages internaly based on total items and perPage
// We just need to slice the data for the current view

const paginatedTrips = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return filteredTrips.value.slice(start, end);
});

// Handlers
const handleSearch = (params) => {
  searchParams.value = params;
  currentPage.value = 1; // Reset to first page on new search
  console.log('Search params:', params);
};

const handlePageChange = (page) => {
  currentPage.value = page;
  window.scrollTo({ top: 400, behavior: 'smooth' });
};

const resetSearch = () => {
    searchParams.value = {};
    currentPage.value = 1;
}

</script>

<style scoped>
.font-cairo {
  font-family: 'Cairo', sans-serif;
}
</style>
