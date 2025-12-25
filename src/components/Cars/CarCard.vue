<template>
  <div 
    class="
      group
      w-full
      max-w-xs
      sm:max-w-sm
      lg:max-w-sm
      bg-base-100
      rounded-2xl
      overflow-hidden
      shadow-lg
      transition-all
      duration-300
      md:hover:shadow-2xl
      md:hover:-translate-y-2
      font-cairo
      flex
      flex-col
      mx-auto
    "
  >
    <!-- Image Section -->
    <div class="relative h-64 sm:h-64 overflow-hidden">
      <button
        class="btn btn-circle btn-sm absolute right-3 top-3 z-10 bg-base-100/80"
        :class="{ 'text-error': isWishlisted }"
        @click.stop="toggleWishlist"
        title="Save to wishlist"
      >
        <i :class="isWishlisted ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
      </button>
      <img 
        :src="getImage(car.images)"
        :alt="car.name"
        class="
          w-full
          h-full
          object-cover
          object-center
          transition-transform
          duration-500
          ease-in-out
          md:group-hover:scale-110
        "
        @error="handleImageError"
      />
      <!-- Gradient Overlay -->
      <div 
        class="
          absolute
          inset-0
          bg-gradient-to-t
          from-black/40
          via-black/10
          to-transparent
          opacity-0
          md:group-hover:opacity-100
          transition-opacity
          duration-300
        "
      />
    </div>

    <!-- Content Section -->
    <div class="p-4 flex flex-col flex-1">
      <!-- Title & Price -->
      <div class="flex items-start justify-between mb-2 gap-2">
        <h2 class="text-lg sm:text-lg lg:text-xl font-bold text-base-content flex-1 leading-snug">
          {{ car.name }}
        </h2>
        <span class="text-primary font-bold text-base sm:text-lg lg:text-xl flex-shrink-0 whitespace-nowrap">
          ${{ car.price }}
        </span>
      </div>

      <!-- Description-->
      <p class="text-sm sm:text-sm text-base-content/70 leading-relaxed line-clamp-2 flex-1 mb-3">
        {{ car.overview }}
      </p>

      <!-- Location & Rating -->
      <div class="flex items-center gap-2 text-sm mb-4">
        <i class="fas fa-star text-accent"></i>
        <span class="font-semibold text-base-content">
          {{ car.reviews.overallRating }}
        </span>
        <span class="text-base-content/60">
          ({{ car.reviews.totalReviews }} reviews)
        </span>
      </div>

      <!-- Button -->
      <router-link 
        :to="`/cars/details/${car.id}`"
        class="
          block
          text-center
          w-full
          bg-primary
          md:hover:bg-primary-focus
          text-primary-content
          font-bold
          py-2.5
          sm:py-3
          rounded-xl
          transition-all
          duration-300
          md:hover:shadow-lg
          active:scale-95
          mt-auto
        "
      >
        View Details
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useAuthStore } from '@/stores/authStore';
import { useWishlistStore } from '@/stores/wishlistStore';

const props = defineProps({
  car: Object
});

defineEmits(['view']);

const authStore = useAuthStore();
const wishlistStore = useWishlistStore();

const isWishlisted = computed(() => wishlistStore.isWishlisted('Car', props.car?.id));
const isSaving = computed(() => wishlistStore.isLoading);

const toggleWishlist = async () => {
  if (isSaving.value) return;
  if (!authStore.user?.id) {
    // Future: redirect to login or show toast
    return;
  }

  try {
    const payload = {
      itemId: props.car.id,
      itemType: 'Car',
      title: props.car.name,
      imageUrl: getImage(props.car.images),
      location: props.car.location || props.car.city || '',
      rating: props.car.reviews?.overallRating || null
    };

    if (isWishlisted.value) {
      await wishlistStore.removeFromWishlist('Car', props.car.id);
    } else {
      await wishlistStore.addToWishlist(payload);
    }
  } catch (error) {
    console.error('Wishlist toggle error:', error);
  }
};

const getImage = (images) => {
  if (!images) return '/images-car/placeholder.jpg';
  if (Array.isArray(images)) return images[0] || '/images-car/placeholder.jpg';
  return images; // Ensure string case is handled if data is not fixed yet
};

const handleImageError = (event) => {
  event.target.src = '/images-car/placeholder.jpg';
};
</script>

<style scoped>
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');

.line-clamp-2 {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
  -webkit-line-clamp: 2;
}
.active\:scale-95:active {
  transform: scale(0.95);
}
</style>
