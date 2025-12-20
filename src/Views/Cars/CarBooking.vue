<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px]"
      :style="{ backgroundImage: `linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url(${heroImage})` }"
    >
<!-- Positioned Content -->
<div class="absolute inset-0 flex flex-col items-center justify-center text-center text-white px-4 transform -translate-x-0 -translate-y-[75px]">
  <!-- Hero Text -->
  <div class="max-w-4xl mb-4">
    <h1 class="font-cairo text-3xl sm:text-4xl md:text-5xl lg:text-6xl font-bold leading-tight">
      Find Your
      <span
        class="relative inline-block
        bg-[linear-gradient(to_bottom,white_56%,transparent_85%)]
        bg-clip-text text-transparent"
      >
        Perfect Ride
      </span>
    </h1>
    <p class="font-cairo text-sm sm:text-base md:text-lg text-gray-200 mt-2 leading-relaxed">
      Discover premium vehicles for every journey.
      Luxury, comfort, and reliability at your fingertips.
    </p>
  </div>

  <!-- Search Bar Component -->
  <div class="w-full max-w-6xl">
    <Search
      type="cars"
      @search="handleSearch"
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

    <!-- Cars Grid -->
    <div class="page-container grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
      <LuxurySUVCardDynamic
        v-for="car in cars"
        :key="car.id"
        :car="car"
        @view="handleViewCar"
      />
    </div>

    <!-- View All -->
    <div class="page-container mt-10 max-w-md mx-auto">
      <button
        class="btn btn-primary w-full"
        @click="goToCarsList"
      >
        View All
      </button>
    </div>

    <!-- Popular Brands -->
    <div class="page-container my-12 text-center">
      <h1 class="font-cairo text-2xl sm:text-3xl md:text-4xl font-bold text-base-content">
        Popular Brands
      </h1>
      <p class="font-cairo text-sm sm:text-base text-gray-500 mt-1">
        Choose from the world's leading automotive manufacturers
      </p>
      <div class="flex justify-center gap-6 flex-wrap mt-4">
        <button
          v-for="brand in brands"
          :key="brand"
          @click="filterByBrand(brand)"
          class="btn btn-outline btn-secondary font-semibold py-5 px-8 transition hover:scale-105"
        >
          {{ brand }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useCarsStore } from "@/stores/carsStore";

import Search from "@/components/Common/Search.vue";
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";

import heroImage from "@/assets/images/CarHeroSection.jpg";

const store = useCarsStore();
const router = useRouter();

const brands = ["Mercedes", "BMW", "Audi", "Tesla"];

onMounted(async () => {
  await store.fetchCars();
});

const cars = computed(() => store.cars);

function handleViewCar(car) {
  router.push({ name: "CarDetails", params: { id: car.id } });
}

function goToCarsList() {
  router.push({ name: "CarsList" });
}

function filterByBrand(brand) {
  router.push({ name: "CarsList", query: { brand } });
}

function handleSearch(payload) {
  console.log("Cars search:", payload);
}
</script>

<style scoped>
.font-cairo {
  font-family: 'Cairo', sans-serif;
}
</style>
