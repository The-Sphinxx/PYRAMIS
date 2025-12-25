<template>
  <div 
    class="group bg-base-100 rounded-2xl overflow-hidden shadow-lg hover:shadow-2xl transition-all duration-300 hover:-translate-y-2 border border-base-300/50"
  >
    <!-- Image Container -->
    <div class="relative h-64 overflow-hidden">
      <button
        class="btn btn-circle btn-sm absolute right-3 top-3 z-10 bg-base-100/80"
        :class="{ 'text-error': isWishlisted }"
        @click.stop="toggleWishlist"
        title="Save to wishlist"
      >
        <i :class="isWishlisted ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
      </button>
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
        <h3 class="text-xl font-bold text-base-content font-cairo flex-1 leading-tight line-clamp-2 min-h-[3.25rem]">
          {{ title }}
        </h3>
        <span class="text-primary font-bold text-lg flex-shrink-0">
          {{ price }}
        </span>
      </div>

      <!-- Location -->
      <div class="flex items-center gap-1.5 mb-2 text-base-content/70 text-[13px]">
        <i class="fas fa-map-marker-alt text-primary text-sm"></i>
        <span>{{ location }}</span>
      </div>

      <!-- Rating -->
      <div class="flex items-center gap-1.5 mb-3">
        <i class="fas fa-star text-primary text-sm"></i>
        <span class="text-[13px] font-semibold text-base-content">{{ rating }} ({{ formattedReviews }} {{ reviewsText }})</span>
      </div>

      <!-- Button -->
      <router-link
        :to="`/attractions/details/${id}`"
        class="block text-center w-full bg-primary hover:bg-primary-focus text-primary-content font-bold py-3 px-4 rounded-xl transition-all duration-300 hover:shadow-lg active:scale-95"
      >
        {{ buttonText }}
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import wishlistApi from '@/Services/wishlistApi';

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
  },
  isWishlistedProp: {
    type: Boolean,
    default: false
  }
});

// Emits
const emit = defineEmits(['viewDetails']);

const authStore = useAuthStore();
const isWishlisted = ref(!!props.isWishlistedProp);
const isSaving = ref(false);

const toggleWishlist = async () => {
  if (isSaving.value) return;
  if (!authStore.user?.id) {
    // Future: redirect to login or show toast
    return;
  }

  isSaving.value = true;
  try {
    const payload = {
      itemId: props.id,
      itemType: 'Attraction',
      title: props.title,
      imageUrl: props.image,
      location: props.location,
      rating: props.rating || null
    };

    if (isWishlisted.value) {
      await wishlistApi.remove('Attraction', props.id);
      isWishlisted.value = false;
    } else {
      await wishlistApi.add(payload);
      isWishlisted.value = true;
    }
  } finally {
    isSaving.value = false;
  }
};

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

.line-clamp-2 {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
  -webkit-line-clamp: 2;
  line-clamp: 2;
}
</style>