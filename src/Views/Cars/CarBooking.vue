<template>
  <div class="p-4 sm:p-6 md:p-8 bg-gray-100 min-h-screen">
    <!-- Hero Section -->
    <CarHeroSection @search="handleSearch" />

```
<!-- Page Title -->
<div class="my-6 text-center">
  <h1 class="text-2xl sm:text-3xl md:text-4xl font-bold text-black">Featured Vehicles</h1>
  <p class="text-sm sm:text-base text-gray-500 mt-1">Explore our handpicked selection of premium cars</p>
</div>

<!-- Cars Grid -->
<div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6 auto-rows-fr">
  <LuxurySUVCardDynamic 
    v-for="car in cars" 
    :key="car.id" 
    :car="car"
    @view="handleViewCar"
    class="w-full flex flex-col min-w-0 overflow-hidden"
  />
</div>

<!-- View All Button -->
<div class="mt-6 sm:mt-8 md:mt-10 max-w-md mx-auto">
    <button 
      class="w-full bg-[#C86A41] hover:bg-[#b65c36] text-white py-2 rounded-xl font-semibold transition-all duration-200"
    @click="goToCarsList"
    >
    View All
    </button>
</div>
<br><br>

<!-- Popular Brands -->
<div class="my-6 text-center">
  <h1 class="text-2xl sm:text-3xl md:text-4xl font-bold text-black">Popular Brands</h1>
  <p class="text-sm sm:text-base text-gray-500 mt-1">Choose from the world's leading automotive manufacturers</p>
</div>

<div class="mt-6 sm:mt-8 md:mt-10 flex justify-center gap-12 flex-wrap">
  <button 
    @click="filterByBrand('Mercedes')"
    class="bg-gray-100 text-[#C86A41] border border-[#121826] font-semibold py-5 px-8 rounded-xl transition-all duration-200 hover:bg-gray-200 hover:scale-105 transform"
  >
    Mercedes
  </button>
  <button 
    @click="filterByBrand('BMW')"
    class="bg-gray-100 text-[#C86A41] border border-[#121826] font-semibold py-5 px-8 rounded-xl transition-all duration-200 hover:bg-gray-200 hover:scale-105 transform"
  >
    BMW
  </button>
  <button 
    @click="filterByBrand('Audi')"
    class="bg-gray-100 text-[#C86A41] border border-[#121826] font-semibold py-5 px-8 rounded-xl transition-all duration-200 hover:bg-gray-200 hover:scale-105 transform"
  >
    Audi
  </button>
  <button 
    @click="filterByBrand('Tesla')"
    class="bg-gray-100 text-[#C86A41] border border-[#121826] font-semibold py-5 px-8 rounded-xl transition-all duration-200 hover:bg-gray-200 hover:scale-105 transform"
  >
    Tesla
  </button>
</div>
```

  </div>
</template>

<script setup>
import { onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useCarsStore } from "@/stores/carsStore.js";
import CarHeroSection from "@/components/Cars/CarHeroSection.vue"; 
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";

const store = useCarsStore();
const router = useRouter();

onMounted(async () => {
  await store.fetchCars();
});

const cars = computed(() => store.cars);

function handleViewCar(car) {
  // Navigate to CarDetails page
  router.push({ name: 'CarDetails', params: { id: car.id } });
}

function goToCarsList() {
  // Navigate to CarsList page
  router.push({ name: 'CarsList' });
}

function filterByBrand(brand) {
  // Navigate to CarsList page with brand filter
  router.push({ name: 'CarsList', query: { brand } });
}

function handleSearch(query) {
  console.log("Search query:", query);
}
</script>
