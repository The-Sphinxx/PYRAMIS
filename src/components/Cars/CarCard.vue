<template>
  <div 
    class="group w-full sm:w-80 bg-base-100 rounded-2xl overflow-hidden shadow-2xl hover:shadow-3xl transition-all duration-300 hover:-translate-y-1 font-cairo flex flex-col"
  >
    <!-- Image Section -->
    <div class="relative h-64 sm:h-80 overflow-hidden">
      <img 
        :src="getImage(car.images)"
        :alt="car.name"
        class="w-full h-full object-cover object-center transition-transform duration-500 ease-in-out group-hover:scale-110"
        @error="handleImageError"
      />
      <!-- Gradient Overlay -->
      <div class="absolute inset-0 bg-gradient-to-t from-black/40 via-black/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
    </div>

    <!-- Content Section -->
    <div class="p-5 flex flex-col flex-1">
      <!-- Title & Price -->
      <div class="flex items-start justify-between mb-3 gap-2">
        <h2 class="text-xl sm:text-2xl font-bold text-base-content flex-1 leading-tight">
          {{ car.name }}
        </h2>
        <span class="text-primary font-bold text-lg sm:text-2xl flex-shrink-0">${{ car.price }}</span>
      </div>

      <!-- Description-->
      <p class="text-sm text-base-content/70 leading-relaxed line-clamp-2 flex-1 mb-3">
        {{ car.overview }}
      </p>

      <!-- Location & Rating -->
      <div class="flex items-center gap-2 text-sm mb-4">
        <i class="fas fa-star text-accent"></i>
        <span class="font-semibold text-base-content">{{ car.reviews.overallRating }}</span>
        <span class="text-base-content/60">({{ car.reviews.totalReviews }} reviews)</span>
      </div>

      <!-- Button -->
      <button 
        @click="$emit('view', car)"
        class="w-full bg-primary hover:bg-primary-focus text-primary-content font-bold py-3 rounded-xl transition-all duration-300 hover:shadow-lg active:scale-95 mt-auto"
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
defineEmits(['view']);

const getImage = (images) => images[0] || '/images-car/placeholder.jpg';

const handleImageError = (event) => {
  event.target.src = '/images-car/placeholder.jpg';
};
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
  -webkit-line-clamp: 2;
}
.active\:scale-95:active {
  transform: scale(0.95);
}
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');
</style>
