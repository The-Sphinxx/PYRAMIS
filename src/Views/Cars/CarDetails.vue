<template>
  <div class="p-4 sm:p-6 md:p-8 bg-gray-100 min-h-screen">
    <!-- Car Main Section -->
    <div v-if="car" class="max-w-[1400px] mx-auto mt-6 grid grid-cols-1 lg:grid-cols-12 gap-6">
      <!-- Left: Car Images and Info -->
      <div class="lg:col-span-8">
        <div class="bg-white rounded-lg p-6">
          <!-- Main Image -->
          <div class="relative bg-gray-200 rounded-lg overflow-hidden mb-4">
            <img :src="car.mainImage || 'https://via.placeholder.com/800x450'" :alt="car.name" class="w-full h-96 object-cover" />
          </div>

```
      <!-- Thumbnails -->
      <div class="flex gap-3 overflow-x-auto">
        <button 
          v-for="(img, idx) in car.images || ['https://via.placeholder.com/80x60','https://via.placeholder.com/80x60','https://via.placeholder.com/80x60']" 
          :key="idx" 
          @click="selectedImage = idx"
          class="flex-shrink-0 rounded-lg overflow-hidden border-2"
          :class="selectedImage === idx ? 'border-blue-500' : 'border-gray-200'"
        >
          <img :src="img" class="w-20 h-16 object-cover" />
        </button>
      </div>

      <!-- Car Info -->
      <div class="mt-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h1 class="text-2xl font-bold text-black mb-2">{{ car.name }}</h1>
            <div class="flex items-center gap-2">
              <div class="flex">
                <Star v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= car.rating ? 'fill-amber-400 text-amber-400' : 'text-gray-300'" />
              </div>
              <span class="text-sm text-black">{{ car.reviews?.length || 0 }} Reviewer</span>
            </div>
          </div>
          <div class="text-right">
            <span class="text-3xl font-bold text-black">{{ car.price }}</span>
            <p class="text-sm text-black">/ days</p>
          </div>
        </div>

        <p class="text-black text-sm mb-6 leading-relaxed">{{ car.description }}</p>

        <!-- Feature Grid -->
    <div class="grid grid-cols-2 gap-[80px]">
  <div v-for="feature in car.features || []" :key="feature.label" class="flex items-center gap-3 bg-pink-50 rounded-lg p-3">
    <div class="w-10 h-10 bg-white rounded-full flex items-center justify-center">
      <component :is="feature.icon" class="w-5 h-5 text-red-400" />
    </div>
    <div>
      <p class="text-xs text-black">{{ feature.label }}</p>
      <p class="text-sm font-semibold text-black">{{ feature.value }}</p>
    </div>
  </div>
</div>

<br>

<div class="grid grid-cols-2 gap-[80px]">
  <div v-for="feature in car.features || []" :key="feature.label" class="flex items-center gap-6 bg-pink-50 rounded-lg p-3">
    <div class="w-10 h-10 bg-white rounded-full flex items-center justify-center">
      <component :is="feature.icon" class="w-5 h-5 text-red-400" />
    </div>
    <div>
      <p class="text-xs text-black">{{ feature.label }}</p>
      <p class="text-sm font-semibold text-black">{{ feature.value }}</p>
    </div>
  </div>
</div>

      </div>
    </div>
  </div>

  <!-- Right: Add-Ons -->
  <div class="lg:col-span-4">
    <div class="bg-white rounded-lg p-6 h-full">
      <h2 class="font-bold text-black text-lg mb-6">Add-Ons & Extras</h2>

      <div class="space-y-4 mb-6">
        <div v-for="add in car.addOns || []" :key="add.name" class="flex justify-between items-center text-sm">
          <span class="text-gray-600">• {{ add.name }}</span>
          <span class="text-red-500 font-semibold" v-if="add.price">+ {{ add.price }}/day</span>
          <span class="font-semibold" v-else>{{ car.price }}/day</span>
        </div>
      </div>

      <div class="border-t pt-4 mb-6">
        <div class="flex justify-between items-center">
          <span class="font-bold text-black text-sm">Total per day</span>
          <span class="font-bold text-2xl">{{ car.total || car.price }}</span>
        </div>
      </div>

      <button class="w-full bg-gradient-to-r from-orange-400 to-orange-500 text-white py-3 rounded-lg font-semibold shadow-lg hover:shadow-xl transition-all mb-4">
        Continue Booking
      </button>

      <div class="text-center">
        <input 
          type="text" 
          placeholder="Promo code/coupon for Discount code(optional)" 
          class="w-full border border-gray-300 rounded-lg px-4 py-2 text-sm text-gray-500"
        />
      </div>
    </div>
  </div>
</div>

<!-- You May Also Like -->
<div v-if="recommendedCars.length" class="bg-white rounded-lg p-3 mt-3 max-w-[1400px] mx-auto relative">
  <h2 class="text-lg font-bold text-black mb-3">You May Also Like</h2>
  <div class="flex gap-3 overflow-x-auto pb-3 scroll-smooth" ref="youMayScroll">
    <LuxurySUVCardDynamic 
      v-for="rec in recommendedCars.slice(0,5)" 
      :key="rec.id" 
      :car="rec" 
      @view="() => handleViewCar(rec.id)"
    />
  </div>


</div>
```

  </div>
</template>

<script setup>
import { onMounted, computed, ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useCarsStore } from "@/stores/carsStore.js";
import LuxurySUVCardDynamic from "@/components/Cars/CarCard.vue";
import { Star } from 'lucide-vue-next';

const store = useCarsStore();
const router = useRouter();
const route = useRoute();
const selectedImage = ref(0);
const youMayScroll = ref(null);

onMounted(async () => {
  await store.fetchCars();
});

const car = computed(() => store.cars.find(c => c.id === parseInt(route.params.id)));
const recommendedCars = computed(() => store.cars.filter(c => c.id !== parseInt(route.params.id)));

function handleViewCar(carId) {
  router.push({ name: 'CarDetails', params: { id: carId } });
}

// Scroll functions
function scrollRight() {
  youMayScroll.value.scrollBy({ left: 300, behavior: 'smooth' });
}
function scrollLeft() {
  youMayScroll.value.scrollBy({ left: -300, behavior: 'smooth' });
}
</script>
