<template>
  <div 
    class="group bg-base-100 rounded-2xl overflow-hidden shadow-lg hover:shadow-2xl transition-all duration-300 hover:-translate-y-2 border border-base-300/50"
  >
    <!-- Image Container -->
    <div class="relative h-64 overflow-hidden">
      <img 
        :src="image" 
        :alt="title"
        class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
        loading="lazy"
      />
      <!-- Gradient Overlay -->
      <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
    </div>

    <!-- Content -->
    <div class="p-5">
      <!-- Title and Price -->
      <div class="flex items-start justify-between mb-3 gap-2">
        <h3 class="text-xl font-bold text-base-content font-cairo flex-1 leading-tight">
          {{ title }}
        </h3>
        <span class="text-primary font-bold text-lg flex-shrink-0">
          {{ price }}
        </span>
      </div>

      <!-- Location -->
      <div class="flex items-center gap-2 mb-3 text-neutral">
        <i class="fas fa-map-marker-alt text-primary text-sm"></i>
        <span class="text-sm">{{ location }}</span>
      </div>

      <!-- Rating -->
      <div class="flex items-center gap-2 mb-4">
        <i class="fas fa-star text-primary text-sm"></i>
        <span class="font-bold text-base-content">{{ rating }}</span>
        <span class="text-sm text-neutral">
          ({{ formattedReviews }} {{ reviewsText }})
        </span>
      </div>

      <!-- Button -->
      <button
        @click="handleViewDetails"
        class="w-full bg-primary hover:bg-primary-focus text-primary-content font-bold py-3 px-4 rounded-xl transition-all duration-300 hover:shadow-lg active:scale-95"
      >
        {{ buttonText }}
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';

// Props
const props = defineProps({
  id: {
    type: Number,
    required: true
  },
  image: {
    type: String,
    required: true
  },
  title: {
    type: String,
    required: true
  },
  price: {
    type: [String, Number],
    required: true
  },
  location: {
    type: String,
    required: true
  },
  rating: {
    type: Number,
    required: true
  },
  reviews: {
    type: Number,
    required: true
  },
  buttonText: {
    type: String,
    default: 'View Details'
  },
  reviewsText: {
    type: String,
    default: 'reviews'
  }
});

// Emits
const emit = defineEmits(['viewDetails']);

// Computed
const formattedReviews = computed(() => {
  return props.reviews.toLocaleString();
});

// Methods
const handleViewDetails = () => {
  emit('viewDetails', {
    id: props.id,
    title: props.title,
    price: props.price,
    location: props.location,
    rating: props.rating,
    reviews: props.reviews
  });
};
</script>

<style scoped>
/* Import Font Awesome if not globally imported */
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');
</style>