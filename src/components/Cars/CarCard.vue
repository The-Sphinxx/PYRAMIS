<template>
  <div class="w-full sm:w-80 bg-white rounded-3xl shadow-2xl overflow-hidden hover:shadow-3xl transition-all duration-300">
    <!-- Image Section -->
    <div class="relative bg-gradient-to-br from-amber-950 via-amber-900 to-amber-800 p-6 pb-8">
      <img 
        :src="car.image"
        :alt="car.name"
        class="w-full h-48 object-cover rounded-2xl shadow-xl"
      />
    </div>

    <!-- Content Section -->
    <div class="p-6 space-y-4">
      <div class="flex items-start justify-between">
        <h2 class="text-2xl font-bold text-gray-800">{{ car.name }}</h2>
        <span class="text-2xl font-bold text-orange-600">{{ car.year || '' }}</span>
      </div>

      <p class="text-sm text-gray-600 leading-relaxed">{{ car.description }}</p>

      <div class="flex items-center gap-2">
        <svg class="w-5 h-5 text-orange-500 fill-current" viewBox="0 0 24 24">
          <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
        </svg>
        <span class="text-sm font-semibold text-gray-800">{{ car.rating }}</span>
        <span class="text-sm text-gray-500">({{ formatNumber(car.reviews) }} reviews)</span>
      </div>

     <button 
  @click="$emit('view', car)"
  class="w-full bg-[#C86A41] hover:bg-[#b65c36] text-white font-semibold py-3.5 rounded-xl transition-all duration-200 shadow-md hover:shadow-lg active:scale-95 transform"
>
  View Details
</button>

    </div>
  </div>
</template>

<script setup>
defineProps({
  car: Object
});

const emit = defineEmits(['view']);

function formatNumber(num) {
  return num.toLocaleString();
}
</script>

<style scoped>
.active\:scale-98:active {
  transform: scale(0.98);
}
</style>
