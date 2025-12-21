<template>
  <div class="min-h-screen bg-base-200">
    <!-- Hero Section with Search -->
    <div 
      class="relative bg-cover bg-center min-h-[585px]"
      :style="{ backgroundImage: `linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url(${heroBg || ''})` }"
    >
      <div class="absolute inset-0 flex flex-col items-center justify-center text-center text-white px-4 transform -translate-x-0 -translate-y-[75px]">
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
          <Search
            type="cars"
            @search="handleSearch"
          />
        </div>
      </div>
    </div>

    <!-- Page Title -->
    <div class="my-6 text-center">
      <h1 class="text-2xl sm:text-3xl md:text-4xl font-bold text-base-content">
        Featured Vehicles
      </h1>
      <p class="text-sm sm:text-base text-base-content/70 mt-1">
        Explore our handpicked selection of premium cars
      </p>
    </div>

    <!-- Main Content: Filters Button + Cars Grid -->
    <div class="flex flex-col lg:flex-row gap-6 mt-4">
      <!-- Filter -->
      <div class="w-full lg:w-64 flex-shrink-0">
        <Filter
          :show-price-filter="true"
          :price-range="{ min: 0, max: maxPrice }"
          :category-options="dynamicCategories"
          :custom-filters="dynamicCustomFilters"
          :initial-filters="filters"
          @filter-change="handleFilterChange"
        />
      </div>

      <!-- Cars Grid -->
      <main class="flex-1">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <LuxurySUVCardDynamic 
            v-for="car in filteredCars" 
            :key="car.id" 
            :car="car"
            @view="handleViewCar"
          />
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useCarsStore } from "@/stores/carsStore.js";
import Search from "@/components/Common/Search.vue";
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";
import Filter from "@/components/Common/Filter.vue";
import { useRouter } from 'vue-router';
import { getBackgrounds } from '@/Services/systemService';
const heroBg = ref('');

const store = useCarsStore();
const router = useRouter();

const filters = ref({
  maxPrice: 0,
  categories: [],
  type: [],
  brand: [],
  seats: [],
  transmission: []
});

onMounted(async () => {
  const [_, backgrounds] = await Promise.all([
    store.fetchCars(),
    getBackgrounds('CarsListHero')
  ]);
  filters.value.maxPrice = maxPrice.value;
  heroBg.value = backgrounds[0]?.url || '';
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

</script>
