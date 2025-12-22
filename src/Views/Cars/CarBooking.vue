<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px]"
      :style="{ backgroundImage: `linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url(${heroBg || ''})` }"
    >
      <div class="absolute inset-0 flex flex-col items-center justify-center text-center text-white px-4 transform -translate-y-[75px]">
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

        <div class="w-full max-w-6xl">
          <Search type="cars" @search="handleSearch" />
        </div>
      </div>
    </div>

    <!-- Page Title -->
    <div class="my-6 text-center">
      <!-- <h1 class="text-2xl sm:text-3xl md:text-4xl font-bold text-base-content">
All Results       </h1>
      <p class="text-sm sm:text-base text-base-content/70 mt-1">
        Explore our handpicked selection of premium cars
      </p> -->
    </div>
<!-- Filter + Cars Grid -->
<div class="page-container">
  <!-- Header: All Results + Filter -->
  <div class="flex justify-between items-center mb-4">
    <h1 class="text-2xl sm:text-3xl md:text-4xl font-bold text-primary">
      All Results
    </h1>
    <Filter
      class="w-full lg:w-64"
      :show-price-filter="true"
      :price-range="{ min: 0, max: maxPrice }"
      :category-options="dynamicCategories"
      :custom-filters="dynamicCustomFilters"
      :initial-filters="filters"
      @filter-change="handleFilterChange"
    />
  </div>

  <!-- Cars Grid -->
<div class="grid gap-6 grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
    <LuxurySUVCardDynamic 
      v-for="car in filteredCars" 
      :key="car.id" 
      :car="car"
      @view="handleViewCar"
    />
  </div>
</div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { useCarsStore } from "@/stores/carsStore.js";
import Search from "@/components/Common/Search.vue";
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";
import Filter from "@/components/Common/Filter.vue";
import { useRouter, useRoute } from 'vue-router';
import heroImage from "@/assets/images/CarHeroSection.jpg";

const store = useCarsStore();
const router = useRouter();
const route = useRoute();

const filters = ref({
  maxPrice: 0,
  categories: [],
  type: [],
  brand: [],
  seats: [],
  transmission: []
});

onMounted(async () => {
  await store.fetchCars();
  filters.value.maxPrice = maxPrice.value;
  if (route.query.category) {
    filters.value.brand = [route.query.category]; 
  }
});

const maxPrice = computed(() => {
  if (!store.cars.length) return 300;
  return Math.max(...store.cars.map(c => c.price || 0));
});

const dynamicCategories = computed(() => {
  return Array.from(new Set(store.cars.map(c => c.category))).map(c => ({ label: c, value: c }));
});

const dynamicCustomFilters = computed(() => {
  const brands = Array.from(new Set(store.cars.map(c => c.name.split(" ")[0]))).map(b => ({ label: b, value: b }));
  const transmissions = Array.from(new Set(store.cars.map(c => c.transmission))).map(t => ({ label: t, value: t }));
  const seatsOptions = Array.from(new Set(store.cars.map(c => c.seats))).map(s => ({ label: `${s} Seats`, value: s }));
  return [
    { key: 'brand', title: 'Brand', options: brands },
    { key: 'seats', title: 'Seats', options: seatsOptions },
    { key: 'transmission', title: 'Transmission', options: transmissions },
    { key: 'type', title: 'Car Type', options: dynamicCategories.value }
  ];
});

const filteredCars = computed(() => {
  return store.cars.filter(car => {
    const priceMatch = car.price <= filters.value.maxPrice;
    const typeMatch = filters.value.type.length ? filters.value.type.includes(car.category) : true;
    const brandMatch = filters.value.brand.length ? filters.value.brand.some(b => car.name.includes(b)) : true;
    const seatsMatch = filters.value.seats.length ? filters.value.seats.includes(car.seats) : true;
    const transmissionMatch = filters.value.transmission.length ? filters.value.transmission.includes(car.transmission) : true;
    const categoryMatch = filters.value.categories.length ? filters.value.categories.includes(car.category) : true;
    return priceMatch && typeMatch && brandMatch && seatsMatch && transmissionMatch && categoryMatch;
  });
});

function handleFilterChange(newFilters) {
  filters.value = { ...filters.value, ...newFilters };
}

function handleViewCar(car) {
  router.push({ name: 'CarDetails', params: { id: car.id } });
}

function handleSearch(query) {
  console.log("Search query:", query);
}

watch(() => route.query.category, (newCategory) => {
  if (newCategory) filters.value.brand = [newCategory]; 
});
</script>

<style scoped>
.font-cairo {
  font-family: "Cairo", sans-serif;
}
</style>
