<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px]"
      :style="{ backgroundImage: `linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url(${heroImage})` }"
    >
      <!-- Positioned Content -->
      <div class="absolute inset-0 flex flex-col items-center justify-start pt-32 lg:justify-center lg:pt-20 text-center text-white px-4">
        <!-- Hero Text -->
        <div class="max-w-4xl mb-4">
          <h1 class="font-cairo text-3xl sm:text-4xl md:text-5xl lg:text-6xl font-bold leading-tight">
            Find Your Perfect Ride
            
          </h1>
          <p class="font-cairo text-sm sm:text-base md:text-lg text-gray-200 mt-2 leading-relaxed">
            Discover premium vehicles for every journey.
            Luxury, comfort, and reliability at your fingertips.
          </p>
        </div>

        <!-- Search Bar -->
        <div class="w-full max-w-6xl">
          <Search 
            type="cars" 
            :client-side-mode="true"
            :data-to-filter="store.cars"
            :initial-data="searchParams"
            @search="handleSearch"
            @filtered-results="handleFilteredResults"
          />
        </div>
      </div>
    </div>

    <!-- Page Title -->
    <div class="page-container my-6 text-center">
      <h1 class="font-cairo text-2xl sm:text-3xl md:text-4xl font-bold text-base-content">
        Featured Vehicles
      </h1>
      <p class="font-cairo text-sm sm:text-base text-gray-500 mt-1">
        Explore our handpicked selection of premium cars
      </p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="page-container flex justify-center items-center py-20">
      <span class="loading loading-spinner loading-lg text-primary"></span>
    </div>

    <!-- Cars Grid -->
    <div v-else-if="paginatedCars.length > 0" class="page-container">
      <div class="grid gap-6 grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
        <LuxurySUVCardDynamic
          v-for="car in paginatedCars"
          :key="car.id"
          :car="car"
          @view="handleViewCar"
        />
      </div>

      <!-- Pagination Component -->
      <div class="mt-8 flex justify-center">
        <Pagination
          :current-page="currentPage"
          :total="totalCount"
          :per-page="itemsPerPage"
          :show-info="true"
          :show-per-page-selector="true"
          :per-page-options="[10, 20, 30]"
          @update:current-page="handlePageChange"
          @update:per-page="async (val) => { itemsPerPage.value = val; currentPage.value = 1; await fetchCarsData(1); }"
        />
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="page-container text-center py-20">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-base-300 mx-auto mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16h8m-4-8v8m0-8V6m10 10v6H2v-6m0 0V6a2 2 0 012-2h16a2 2 0 012 2v10z" />
      </svg>
      <h3 class="text-2xl font-bold text-base-content mb-2 font-cairo">No cars found</h3>
      <p class="text-base-content/60 mb-4 font-cairo">Try adjusting your search parameters or filters</p>
      <button @click="resetFilters" class="btn btn-primary font-cairo text-white">
        Reset Filters
      </button>
    </div>

    <!-- Error State -->
    <div v-if="error" class="page-container alert alert-error my-6">
      <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l-2-2m0 0l-2-2m2 2l2-2m-2 2l-2 2m9-11a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      <span>{{ error }}</span>
    </div>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, computed, ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useCarsStore } from "@/stores/carsStore";

import Search from "@/components/Common/Search.vue";
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";
import Pagination from "@/components/Common/Pagination.vue";

import heroImage from "@/assets/images/CarHeroSection.jpg";

const store = useCarsStore();
const router = useRouter();
const route = useRoute();

const currentPage = ref(1);
const itemsPerPage = ref(12);
const fetchKey = ref(0);

// Client-side filtering state
const filteredCars = ref([]);
const isFiltering = ref(false);
const searchParams = ref({});

const loading = computed(() => store.loading);
const error = computed(() => store.error);

const totalPages = computed(() => {
  if (isFiltering.value) {
    return Math.ceil(filteredCars.value.length / itemsPerPage.value);
  }
  return store.pagination.totalPages;
});

const totalCount = computed(() => {
  if (isFiltering.value) {
    return filteredCars.value.length;
  }
  return store.pagination.totalCount;
});

const fetchCarsData = async (page = 1) => {
  await store.fetchCars({
    pageIndex: page - 1, // API uses 0-based indexing
    pageSize: itemsPerPage.value
  });
};

onMounted(async () => {
  // Read query parameters from route (from Home page navigation)
  if (route.query.location || route.query.pickupDate || route.query.pickupTime) {
    searchParams.value = {
      pickupLocation: route.query.location || '',
      pickupDate: route.query.pickupDate || null,
      pickupTime: route.query.pickupTime || '10:00',
      dropoffDate: route.query.dropoffDate || null,
      dropoffTime: route.query.dropoffTime || '10:00'
    };
  }
  
  await fetchCarsData(currentPage.value);
});

const paginatedCars = computed(() => {
  if (isFiltering.value) {
    const start = (currentPage.value - 1) * itemsPerPage.value;
    const end = start + itemsPerPage.value;
    return filteredCars.value.slice(start, end);
  }
  const cars = store.cars;
  console.log('🎨 RENDER Carslist - Total Cars:', cars?.length);
  console.log('🎨 RENDER Carslist - Car IDs:', cars?.map(c => c.id));
  console.log('🎨 RENDER Carslist - Has Duplicates?', new Set(cars?.map(c => c.id)).size !== cars?.length);
  return cars;
});

function handleViewCar(car) {
  router.push({ name: "CarDetails", params: { id: car.id } });
}

function goToCarBooking() {
  router.push({ name: "CarBooking" });
}

function goToCategory(category) {
  router.push({
    name: "CarBooking",
    query: { category }
  });
}

function handleFilteredResults(results) {
  filteredCars.value = results;
  isFiltering.value = results.length < store.cars.length || searchParams.value.pickupLocation;
  currentPage.value = 1;
}

function handleSearch(payload) {
  // Client-side filtering is handled automatically
  console.log('Search triggered:', payload);
}

async function handlePageChange(page) {
  currentPage.value = page;
  await fetchCarsData(page);
  window.scrollTo({ top: 400, behavior: 'smooth' });
}

async function resetFilters() {
  isFiltering.value = false;
  filteredCars.value = [];
  searchParams.value = {};
  currentPage.value = 1;
  await fetchCarsData(1);
}

onUnmounted(() => {
  currentPage.value = 1;
  isFiltering.value = false;
  filteredCars.value = [];
});
</script>

<style scoped>
.font-cairo {
  font-family: "Cairo", sans-serif;
}
.page-container {
  max-width: 1280px;
  margin-inline: auto;
  padding-inline: 1rem;
}
</style>
